using System;
using System.Collections.Generic;

namespace SimplifyUi.Common.ViewModel
{
    public class WorkFlowStepViewModel
    {
        public WorkFlowStepViewModel()
        {
            AdditionalCommands = new List<NamedCommand>();
        }

        public event Action StateChanged;
        public List<NamedCommand> AdditionalCommands { get; set; } 
        public bool CanGoToNext { get; set; }
        public bool CanGoToHome { get; set; }
        public bool CanGoToPrevious { get; set; }

        public string Name { get; set; }
        protected virtual void FireStateChanged()
        {
            StateChanged?.Invoke();
        }

        protected void AddCommand(NamedCommand command)
        {
            AdditionalCommands.Add(command);
        }

    }
}