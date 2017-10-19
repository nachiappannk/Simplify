using Prism.Commands;

namespace SimplifyUi.Common.ViewModel
{
    public class NamedCommand
    {
        public NamedCommand(string name, DelegateCommand command)
        {
            Name = name;
            Command = command;
        }
        public string Name { get; set; }
        public DelegateCommand Command { get; set; }
    }
}