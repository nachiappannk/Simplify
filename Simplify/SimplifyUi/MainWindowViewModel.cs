using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplifyUi.BooksOfAccountGeneration.ViewModel;
using SimplifyUi.CapitalGainsGeneration.ViewModel;
using SimplifyUi.Common.ViewModel;

namespace SimplifyUi
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            BooksOfAccountGenerationWorkflowViewModel = new BooksOfAccountGenerationWorkflowViewModel();
            
            TradeStatementWorkFlow = new TradeStatementWorkFlow();
        }

        public BooksOfAccountGenerationWorkflowViewModel BooksOfAccountGenerationWorkflowViewModel { get; set; }
        
        public TradeStatementWorkFlow TradeStatementWorkFlow { get; set; }
    }
}
