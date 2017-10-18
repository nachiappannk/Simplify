using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SimplifyUi.CapitalGainsGeneration.ViewModel;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.BooksOfAccountGeneration.ViewModel
{
    public class BooksOfAccountGenerationWorkflowViewModel : WorkflowViewModel, INotifyPropertyChanged
    {
        public override string Name  => "Books of Account";

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

        public void GotoInformationStep(List<string> mainInfo, Logger logger)
        {
            WorkflowStepViewModel = new DisplayInformationViewModel("Books of AccountGeneration Status", mainInfo,
                logger.GetLogMessages(), GotoGatherInput);
        }

        public void GotoGatherInput()
        {
            WorkflowStepViewModel = new BooksOfAccountInputViewModel(GotoInformationStep);
        }

        public BooksOfAccountGenerationWorkflowViewModel()
        {
            GotoGatherInput();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
