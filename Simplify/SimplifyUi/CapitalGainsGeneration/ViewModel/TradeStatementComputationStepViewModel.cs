using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Prism.Commands;
using Simplify.Application;
using Simplify.ExcelDataGateway;
using Simplify.ExcelDataGateway.Trade;
using Simplify.Trade;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;
using SimplifyUi.Properties;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
{
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
            Name = "Error(s) in Processing";
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
                new ProcessedTradeStatementsContainer(tradeLogs, new List<EvaluationStatement>());

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
}