using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Simplify.Properties;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.Common.ViewModel
{
    public  class WorkflowViewModel : INotifyPropertyChanged
    {
        protected Bag Bag = new Bag();

        Dictionary<int, Action> _nextStepActionDictionary = new Dictionary<int, Action>();

        protected void RegisterNextStep(int stepIndex, Action goToNextStepAction)
        {
            _nextStepActionDictionary.Add(stepIndex, goToNextStepAction);
        }

        public void GoToNextStep(int nextStepIndex)
        {
            if (_nextStepActionDictionary.ContainsKey(nextStepIndex))
            {
                _nextStepActionDictionary[nextStepIndex].Invoke();
            }
        }

        private object _workflowStepViewModel;

        public object WorkflowStepViewModel
        {
            get { return _workflowStepViewModel; }
            set
            {
                if (_workflowStepViewModel != value)
                {
                    _workflowStepViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}