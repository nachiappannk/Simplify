using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace Simplify.ExcelDataGateway
{
    public class ExcelSheetInfoProvider
    {
        private readonly string _excelFileName;

        public ExcelSheetInfoProvider(string excelFileName)
        {
            _excelFileName = excelFileName;
        }

        public IList<string> GetSheetNames()
        {
            using (var package = GetReadOnlyExcelPackage(_excelFileName))
            {
                return package.Workbook.Worksheets.Select(s => s.Name).ToList();
            }
        }

        private static void AssertFileExtentionIsXlsx(string excelFileName)
        {
            if (Path.GetExtension(excelFileName) != ".xlsx")
            {
                throw new Exception("The extention is not xlsx");
            }
        }

        private static void AssertFileExists(string excelFileName)
        {
            if (!File.Exists(excelFileName))
            {
                throw new Exception("File Does Not Exist");
            }
        }

        private static FileStream GetFileStream(string excelFileName)
        {
            FileStream stream = File.Open(excelFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return stream;
        }

        public static ExcelPackage GetReadOnlyExcelPackage(string excelFileName)
        {
            AssertFileExists(excelFileName);
            AssertFileExtentionIsXlsx(excelFileName);
            var stream = GetFileStream(excelFileName);
            return new ExcelPackage(stream);
        }
    }

    public static class ExcelSheetInfoProviderExt
    {
        public static bool IsSheetPresent(this ExcelSheetInfoProvider infoProvider, string sheetName)
        {
            return infoProvider.GetSheetNames().Select(x => x.ToLower()).Contains(sheetName.ToLower());
        }
    }


}