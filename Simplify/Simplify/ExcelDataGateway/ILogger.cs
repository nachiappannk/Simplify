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
        Information,
        Warning,
        IgnorableError,
        Error,
    }
}
