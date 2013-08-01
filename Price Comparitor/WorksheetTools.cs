using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Validation;
using System;
using System.Linq;

namespace Price_Comparitor
{
    public static class WorksheetTools
    {
        public static SharedStringTablePart GetStringTablePart(SpreadsheetDocument doc)
        {
            if (doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                return doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
            else
                return doc.WorkbookPart.AddNewPart<SharedStringTablePart>();
        }

        public static WorkbookPart InsertWorkbook(SpreadsheetDocument doc)
        {
            WorkbookPart wbPart = doc.AddWorkbookPart();
            wbPart.Workbook = new Workbook();
            return wbPart;
        }

        public static Cell InsertCellInWorksheet(String column, uint rowIndex, WorksheetPart wsPart)
        {
            Worksheet worksheet = wsPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = column + rowIndex;
            Row row;

            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };

                /* 
                 * Need to put the rows in numeric order, or Excel will claim it's unreadable data when the file is opened,
                 * and remove the offending rows
                 * 
                 * For Example, if the rows are "5, 9, 2, 3, 12, 6, 13, 14", 
                 * then Excel will remove 2, 3, and 6 - any that are Less-Than the highest row before them. 
                 */

                Row lastRow = findPreviousRow(sheetData, rowIndex);
                if (null == lastRow)
                {
                    sheetData.Append(row);
                }
                else
                {
                    sheetData.InsertAfter<Row>(row, lastRow);
                }
            }

            if (row.Elements<Cell>().Where(c => c.CellReference.Value == column + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }

        private static Row findPreviousRow(SheetData sheetData, uint targetIndex)
        {
            uint childIndex = 0;
            Row lastRow = null;

            foreach(Row row in sheetData.Elements<Row>().Where(r => r.RowIndex < targetIndex))
            {
                if (row.RowIndex > childIndex)
                {
                    childIndex = row.RowIndex;
                    lastRow = row;
                }
            }
            return lastRow;
        }

        public static int InsertSharedStringItem(string text, SharedStringTablePart part)
        {
            if (part.SharedStringTable == null)
                part.SharedStringTable = new SharedStringTable();

            int i = 0;
            foreach (SharedStringItem item in part.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText.Equals(text))
                    return i;
                i++;
            }

            part.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            part.SharedStringTable.Save();
            return i;
        }

        // Given a WorkbookPart, inserts a new worksheet.
        public static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
        {
            return InsertWorksheet(workbookPart, string.Empty);
        }

        public static WorksheetPart InsertWorksheet(WorkbookPart workbookPart, string sheetName)
        {
            // Add a new worksheet part to the workbook.
            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            //Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            Sheets sheets = workbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            if(string.IsNullOrEmpty(sheetName))
                sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart;
        }

        public static void ValidateXLFile(String saveFile)
        {
            try
            {
                OpenXmlValidator validator = new OpenXmlValidator();
                int count = 0;
                foreach (ValidationErrorInfo error in
                      validator.Validate(SpreadsheetDocument.Open(saveFile, true)))
                {
                    count++;
                    Console.WriteLine("Error " + count);
                    Console.WriteLine("Description: " + error.Description);
                    Console.WriteLine("Path: " + error.Path.XPath);
                    Console.WriteLine("Part: " + error.Part.Uri);
                    Console.WriteLine("-----------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
