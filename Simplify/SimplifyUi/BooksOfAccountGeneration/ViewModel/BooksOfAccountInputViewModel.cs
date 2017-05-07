using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Simplify.Facade;
using SimplifyUi.CapitalGainsGeneration.ViewModel;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.BooksOfAccountGeneration.ViewModel
{
    public class BooksOfAccountInputViewModel
    {
        private Action<List<string>, Logger> _gotoInformationStep;

        public BooksOfAccountInputViewModel(Action<List<string>, Logger> gotoInformationStep)
        {
            _gotoInformationStep = gotoInformationStep;
            JournalSelectorViewModel = new ExcelSheetSelectorViewModel();
            JournalSelectorViewModel.Title = "Please provide the journal";
            PreviousBalanceSheetSelectorViewModel = new ExcelSheetSelectorViewModel();
            PreviousBalanceSheetSelectorViewModel.Title = "Please provide the previous period balance sheet";
            GenerateCommand = new DelegateCommand(Generate,CanGenerate);
            JournalSelectorViewModel.ValidityChanged += RaiseCanExecuteChanged;
            PreviousBalanceSheetSelectorViewModel.ValidityChanged += RaiseCanExecuteChanged;
        }

        private void RaiseCanExecuteChanged()
        {
            GenerateCommand.RaiseCanExecuteChanged();
        }

        public DelegateCommand GenerateCommand { get; set; }
        public ExcelSheetSelectorViewModel JournalSelectorViewModel { get; set; }
        public ExcelSheetSelectorViewModel PreviousBalanceSheetSelectorViewModel { get; set; }

        private DateTime? _accountingPeriodStartDate;
        public DateTime? AccountingPeriodStartDate
        {
            get { return _accountingPeriodStartDate;}
            set
            {
                if (_accountingPeriodStartDate != value)
                {
                    _accountingPeriodStartDate = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        private DateTime? _accountingPeriodEndDate;
        public DateTime? AccountingPeriodEndDate
        {
            get { return _accountingPeriodEndDate; }
            set
            {
                if (_accountingPeriodEndDate != value)
                {
                    _accountingPeriodEndDate = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        bool CanGenerate()
        {
            if(!JournalSelectorViewModel.IsValid)return false;
            if (!PreviousBalanceSheetSelectorViewModel.IsValid) return false;
            if (AccountingPeriodEndDate == null) return false;
            if (AccountingPeriodStartDate == null) return false;
            return true;
        }

        void Generate()
        {


            if (!AccountingPeriodEndDate.HasValue) return;
            if (!AccountingPeriodStartDate.HasValue) return;

            List<string> mainMessage = new List<string>();
            var logger = new Logger();
            var fullPath = OutputNameComputer.ComputeOutputFile("BalanceSheet", ".xlsx");
            try
            {
                var facade = new BooksOfAccountStatementGenerationFacade();

                facade.GenerateStatements(JournalSelectorViewModel.InputFileName,
                    JournalSelectorViewModel.SelectedSheet,
                    PreviousBalanceSheetSelectorViewModel.InputFileName,
                    PreviousBalanceSheetSelectorViewModel.SelectedSheet,
                    fullPath,AccountingPeriodStartDate.Value,
                    AccountingPeriodEndDate.Value, logger);
                mainMessage.Add("Successfully Generated Capital Gains File");
                mainMessage.Add("Output :" + fullPath);
            }
            catch (Exception e)
            {
                var errorFile = OutputNameComputer.ComputeOutputFile("SimplifyBalanceSheetError", ".txt");
                File.WriteAllText(errorFile,
                    e.StackTrace);
                mainMessage.Add("Fully or Partially Failed to generate");
                mainMessage.Add("Error :" + errorFile);
                mainMessage.Add("Output (may be):" + fullPath);
            }
            _gotoInformationStep.Invoke(mainMessage, logger);

        }
    }
}
