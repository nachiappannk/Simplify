using System.Collections.Generic;
using Simplify.ExcelDataGateway;

namespace SimplifyUi.Common.ViewModelTools
{
    public class Logger : ILogger
    {
        List<string> _messages = new List<string>();

        public void Log(MessageType messageType, string message)
        {
            _messages.Add(message);
        }

        public List<string> GetLogMessages()
        {
            return _messages;
        }
    }
}