using System;

namespace Simplify.ExcelDataGateway
{
    public interface IRowCellsReader
    {
        string SheetName { get; }
        string FileName { get; }
        int LineNumber { get; }
        DateTime ReadDate(int zeroBasedColumnIndex);
        int ReadInteger(int zeroBasedColumnIndex);
        double ReadDouble(int zeroBasedColumnIndex);
        string ReadString(int zeroBasedColumnIndex);
        bool IsValueAvailable(int zeroBasedColumnIndex);
    }
}