using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace Simplify.ExcelDataGateway
{
    public class ExcelReader
    {
        public T ReadLine<T>(ExcelWorksheet workSheet, int zeroBasedRowIndex, Func<IRowCellsReader, T> rowToObjectConvertor)
        {

            RowCellsReader rowCellsReader = new RowCellsReader(workSheet, zeroBasedRowIndex);
            return rowToObjectConvertor.Invoke(rowCellsReader);
        }

        public IList<T> ReadAllLines<T>(ExcelWorksheet workSheet, int zeroBasedStartRowIndex, Func<IRowCellsReader, T> rowToObjectConvertor)
        {
            
            var numberOfRows = workSheet.Dimension.Rows;
            List<T> results = new List<T>();
            for (int i = zeroBasedStartRowIndex; zeroBasedStartRowIndex < numberOfRows; zeroBasedStartRowIndex++)
            {
                RowCellsReader rowCellsReader = new RowCellsReader(workSheet, zeroBasedStartRowIndex);
                results.Add(rowToObjectConvertor.Invoke(rowCellsReader));
            }
            return results;
        }
    }
}