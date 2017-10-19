using System;

namespace SimplifyUi.Common.ViewModelTools
{
    public class OutputNameComputer
    {
        public static string ComputeOutputFile(string output, string extention)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputExcelFileName = output + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss_fff");
            string fullPath = path + "\\" + outputExcelFileName + extention;
            return fullPath;
        }
    }
}