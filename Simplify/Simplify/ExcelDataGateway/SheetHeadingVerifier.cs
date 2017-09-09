using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simplify.ExcelDataGateway
{
    public class SheetHeadingVerifier
    {
        public static void VerifyHeadingNames(ILogger logger, ExcelReader reader, List<List<string>> columnsHeadingOptions)
        {
            reader.ReadLine(0,
                r =>
                {
                    string[] ret = new string[columnsHeadingOptions.Count];
                    for (int i = 0; i < columnsHeadingOptions.Count; i++)
                    {
                        var readColumnName = r.ReadString(i);
                        ret[i] = readColumnName;
                        var columnNameOptions = columnsHeadingOptions[i];
                        LogUnmatchedColumnName(logger, r.FileName, r.SheetName, readColumnName, columnNameOptions);
                    }
                    return ret;
                });
        }

        public static void LogUnmatchedColumnName(ILogger logger, string fileName, string sheetName, string columnName, List<string> columnNameOptions)
        {
            var columnNameLower = columnName.ToLower();
            var columnNameOptionsLower = columnNameOptions.Select(x => x.ToLower()).ToList();
            if (!columnNameOptionsLower.Contains(columnNameLower))
            {
                StringBuilder b = new StringBuilder();
                b.AppendLine($"In File {fileName}, ");
                b.AppendLine($"In Sheet {sheetName}, ");
                b.AppendLine($"Expected heading {columnNameOptions.ElementAt(0)} but found {columnName}");
                logger.Log(MessageType.IgnorableError, b.ToString());
            }
        }
    }
}