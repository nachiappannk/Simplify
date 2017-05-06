using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Simplify.ExcelDataGateway
{
    public class ExcelWriter : IDisposable
    {
        private ExcelPackage _package;
        private ExcelWorksheet _workSheet;
        public ExcelWriter(string fileName, string sheetName)
        {
            FileInfo file = new FileInfo(fileName);
            _package = new ExcelPackage(file);
            _workSheet = _package.Workbook.Worksheets.Add(sheetName);
            
        }

        public void Write(int zeroBasedRowIndex, params object[] values)
        {
            for (int j = 0; j < values.Length; j++)
            {
                int column = j + 1;
                int row = zeroBasedRowIndex + 1;
                _workSheet.Cells[row, column].Value = values[j];
                if (values[j] is double)
                    _workSheet.Cells[row, column].Style.Numberformat.Format = "#,##0.00";
                else if (values[j] is DateTime)
                    _workSheet.Cells[row, column].Style.Numberformat.Format = "dd-mmm-yy";
            }
        }

        public void WriteList<T>(int zeroBasedStartingRowIndex, IList<T> items,
            Func<T ,int, object[]> itemInterpretor)
        {
            int row = zeroBasedStartingRowIndex;
            foreach (var item in items)
            {
                Write(row++, itemInterpretor.Invoke(item, row));
            }
        }

        public void SetColumnsWidth(params double[] widths)
        {
            for (int i = 1; i <= widths.Length; i++)
            {
                _workSheet.Column(i).Width = widths[i - 1];
            }
        }

        public void ApplyHeadingFormat(int numberOfColumns)
        {
            using (var range = _workSheet.Cells[1, 1, 1, numberOfColumns])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                range.Style.Font.Color.SetColor(Color.White);
                range.AutoFilter = true;
            }
        }

        public void Dispose()
        {
            _package.Save();
            _workSheet.Dispose();
            _package.Dispose();
        }
    }
}