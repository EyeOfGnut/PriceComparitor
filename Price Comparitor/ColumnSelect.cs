using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Price_Comparitor
{
    public partial class ColumnSelect : Form
    {
        private IEnumerable<Row> rows;
        private SharedStringTable stringTable;

        public string ItemName, SKU, Price;

        public ColumnSelect(IEnumerable<Row> rows, SharedStringTable stringTable)
        {
            InitializeComponent();
            this.rows = rows;
            this.stringTable = stringTable;
        }

        private void loadTable()
        {
            Point cbPoint = cb1.Location;
            Row row = rows.ElementAt(0);
            foreach (Cell cell in row.Descendants<Cell>())
            {
                if (cell.CellValue != null)
                {
                    if (cell.CellReference.InnerText.Equals("A1"))
                    {
                        cb1.Visible = true;
                        cb1.Tag = Regex.Replace(cell.CellReference.InnerText, "[0-9]", "");
                        cb1.Name = "cb" + Regex.Replace(cell.CellReference.InnerText, "[0-9]", "");
                        lb1.Visible = true;
                        lb1.Text = stringTable.ChildElements[int.Parse(cell.CellValue.InnerText)].InnerText;
                        lb1.Name = "lb" + Regex.Replace(cell.CellReference.InnerText, "[0-9]", "");
                    }
                    else
                    {
                        cbPoint.Y += 30;

                        ComboBox cb = new ComboBox();
                        foreach (Object o in cb1.Items)
                            cb.Items.Add(o);

                        cb.DropDownStyle = cb1.DropDownStyle;
                        cb.Name = "cb" + Regex.Replace(cell.CellReference.InnerText, "[0-9]", "");
                        cb.Tag = Regex.Replace(cell.CellReference.InnerText, "[0-9]", "");
                        cb.Location = cbPoint;
                        dataPanel.Controls.Add(cb);

                        Label lb = new Label();
                        lb.Name = "lb" + Regex.Replace(cell.CellReference.InnerText, "[0-9]", "");
                        lb.Text = stringTable.ChildElements[int.Parse(cell.CellValue.InnerText)].InnerText;
                        Point p = cbPoint;
                        p.X += 140;
                        p.Y += 3;
                        lb.Location = p;
                        dataPanel.Controls.Add(lb);
                    }
                }
            }
        }

        private void ColumnSelect_Load(object sender, EventArgs e)
        {
            loadTable();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            char[] cols = Enumerable.Range('A', 'Z' - ('A' + 1)).Select(i => (Char)i).ToArray(); // Or "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            
            for (int i = 0; i <= (dataPanel.Controls.Count)/2; i++)
            {
                System.Windows.Forms.Control[] cb = dataPanel.Controls.Find("cb" + cols[i], true);
                if (cb.Length > 0)
                {
                    switch(((ComboBox)cb[0]).SelectedIndex)
                    {
                        case 1:
                            ItemName = ((ComboBox)cb[0]).Tag.ToString();
                            break;
                        case 2:
                            SKU = ((ComboBox)cb[0]).Tag.ToString();
                            break;
                        case 3:
                            Price = ((ComboBox)cb[0]).Tag.ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
            this.Close();
        }
    }
}
