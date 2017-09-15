using System.Collections.Generic;
using System.Linq;
using Simplify.Application;
using Simplify.ExcelDataGateway;

namespace SimplifyUi.Common.ViewModelTools
{
    public class Logger : ILogger
    {
        private class InternalMessage
        {
            public MessageType MessageType { get; set; }
            public string MessageInfo { get; set; }
        }

        List<InternalMessage> _messages = new List<InternalMessage>();

        public void Log(MessageType messageType, string message)
        {
            _messages.Add(new InternalMessage()
            {
                MessageType = messageType,
                MessageInfo = message,
            });
        }

        public List<Message> GetLogMessages()
        {
            return _messages.OrderBy(x => x.MessageType)
                .Select(x => MessageFactory.GetMessage(x.MessageType, x.MessageInfo)).ToList();
        }
    }

    public class Message
    {
        public Message(string messageInfo)
        {
            MessageInfo = messageInfo;  
        }
        public string MessageInfo { get; set; }
    }

    public static class MessageFactory
    {
        public static Message GetMessage(MessageType messageType, string info)
        {
            switch (messageType)
            {
                case MessageType.Information:
                    return new InformationMessage(info);
                case MessageType.Warning:
                    return new WarningMessage(info);
                case MessageType.IgnorableError:
                    return new IgnorableErrorMessage(info);
                case MessageType.Error:
                    return new ErrorMessage(info);
                default:
                    return new Message(info);
            }
        }
    }

    public class InformationMessage : Message
    {
        public InformationMessage(string messageInfo) : base(messageInfo)
        {
        }
    }

    public class WarningMessage : Message
    {
        public WarningMessage(string messageInfo) : base(messageInfo)
        {
        }
    }

    public class IgnorableErrorMessage : Message
    {
        public IgnorableErrorMessage(string messageInfo) : base(messageInfo)
        {
        }
    }

    public class ErrorMessage : Message
    {
        public ErrorMessage(string messageInfo) : base(messageInfo)
        {
        }
    }
}