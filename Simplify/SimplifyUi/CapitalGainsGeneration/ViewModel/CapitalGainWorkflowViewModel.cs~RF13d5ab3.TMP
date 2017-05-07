using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Simplify.ExcelDataGateway;
using Simplify.Properties;
using SimplifyUi.BooksOfAccountGeneration.ViewModel;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
{
    public class CapitalGainWorkflowViewModel : INotifyPropertyChanged
    {
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

        public void GotoInformationStep(IList<string> mainInfo, ILogger logger)
        {
            WorkflowStepViewModel = new object();
        }

        public void GotoGatherInput()
        {

        }

        public CapitalGainWorkflowViewModel()
        {
            WorkflowStepViewModel = new CapitalGainsInputViewModel(GotoInformationStep);    
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
