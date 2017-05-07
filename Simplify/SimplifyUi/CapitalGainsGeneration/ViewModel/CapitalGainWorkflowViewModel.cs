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

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
{
    public class CapitalGainWorkflowViewModel : WorkflowViewModel
    {

        public static readonly int ReadTradeLog = 1;
        public static readonly int DisplayMessage = 2;
        
        public static readonly string SquaredTradeKey= nameof(SquaredTradeKey);
        public static readonly string DisplayMessagesKey = nameof(DisplayMessagesKey);


        public CapitalGainWorkflowViewModel()
        {
            RegisterNextStep(ReadTradeLog, () =>
            {
                WorkflowStepViewModel = new ReadTradeLogViewModel(Bag, GoToNextStep);
            });
            GoToNextStep(ReadTradeLog);    
        }

    }
}
