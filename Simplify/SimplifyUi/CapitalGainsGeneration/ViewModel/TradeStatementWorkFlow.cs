using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Simplify.Application;
using Simplify.ExcelDataGateway;
using Simplify.Facade;
using SimplifyUi.Annotations;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
{
    public class TradeStatementWorkFlow : WorkflowViewModel
    {
        public TradeStatementWorkFlow() : base("Trade Statements")
        {
            var inputStep = new TradeStatementInputStepViewModel();
            var statementComputingStep = new TradeStatementComputationStepViewModel();
            var resultStep = new TradeStatementResultStepViewModel();
            inputStep.InputChanged += (x) => statementComputingStep.Compute(x.FileName, x.SheetName);
            statementComputingStep.StatementComputed += (x) => resultStep.SetStatements(x);
            AddWorkFlowStep(inputStep);
            AddWorkFlowStep(statementComputingStep);
            AddWorkFlowStep(resultStep);
        }
    }

    public class TradeStatementInput
    {
        public string FileName { get; set; }
        public string SheetName { get; set; }
    }

    public class TradeStatementInputStepViewModel : WorkFlowStepViewModel
    {
        public event Action<TradeStatementInput> InputChanged; 
        public ExcelSheetSelectorViewModel TradeLogExcelSheetSelectorViewModel { get; set; }

        public TradeStatementInputStepViewModel()
        {
            Name = "Inputs";
            TradeLogExcelSheetSelectorViewModel = new ExcelSheetSelectorViewModel();
            TradeLogExcelSheetSelectorViewModel.Title = "Please provide trade log";
            TradeLogExcelSheetSelectorViewModel.ValidityChanged += () =>
            {
                var input = new TradeStatementInput
                {
                    FileName = TradeLogExcelSheetSelectorViewModel.InputFileName,
                    SheetName = TradeLogExcelSheetSelectorViewModel.SelectedSheet
                };

                InputChanged?.Invoke(input);


                if (string.IsNullOrWhiteSpace(input.FileName)) CanGoToNext = false;
                else if (string.IsNullOrWhiteSpace(input.SheetName)) CanGoToNext = false;
                else CanGoToNext = true;

                FireStateChanged();
            };
        }
    }

    public class TradeStatementComputationStepViewModel : WorkFlowStepViewModel, INotifyPropertyChanged
    {
        public event Action<ProcessedTradeStatementsContainer> StatementComputed;
        private string FileName { get; set; }
        private string SheetName { get; set; }

        private List<Message> _messages;

        public List<Message> Messages
        {
            get { return _messages; }
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    FirePropertyChanged();
                }
            }
        }

        private NamedCommand RefreshCommand { get; set; }

        public TradeStatementComputationStepViewModel()
        {
            Name = "Statement Computation";
            RefreshCommand = new NamedCommand("Refresh", new DelegateCommand(Compute));
            AddCommand(RefreshCommand);
        }
        public void Compute(string fileName, string sheetName)
        {
            FileName = fileName;
            SheetName = sheetName;
            Compute();
        }

        private void Compute()
        {
            var logger = new Logger();

            ProcessedTradeStatementsExcelGateway gateway = new ProcessedTradeStatementsExcelGateway();
            var tradeLogs = gateway.ReadTradeLog(logger, FileName, SheetName);

            ProcessedTradeStatementsContainer processedTradeStatementsContainer =
                new ProcessedTradeStatementsContainer(tradeLogs);

            Messages = logger.GetLogMessages();

            CanGoToHome = true;
            CanGoToNext = true;
            CanGoToPrevious = true;

            StatementComputed?.Invoke(processedTradeStatementsContainer);

            FireStateChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void FirePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TradeStatementResultStepViewModel : WorkFlowStepViewModel
    {
        public void SetStatements(ProcessedTradeStatementsContainer statementsContainer)
        {

        }
    }
}
