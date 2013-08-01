using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Price_Comparitor
{
    class ExcelWriter
    {
        private static class ColumnConstants
        {
            public enum COLPOSIT : int
            { NAME, PRICE, UPC, PREF, OTHER, RANK }

            public static String[] ColumnNames = 
            { "Product Name", "Vendor Price", "UPC", "Lowest(Preferred)", "Lowest(Normal)", "Ranking in Video Games" };
        }

        private class _columnData
        {

            private string Name;
            private char Column;

            public _columnData(String name, char col)
            {
                this.Name = name;
                this.Column = col;
            }


            // Implicit/Explicit type conversion methods
            /*
             * Ex.
             * _columnData cd = new _columndData("Name", 'A');
             * String s = cd // s is now == "Name";
             * int i = cd // i is equal to 0 (int value of the Char - int value of 'A')
             * Char c = (char)cd // c == 'A' (this is *explicit* to save us from accidentally getting (string)cd)
             */

            public static implicit operator int(_columnData ColData)
            {
                return (int)ColData.Column - (int)'A';
            }

            public static explicit operator char(_columnData ColData)
            {
                return ColData.Column;
            }

            public static implicit operator string(_columnData ColData)
            {
                return ColData.Name;
            }
        }

        static readonly List<_columnData> colData;
        static readonly Queue<Product> _itemQueue;
        private readonly string _filePath;
        private CancellationToken _ct;

        public Task fileWriter = null;

        public TaskStatus Status()
        {
            return fileWriter.Status;
        }

        /// <summary>
        /// Create the constants that will be used for every instance of the class.
        /// </summary>
        static ExcelWriter()
        {
            _itemQueue = new Queue<Product>();
            colData = new List<_columnData>();



            for (int i = 0; i < ColumnConstants.ColumnNames.Length; i++)
            {
                colData.Add(new _columnData(ColumnConstants.ColumnNames[i], (Char)(((int)'A') + i)));
            }
        
        }

        /// <summary>
        /// A consumer that writes data to an Excel file asyncronously to the main thread.
        /// </summary>
        /// <param name="FilePath">Path to the Save File</param>
        /// <param name="CancellationToken">Cancellation Token</param>
        public ExcelWriter(String FilePath, CancellationToken CancellationToken)
        {
            this._filePath = FilePath;
            this._ct = CancellationToken;
        }

        public ExcelWriter(String FilePath, CancellationToken CancellationToken, List<Server> serverList)
            : this(FilePath, CancellationToken)
        {
            String[] serverCols = {"Preferred", "Normal", "Rank"};
            foreach (Server s in serverList.FindAll(s => !s.Servername.Equals("Amazon.com")))
            {
                if (s.Enabled)
                {
                    foreach (String category in serverCols)
                    {
                        colData.Add(new _columnData(s.Servername + " " + category, (Char)(((int)'A') + colData.Count())));
                    }
                }
            }
        }

        
        /// <summary>
        /// Save the Product object to an Excel file, as a separate thread under the default task scheduler
        /// </summary>
        /// <param name="Item">The Product object to save</param>
        public void SaveItem(Product Item)
        {
            // Add the Product object to the queue
            lock (_itemQueue) { Console.WriteLine("Queueing Row # " + Item.RowNumber); _itemQueue.Enqueue(Item); }

            // If the task thread hasn't been created/started yet, do so. Otherwise add the new continuation task.
            if (null == fileWriter)
            {
                fileWriter = new Task(() =>
                    { this.queueWork(); Thread.CurrentThread.Name = "ExcelWriter"; }, this._ct, TaskCreationOptions.LongRunning);

                fileWriter.Start();
            }
            else
            {
                fileWriter = fileWriter.ContinueWith(x =>
                    { this.queueWork(); }, this._ct, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Generic wrapper for adding the "work" to the task thread, for use by the SaveItem method.
        /// </summary>
        private void queueWork()
        {
            // Change the method call to suit whatever program is using this file-writing consumer.
            try { saveExcelFile(); }
            catch (AggregateException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// Open the Excel File for saving, or create it if it doesn't exist yet.
        /// </summary>
        /// <returns> The SpreadsheetDocument object point to the save file</returns>
        private SpreadsheetDocument openOrCreateExcel()
        {
            // Create the file if it doesn't exist
            FileInfo fileInfo = new FileInfo(this._filePath);
            if(!fileInfo.Exists)
                startExcelFile();

            // Try to open the document. If it's busy, sleep for 1/10th of a second and return null
            SpreadsheetDocument ssDoc = null;
            try { ssDoc = SpreadsheetDocument.Open(this._filePath, true); }
            catch (IOException iox) { Console.WriteLine(iox.Message);  Thread.Sleep(100); }

            return ssDoc;
        }

        /// <summary>
        /// Create the new Excel file, and add the header row.
        /// </summary>
        private void startExcelFile()
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Create(this._filePath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart wbPart = WorksheetTools.InsertWorkbook(doc);
                SharedStringTablePart sstPart = WorksheetTools.GetStringTablePart(doc);
                WorksheetPart wsPart = WorksheetTools.InsertWorksheet(wbPart);

                uint row = 1; // Not 0-based. Blame Microsoft for breaking standards.
                foreach(_columnData column in colData)
                    {
                        int index;
                        index = WorksheetTools.InsertSharedStringItem(column, sstPart);

                        Cell cell;
                        cell = WorksheetTools.InsertCellInWorksheet(((Char)column).ToString(), row, wsPart);
                        cell.CellValue = new CellValue(index.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    }

                wbPart.Workbook.Save();
                doc.Close();
            }
        }

        /// <summary>
        /// Write the information to the Excel file
        /// </summary>
        private void saveExcelFile()
        {
            // Check if the user cancelled the job
            if (this._ct.IsCancellationRequested) return;

            // Pop the next item off the FIFO queue.
            Product item;
            lock (_itemQueue) { item = _itemQueue.Dequeue(); }

            // Open the Excel document. If it's busy, keep retrying.
            // !!!!! Check this for the possibility of an infinite loop !!!!!!!!
            SpreadsheetDocument doc = null;
            while (null == doc) { doc = openOrCreateExcel(); }

            SharedStringTablePart sstPart = WorksheetTools.GetStringTablePart(doc);
            WorkbookPart wbPart = doc.WorkbookPart;
            WorksheetPart wsPart = (WorksheetPart)wbPart.GetPartById(wbPart.Workbook.Descendants<Sheet>().First<Sheet>().Id);

            // If the item doesn't have a row number assigned, put it in the bottom row.
            // This is a major bug in the works - what happens if it's assigned to the bottom row, and an
            // item is later added that *is* assigned a row, and it's that same row? Hmmm??
            uint row = item.RowNumber ?? (uint)wsPart.Worksheet.Descendants<Row>().Count() + 1;
            Product.ProductInfo info;
            try
            {
                info = item.ServerPrices
                    .DefaultIfEmpty(new KeyValuePair<String, Product.ProductInfo>("", new Product.ProductInfo()))
                    .First(P => P.Key.Equals("Amazon.com")).Value;
            }
            catch (InvalidOperationException)
            {
                info = new Product.ProductInfo();
            }

            foreach(_columnData column in colData)
            {
                String columnString;
                switch ((ColumnConstants.COLPOSIT)((int)column))
                {
                    case ColumnConstants.COLPOSIT.NAME:
                        columnString = item.name;
                        break;
                    case ColumnConstants.COLPOSIT.PRICE:
                        columnString = item.price;
                        break;
                    case ColumnConstants.COLPOSIT.UPC:
                        columnString = item.sku;
                        break;
                    case ColumnConstants.COLPOSIT.PREF:
                        columnString = info.Preferred;
                        break;
                    case ColumnConstants.COLPOSIT.OTHER:
                        columnString = info.Other;
                        break;
                    case ColumnConstants.COLPOSIT.RANK:
                        columnString = info.Rank;
                        break;
                    default:
                        Product.ProductInfo otherInfo;
                        try { otherInfo = item.ServerPrices.First(P => ((String)column).Contains(P.Key)).Value; }
                        catch (InvalidOperationException) { otherInfo = new Product.ProductInfo(); }

                        if (((string)column).Contains("Preferred")) columnString = otherInfo.Preferred;
                        else if (((string)column).Contains("Normal")) columnString = otherInfo.Other;
                        else if (((string)column).Contains("Rank")) columnString = otherInfo.Rank;
                        else columnString = "Invalid Column";
                        break;
                }

                // Add the data to the shared string table. If the data is NULL, add an empty string.
                // Otherwise, the shard string table will get multiple entries with nothing in them for each NULL.
                int index = WorksheetTools.InsertSharedStringItem((columnString ?? ""), sstPart);

                Cell cell;
                cell = WorksheetTools.InsertCellInWorksheet(((char)column).ToString(), row, wsPart);
                cell.CellValue = new CellValue(index.ToString());
                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
            }
            doc.Close();
        }
    }
}
