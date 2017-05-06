namespace Simplify.ExcelDataGateway
{
    public class SheetHeadingLogger
    {
        public static void LogHeadingRowDetails(ILogger logger, ExcelReader reader, string[] columnNames)
        {
            string[] headings = reader.ReadLine(0,
                r =>
                {
                    string[] ret = new string[columnNames.Length];
                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        ret[i] = r.ReadString(i);
                        logger.Info("Read " + ret[i] + " as " + columnNames[i]);
                    }
                    return ret;
                });
        }
    }
}