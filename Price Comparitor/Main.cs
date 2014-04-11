using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Price_Comparitor
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    public partial class Main : Form
    {
        private BackgroundWorker bgWorker;
        private String saveFile;
        private String fileName;
        private CancellationTokenSource cToken = new CancellationTokenSource();

        public List<Server> svrs = new List<Server>();
        public bool sleep = false;

        public Main()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExHandler);

            // Exchange Rate Acronyms pulled from www.oanda.com

            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);


            svrs.Add(new Server("Amazon.com", "http://www.amazon.com/s/ref=nb_sb_noss?url=search-alias%3Daps&field-keywords=", float.Parse("1.35")));
            svrs.Add(new Server("Amazon.ca", "http://www.amazon.ca/s/ref=nb_sb_noss?url=search-alias%3Daps&field-keywords=", float.Parse("1.35"), "CAD", false));
            svrs.Add(new Server("Amazon.co.jp", "http://www.amazon.co.jp/s/ref=nb_sb_noss?__mk_ja_JP=%83J%83%5E%83J%83i&url=search-alias%3Daps&field-keywords=", float.Parse("1.35"), "JPY", false));
            //svrs.Add(new Server("Mobile Amazon", "http://www.amazon.com/gp/aw/s/ref=is_box_?k=", float.Parse("1.35")));

            bgWorker = new BackgroundWorker();
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.WorkerReportsProgress = true;
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.ProgressChanged += bgWorker_ProgressChanged;
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
            Microsoft.Win32.SystemEvents.PowerModeChanged += OnPowerChange;

        }

        void OnPowerChange(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Resume:
                    sleep = false;
                    break;
                case Microsoft.Win32.PowerModes.Suspend:
                    sleep = true;
                    break;
                default:
                    break;
            }
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Invoke((MethodInvoker)(() => { progressBar1.Visible = false; }));
            fileBrowseBtn.Invoke((MethodInvoker)(() => { fileBrowseBtn.Visible = true; }));
            cnclBtn.Invoke((MethodInvoker)(() => { cnclBtn.Visible = false; }));

            // Cleanup the temp xlsx file, if we needed to make one.
            if (fileName.Contains(Path.GetTempPath()))
                System.IO.File.Delete(fileName);

            WorksheetTools.ValidateXLFile(saveFile);
            MessageBox.Show("Completed");
        }

        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Invoke((MethodInvoker)(() => { progressBar1.PerformStep(); }));
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ExcelWriter writer = new ExcelWriter(saveFile, cToken.Token, svrs);

            List<Product> pList = (List<Product>)e.Argument;

            Parallel.ForEach(pList, new ParallelOptions { MaxDegreeOfParallelism = 6 }, 
                new Action<Product, ParallelLoopState>((Product item, ParallelLoopState state) =>
                {
                    if(cToken.IsCancellationRequested) state.Break();

                    try { Thread.CurrentThread.Name = "BG-Worker Thread"; }
                    catch (InvalidOperationException) { }

                    while (sleep) Thread.Sleep(120000);

                    if (((BackgroundWorker)sender).CancellationPending) lock (e) e.Cancel = true;

                    item.fillResults(svrs);
                    writer.SaveItem(item); // We're missing lines - does this need to be locked?
                    bgWorker.ReportProgress(0);

                    if (item.subTree.Count > 0)
                    {
                        treeView1.Invoke((MethodInvoker)(() => 
                        { 
                            treeView1.Nodes.Add(new TreeNode(item.name + " (" + item.sku + ") - " + item.price, item.subTree.ToArray())); 
                        }));
                    }
                    else
                    {
                        //IF no results, add the thing anyway. We'll see that it doesn't have any children
                        treeView1.Invoke((MethodInvoker)(() => 
                        { 
                            treeView1.Nodes.Add(new TreeNode(item.name + " (" + item.sku + ") - " + item.price)); 
                        }));
                    }
                }));

            TaskStatus status = writer.Status();
            while (status != TaskStatus.Canceled && status != TaskStatus.Faulted && status != TaskStatus.RanToCompletion)
            {
                Console.WriteLine("Waiting for file writer to finish...");
                Thread.Sleep(1000);
                status = writer.Status();
            }
        }

        /// <summary>
        /// Unhandled Exception handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ExHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show("Unhandled Exception thrown:\n" + e.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Take an older Excel file (*.xls) and save it as an OpenXML document, in the system temp folder
        /// </summary>
        /// <param name="fileName">Full path of XLS file to re-save as XLSX</param>
        /// <returns>Full path of new XLSX file</returns>
        private string convertXLS(string fileName)
        {
            String tempFilePath = string.Empty;
            Excel.Application app = null;

            // Try-Catch-Finally is what "using(stuff)" does anyway. This lets us use Exception handling and types that aren't
            // supported by using() (ie. those types that aren't implicitly convertable to IDisposable)
            try
            {
                String file = Path.GetFileNameWithoutExtension(fileName);
                String path = Path.GetTempPath();
                tempFilePath = path + file + ".xlsx";
                app = new Excel.Application();
                app.Visible = false;
                app.DisplayAlerts = false;
                Excel.Workbook wb = app.Workbooks.Open(fileName, 0, false);

                wb.SaveAs(tempFilePath, Excel.XlFileFormat.xlOpenXMLWorkbook, Type.Missing,
                            false, false, Excel.XlSaveConflictResolution.xlUserResolution, Excel.XlSaveAsAccessMode.xlNoChange,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                app.Quit();
            }

            return tempFilePath;
        }

        /// <summary>
        /// Open the Excel file, and process it. The MEAT of the program.
        /// </summary>
        /// <param name="fileName">Path to the Excel file</param>
        private void openExcel()
        {
            // Convert older Excel files to OpenXML (*.xlsx) files for processing
            if (Path.GetExtension(fileName).ToLower().Equals(".xls"))
                fileName = convertXLS(fileName);

            progressBar1.Visible = true;
            progressBar1.Message = "Loading vendor list...";
            List<Product> pList = makeList(fileName); // Stubb out the list of Products

            progressBar1.Message = null;
            progressBar1.Maximum = pList.Count();
            progressBar1.Value = 0;

            bgWorker.RunWorkerAsync(pList);
        }

        /// <summary>
        /// Make a list of Products, loaded with information from the Excel file
        /// </summary>
        /// <param name="fileName">Full path + filename of the Excel File to open</param>
        /// <returns>Completed List of Product objects</returns>
        private List<Product> makeList(string fileName)
        {


            SpreadsheetDocument doc = null;
            List<Product> pList = new List<Product>();
            using (doc = SpreadsheetDocument.Open(fileName, false))
            {
                SharedStringTable stringTable = doc.WorkbookPart.SharedStringTablePart.SharedStringTable;
                WorksheetPart sheet1 = (WorksheetPart)doc.WorkbookPart.GetPartById(doc.WorkbookPart.Workbook.Descendants<Sheet>().First<Sheet>().Id);
                IEnumerable<Row> rows = sheet1.Worksheet.Descendants<Row>();

                string nameCell, skuCell, priceCell;
                nameCell = skuCell = priceCell = string.Empty;
                Boolean foundHeaders = false;
                foreach (Row row in rows)
                {
                    Product item = new Product(cToken.Token);
                    foreach (Cell cell in row.Descendants<Cell>())
                    {

                        // If the top row has been read, ALL THREE of the needed columns haven't been detected, open the Column Select dialog
                        // to let the user manually select the columns.
                        if (row.RowIndex > 1 && !foundHeaders)
                        {
                            using (ColumnSelect columnSelect = new ColumnSelect(rows, stringTable))
                            {
                                columnSelect.ShowDialog();
                                nameCell = columnSelect.ItemName;
                                skuCell = columnSelect.SKU;
                                priceCell = columnSelect.Price;
                                foundHeaders = true;
                            }
                        }


                        if (cell.CellValue != null)
                        {
                            if (row.RowIndex == 1) // Header row
                            {
                                if (cell.DataType != null && cell.DataType.HasValue && cell.DataType == CellValues.SharedString)
                                {
                                    string headerCell = stringTable.ChildElements[int.Parse(cell.CellValue.InnerText)].InnerText.ToLower();
                                    
                                    // Best-guess as to the strings in the vendor's header row
                                    if (headerCell.Contains("title") || headerCell.Contains("description"))
                                    {
                                        // Cell Col is a letter, Row is a number. This strips off the Row so we have just the col
                                        nameCell = Regex.Replace(cell.CellReference.InnerText, "[0-9]", "");
                                    }
                                    else if (headerCell.Contains("upc"))
                                        skuCell = Regex.Replace(cell.CellReference.InnerText, "[0-9]", "");
                                    else if (headerCell.Contains("price"))
                                        priceCell = Regex.Replace(cell.CellReference.InnerText, "[0-9]", "");
                                    else
                                    {
                                        //Do Nothing
                                    }
                                }

                                if (!String.IsNullOrEmpty(nameCell) & !String.IsNullOrEmpty(skuCell) & !String.IsNullOrEmpty(priceCell))
                                    foundHeaders = true;
                            }
                            else
                            {
                                // If the cell's Column is one of the specified Header columns, save it's value. Otherwise, ignore it
                                if (cell.CellReference.InnerText.Contains(nameCell))
                                {
                                    item.name = getCellText(cell, ref stringTable);
                                }
                                else if (cell.CellReference.InnerText.Contains(skuCell))
                                {
                                    item.sku = getCellText(cell, ref stringTable);
                                }
                                else if (cell.CellReference.InnerText.Contains(priceCell))
                                {
                                    item.price = Single.Parse(getCellText(cell, ref stringTable)).ToString("C2");
                                }
                                else
                                {
                                    if (null == item.RowNumber)
                                    {
                                        item.RowNumber = row.RowIndex;
                                    }
                                }
                            }
                        }
                    }
                    if (row.RowIndex > 1 && item.name != null) // Don't add the header row to the list
                        pList.Add(item);
                }
            }
            return pList;
        }

        /// <summary>
        /// Get the text contained in a single cell 
        /// (OpenXML documents use a cell as an index to an array of Strings, called the Shared String Table. The cell itself doesn't store the String)
        /// </summary>
        /// <param name="cell">The cell the use as an index in the Shared String Table</param>
        /// <param name="stringTable">The Shared String Table to pull the text out of</param>
        /// <returns>String from the String Table</returns>
        private string getCellText(Cell cell, ref SharedStringTable stringTable)
        {
            return (cell.DataType != null
                                        ? stringTable.ChildElements[int.Parse(cell.CellValue.InnerText)].InnerText
                                        : cell.CellValue.InnerText);
        }

        private delegate void MovePBDelegate();
        private delegate void AddTreeNode(TreeNode node);
        
        /// <summary>
        /// Open the dialog to find the Excel file, and kick off the party.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileBrowseBtn_Click(object sender, System.EventArgs e)
        {
            treeView1.Nodes.Clear(); // Reset the tree, in case of doing a second search

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
            openFileDialog.Title = "Select your price list";

            // If the user chose a file, setup the view and go go go.
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Price List Save Location";
                saveFileDialog.DefaultExt = "xlsx";
                saveFileDialog.Filter = "Excel Files|*.xlsx;*.xls|All Files (*.*)|*.*";

                // If the user chose a file, setup the view and go go go.
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    saveFile = saveFileDialog.FileName;

                    FileInfo fileInfo = new FileInfo(saveFile);
                    if (fileInfo.Exists) fileInfo.Delete();

                    fileBrowseBtn.Visible = false;
                    cnclBtn.Visible = true;
                    fileName = openFileDialog.FileName;
                    openExcel(); // Do the work.
                }
            }
        }

        /// <summary>
        /// STOP the load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cnclBtn_Click(object sender, EventArgs e)
        {
            cToken.Cancel();
            bgWorker.CancelAsync();
        }

        /// <summary>
        /// Try to stop the load before closing the window. Kickback from Actions.Do
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            cToken.Cancel();
        }

        /// <summary>
        /// Show the Amazon Servers options menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void amazonServersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerConfig sc = new ServerConfig(svrs);
            sc.ShowDialog();
        }

        // Rt-Mouse-Click menu, for copying the text in a tree node
        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
                if (treeView1.SelectedNode != null)
                    treeViewRtMouseClick.Show(treeView1, e.Location);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(treeView1.SelectedNode.Text);
        }
    }
}
