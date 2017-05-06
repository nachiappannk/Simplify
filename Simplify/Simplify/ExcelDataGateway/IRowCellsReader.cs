using System;

namespace Simplify.ExcelDataGateway
{
    public interface IRowCellsReader
    {
        DateTime ReadDate(int zeroBasedColumnIndex);
        int ReadInteger(int zeroBasedColumnIndex);
        double ReadDouble(int zeroBasedColumnIndex);
        string ReadString(int zeroBasedColumnIndex);
    }
}