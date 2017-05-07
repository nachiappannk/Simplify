using System;
using Simplify.ExcelDataGateway;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.BooksOfAccountGeneration.ViewModel
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
                JournalGateway journalGateway = new JournalGateway(InputExcelFileName);
                var logger = new Logger();
                var journal = journalGateway.GetJournalStatements(logger, SelectedSheet);
                _bag.AddObject(BooksOfAccountGenerationWorkflowViewModel.InputJournalKey, journal);
                _bag.AddObject(BooksOfAccountGenerationWorkflowViewModel.JournalReadMessagesKey, logger.GetLogMessages());
                _nextStepRequestAction.Invoke(BooksOfAccountGenerationWorkflowViewModel.DisplayJournalReadMessages);
            }
            catch (Exception e)
            {
                ComputeAndUpdateSheetName();
                ErrorMessage = e.Message;
            }
        }
    }
}