using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Prism.Commands;
using Simplify.Annotations;
using Simplify.ExcelDataGateway;

namespace Simplify.ViewModel
{
    public class ReadJournalViewModel : ReadExcelViewModel
    {
        public ReadJournalViewModel(Bag bag, Action<int> nextStepRequestAction) : base(bag, nextStepRequestAction)
        {
            ReadFileName = "Journal Excel File";
            ReadSheetName = "Journal Work Sheet";
            Title = "Please provide the journal";
        }

        protected override void ExecuteNextStep()
        {
            try
            {
                ExcelReader excelReader = new ExcelReader();
                List<string> readMessages;
                ExcelSheetNames = excelReader.GetSheetNames(InputExcelFileName);
                var journal = excelReader.GetJournalStatements(InputExcelFileName, SelectedSheet,
                    out readMessages);
                _bag.AddObject(ConsolidatedBooksGenerationWorkflowViewModel.InputJournalKey, journal);
                _bag.AddObject(ConsolidatedBooksGenerationWorkflowViewModel.JournalReadMessagesKey, readMessages);
                _nextStepRequestAction.Invoke(ConsolidatedBooksGenerationWorkflowViewModel.DisplayJournalReadMessages);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }

    public class ReadPreviousPeriodBalanceSheetViewModel : ReadExcelViewModel
    {
        public ReadPreviousPeriodBalanceSheetViewModel(Bag bag, Action<int> nextStepRequestAction) 
            : base(bag, nextStepRequestAction)
        {
            ReadFileName = "Balance Sheet Excel File";
            ReadSheetName = "Previous year Balance Sheet work sheet";
            Title = "Please provide the previous year balance sheet";
        }

        protected override void ExecuteNextStep()
        {
            try
            {
                ExcelReader excelReader = new ExcelReader();
                List<string> readMessages;
                ExcelSheetNames = excelReader.GetSheetNames(InputExcelFileName);
                var balanceSheet = excelReader.GetBalanceSheet(InputExcelFileName, SelectedSheet, out readMessages);
                _bag.AddObject(ConsolidatedBooksGenerationWorkflowViewModel.InputBalanceSheetKey, balanceSheet);
                _bag.AddObject(ConsolidatedBooksGenerationWorkflowViewModel.BalanceSheetReadMessagesKey, readMessages);
                _nextStepRequestAction.Invoke(ConsolidatedBooksGenerationWorkflowViewModel.DisplayBalanceSheetReadMessages);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }


    public abstract class ReadExcelViewModel : INotifyPropertyChanged
    {
        protected readonly Bag _bag;
        protected readonly Action<int> _nextStepRequestAction;

        public string ReadFileName { get; set; }
        public string ReadSheetName { get; set; }
        public string Title { get; set; }

        public ICommand NextStepCommand
        {
            get { return _nextStepDelegateCommand; }
            set { }
        }

        private readonly DelegateCommand _nextStepDelegateCommand;

        private string inputExcelFileName = "";
        public string InputExcelFileName
        {
            get { return inputExcelFileName; }
            set
            {
                if (inputExcelFileName != value)
                {
                    inputExcelFileName = value;
                    ReadInputExcel(inputExcelFileName);
                }

            }
        }

        private string _selectedSheet = String.Empty;
        public string SelectedSheet
        {
            get { return _selectedSheet; }
            set
            {
                if (_selectedSheet != value)
                {
                    _selectedSheet = value;
                    _nextStepDelegateCommand.RaiseCanExecuteChanged();
                    OnPropertyChanged();
                }
            }
        }

        private void ReadInputExcel(string inputExcel)
        {
            try
            {
                ExcelReader excelReader = new ExcelReader();
                SelectedSheet = String.Empty;
                ExcelSheetNames = excelReader.GetSheetNames(inputExcel);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

        }

        private IList<string> excelSheetNames = new List<string>();
        public IList<string> ExcelSheetNames
        {
            get { return excelSheetNames; }
            set
            {
                if (!Equals(excelSheetNames, value))
                {
                    excelSheetNames = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public bool IsSheetSelectionEnabled { get; set; }


        public ReadExcelViewModel(Bag bag, Action<int> nextStepRequestAction)
        {
            _bag = bag;
            _nextStepRequestAction = nextStepRequestAction;
            _nextStepDelegateCommand = new DelegateCommand(ExecuteNextStep, CanNextStepExecute);
        }

        private string errorMessage = String.Empty;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool CanNextStepExecute()
        {
            if (string.IsNullOrEmpty(inputExcelFileName)) return false;
            try
            {
                ExcelReader excelReader = new ExcelReader();
                if (!excelReader.GetSheetNames(inputExcelFileName).Contains(SelectedSheet))
                {
                    ErrorMessage = "The selected sheet "+ SelectedSheet + " was removed";
                    return false;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
            return true;
        }

        protected abstract void ExecuteNextStep();
        

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}