using System;
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

    public class WorkFlowStepViewModel
    {
        public event Action StateChanged;
        public virtual bool CanGoToNext { get; set; }
        public virtual bool CanGoToHome { get; set; }
        public virtual bool CanGoToPrevious { get; set; }

        public string Name { get; set; }
        protected virtual void FireStateChanged()
        {
            StateChanged?.Invoke();
        }
    }
}