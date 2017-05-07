using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplifyUi.BooksOfAccountGeneration.ViewModel;
using SimplifyUi.CapitalGainsGeneration.ViewModel;

namespace SimplifyUi
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            BooksOfAccountGenerationWorkflowViewModel = new BooksOfAccountGenerationWorkflowViewModel();
            CapitalGainWorkflowViewModel = new CapitalGainWorkflowViewModel();
        }

        public BooksOfAccountGenerationWorkflowViewModel BooksOfAccountGenerationWorkflowViewModel { get; set; }

        public CapitalGainWorkflowViewModel CapitalGainWorkflowViewModel { get; set; }
    }
}
