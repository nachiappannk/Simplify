using System;
using Simplify.ExcelDataGateway;

namespace SimplifyUi.ViewModel
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
                BooksOfAccountReader booksOfAccountReader = new BooksOfAccountReader(InputExcelFileName, SelectedSheet);
                var logger = new Logger();
                var journal = booksOfAccountReader.GetJournalStatements(logger);
                _bag.AddObject(ConsolidatedBooksGenerationWorkflowViewModel.InputJournalKey, journal);
                _bag.AddObject(ConsolidatedBooksGenerationWorkflowViewModel.JournalReadMessagesKey, logger.GetLogMessages());
                _nextStepRequestAction.Invoke(ConsolidatedBooksGenerationWorkflowViewModel.DisplayJournalReadMessages);
            }
            catch (Exception e)
            {
                ComputeAndUpdateSheetName();
                ErrorMessage = e.Message;
            }
        }
    }
}