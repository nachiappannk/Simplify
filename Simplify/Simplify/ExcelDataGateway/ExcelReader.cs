using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace Simplify.ExcelDataGateway
{
    public class ExcelReader : IDisposable
    {
        private readonly ILogger _logger;
        private string FileName { get; set; }
        private string SheetName { get; set; }
        private ExcelPackage _package;
        private ExcelWorksheet _sheet;
        private FileStream _stream;

        public ExcelReader(string filename, string sheetName, ILogger logger)
        {
            _logger = logger;
            FileName = filename;
            SheetName = sheetName;
            AssertSheetExists(filename, sheetName);
            _stream = ExcelSheetInfoProvider.GetFileStream(filename);
            _package = new ExcelPackage(_stream);
            _sheet = _package.Workbook.Worksheets[sheetName];
        }

        private static void AssertSheetExists(string excelFileName, string sheetName)
        {
            ExcelSheetInfoProvider sheetInfoProvider = new ExcelSheetInfoProvider(excelFileName);
            if (!sheetInfoProvider.IsSheetPresent(sheetName))
            {
                throw new Exception(sheetName + ": sheet does not exist");
            }
        }


        public T ReadLine<T>(int zeroBasedRowIndex, Func<IRowCellsReader, T> rowToObjectConvertor)
        {
            
            RowCellsReader rowCellsReader = new RowCellsReader(_sheet, zeroBasedRowIndex, FileName, SheetName, _logger);
            return rowToObjectConvertor.Invoke(rowCellsReader);
        }

        public IList<T> ReadAllLines<T>(int zeroBasedStartRowIndex, Func<IRowCellsReader, T> rowToObjectConvertor)
        {
            
            var numberOfRows = _sheet.Dimension.Rows;
            List<T> results = new List<T>();
            for (int i = zeroBasedStartRowIndex; zeroBasedStartRowIndex < numberOfRows; zeroBasedStartRowIndex++)
            {
                RowCellsReader rowCellsReader = new RowCellsReader(_sheet, zeroBasedStartRowIndex, FileName, SheetName, _logger);
                results.Add(rowToObjectConvertor.Invoke(rowCellsReader));
            }
            return results;
        }

        public void Dispose()
        {
            
            _sheet.Dispose();
            _package.Dispose();
            _stream.Dispose();
        }
    }
}