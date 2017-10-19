using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Prism.Commands;
using SimplifyUi.Annotations;

namespace SimplifyUi.Common.ViewModel
{
    public class WorkflowViewModel : INotifyPropertyChanged
    {
        private readonly List<WorkFlowStepViewModel> _viewModels;
        private int _selectedStepIndex = 0;
        public string Name { get; set; }

        private List<NamedCommand> _commands;

        public List<NamedCommand> Commands
        {
            get { return _commands; }
            set {
                if (_commands == value) return;
                _commands = value;
                FirePropertyChanged();
            }
        }
        public DelegateCommand GoToNextCommand { get; set; }

        public DelegateCommand GoToPreviousCommand { get; set; }

        public DelegateCommand GoToHomeCommand { get; set; }

        private WorkFlowStepViewModel _currentWorkflowStep;

        public WorkFlowStepViewModel CurrentWorkflowStep
        {
            get { return _currentWorkflowStep; }
            set {
                if (_currentWorkflowStep != value)
                {
                    _currentWorkflowStep = value;
                    FirePropertyChanged();
                }
            }
        }

        public WorkflowViewModel(string workflowName)
        {
            _viewModels = new List<WorkFlowStepViewModel>();
            Name = workflowName;
            GoToNextCommand = new DelegateCommand(GoToNext, CanGotoNext);
            GoToPreviousCommand = new DelegateCommand(GoToPrevious, CanGoToPrevious);
            GoToHomeCommand = new DelegateCommand(GoToHome, CanGoToHome);
        }

        protected void AddWorkFlowStep(WorkFlowStepViewModel stepViewModel)
        {
            _viewModels.Add(stepViewModel);
            SelectWorkflowStep(0);

        }

        private void SelectWorkflowStep(int index)
        {
            _selectedStepIndex = index;
            if(CurrentWorkflowStep != null) CurrentWorkflowStep.StateChanged -= RaiseAllCanExecutes;
            CurrentWorkflowStep = _viewModels.ElementAt(_selectedStepIndex);
            CurrentWorkflowStep.StateChanged += RaiseAllCanExecutes;
            var commands = new List<NamedCommand>
            {
                new NamedCommand("Next", GoToNextCommand),
                new NamedCommand("Previous", GoToPreviousCommand),
                new NamedCommand("Home", GoToHomeCommand)
            };
            commands.AddRange(CurrentWorkflowStep.AdditionalCommands);
            Commands = commands;
            RaiseAllCanExecutes();
        }

        private void RaiseAllCanExecutes()
        {
            GoToNextCommand.RaiseCanExecuteChanged();
            GoToPreviousCommand.RaiseCanExecuteChanged();
            GoToHomeCommand.RaiseCanExecuteChanged();
        }

        private bool CanGotoNext()
        {
            if (CurrentWorkflowStep == null) return false;
            return CurrentWorkflowStep.CanGoToNext;
        }

        private void GoToNext()
        {
            SelectWorkflowStep(_selectedStepIndex + 1);
        }
        
        private bool CanGoToPrevious()
        {
            if (CurrentWorkflowStep == null) return false;
            return CurrentWorkflowStep.CanGoToPrevious;
        }

        private void GoToPrevious()
        {
            SelectWorkflowStep(_selectedStepIndex - 1);
        }
        

        private bool CanGoToHome()
        {
            if (CurrentWorkflowStep == null) return false;
            return CurrentWorkflowStep.CanGoToHome;
        }


        private void GoToHome()
        {
            SelectWorkflowStep(0);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void FirePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}