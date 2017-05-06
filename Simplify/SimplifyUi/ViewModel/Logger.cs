using System.Collections.Generic;
using Simplify.ExcelDataGateway;

namespace Simplify.ViewModel
{
    public class Logger : ILogger
    {
        List<string> _messages = new List<string>();
        public void Info(string message)
        {
            _messages.Add("Info: " + message);
        }

        public void Error(string message)
        {
            _messages.Add("Error: " + message);
        }

        public List<string> GetLogMessages()
        {
            return _messages;
        }
    }
}