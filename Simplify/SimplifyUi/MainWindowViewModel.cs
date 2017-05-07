using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplifyUi.BooksOfAccountGeneration.ViewModel;

namespace SimplifyUi
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            BooksOfAccountGenerationWorkflowViewModel = new BooksOfAccountGenerationWorkflowViewModel();
        }

        public BooksOfAccountGenerationWorkflowViewModel BooksOfAccountGenerationWorkflowViewModel { get; set; }
    }
}
