using System;
using System.Runtime.InteropServices.WindowsRuntime;
using OfficeOpenXml;

namespace Simplify.ExcelDataGateway
{
    public class RowCellsReader : IRowCellsReader
    {
        private readonly ExcelWorksheet _excelWorksheet;
        private readonly int _zeroBasedRowIndex;
        private readonly int _rowLimit;
        private readonly int _columnLimit;

        public RowCellsReader(ExcelWorksheet excelWorksheet, 
            int zeroBasedRowIndex)
        {
            _excelWorksheet = excelWorksheet;
            _zeroBasedRowIndex = zeroBasedRowIndex;
            _rowLimit = excelWorksheet.Dimension.Rows;
            _columnLimit = excelWorksheet.Dimension.Columns;
        }

        public DateTime ReadDate(int zeroBasedColumnIndex)
        {
            
            var value = ReadCell(zeroBasedColumnIndex);
            if (value == null) return DateTime.FromOADate(0); 
            if (value is int) return DateTime.FromOADate((int)value);
            if (value is double) return DateTime.FromOADate((double)value);
            if (value is string) return Convert.ToDateTime((string)value);
            if (value is DateTime) return (DateTime) value;
            return DateTime.MinValue;
        }

        private object ReadCell(int zeroBasedColumnIndex)
        {
            if (_zeroBasedRowIndex >= _rowLimit) return null;
            if (zeroBasedColumnIndex >= _columnLimit) return null;
            var value =  _excelWorksheet.Cells[_zeroBasedRowIndex+1, zeroBasedColumnIndex+1].Value;
            return value;
        }

        public int ReadInteger(int zeroBasedColumnIndex)
        {
            var value = ReadCell(zeroBasedColumnIndex);
            if (value == null) return 0;
            if (value is int) return (int) value;
            if (value is double) return (int) (double) value;
            if (value is string) return Convert.ToInt32((string) value);
            return 0;
        }

        public double ReadDouble(int zeroBasedColumnIndex)
        {
            var value = ReadCell(zeroBasedColumnIndex);
            if (value == null) return 0;
            if (value is int) return (int)value;
            if (value is double) return (double)value;
            if (value is string) return Convert.ToDouble((string)value);
            return 0;
        }

        public string ReadString(int zeroBasedColumnIndex)
        {
            var x = ReadCell(zeroBasedColumnIndex);
            return x?.ToString() ?? string.Empty;
        }
    }
}