using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplify.ExcelDataGateway
{
    public interface ILogger
    {
        void Log(MessageType type, string message);
    }

    public enum MessageType
    {
        Information = 0,
        Warning = 1,
        IgnorableError = 2,
        Error = 3,
    }
}
