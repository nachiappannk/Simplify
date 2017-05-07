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
        protected readonly Bag Bag;
        protected readonly Action<int> NextStepRequestAction;

        public string ReadFileNameDisplayText { get; set; }
        public string ReadSheetNameDisplayText { get; set; }
        public string TitleDisplayText { get; set; }

        public ICommand NextStepCommand
        {
            get { return _nextStepDelegateCommand; }
            set { }
        }

        private readonly DelegateCommand _nextStepDelegateCommand;

        private string _inputExcelFileName = "";
        public string InputExcelFileName
        {
            get { return _inputExcelFileName; }
            set
            {
                if (_inputExcelFileName != value)
                {
                    _inputExcelFileName = value;
                    ComputeSheetName();
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
                    OnPropertyChanged();
                    RecomputeNextCommandState();
                }
            }
        }

        private void RecomputeNextCommandState()
        {
            _nextStepDelegateCommand.RaiseCanExecuteChanged();
        }


        private IList<string> _excelSheetNames = new List<string>();
        public IList<string> ExcelSheetNames
        {
            get { return _excelSheetNames; }
            set
            {
                if (!Equals(_excelSheetNames, value))
                {
                    _excelSheetNames = value;
                    OnPropertyChanged();
                }
            }
        }
        

        protected ReadExcelViewModel(Bag bag, Action<int> nextStepRequestAction)
        {
            Bag = bag;
            NextStepRequestAction = nextStepRequestAction;
            _nextStepDelegateCommand = new DelegateCommand(ExecuteNextStep, CanNextStepExecute);
        }

        private string _errorMessage = String.Empty;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool CanNextStepExecute()
        {
            if (string.IsNullOrEmpty(_inputExcelFileName)) return false;
            try
            {
                ComputeSheetName();
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

        public void ComputeSheetName()
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