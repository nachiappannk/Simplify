using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Prism.Commands;
using Simplify.ExcelDataGateway;
using Simplify.Properties;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.Common.ViewModel
{
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
                    ComputeAndUpdateSheetName();
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
                ComputeAndUpdateSheetName();
                if (!ExcelSheetNames.Contains(SelectedSheet))
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

        public void ComputeAndUpdateSheetName()
        {
            ExcelSheetInfoProvider sheetInfoProvider = new ExcelSheetInfoProvider(InputExcelFileName);
            ExcelSheetNames = sheetInfoProvider.GetSheetNames();
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