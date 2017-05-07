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
            ReadFileNameDisplayText = "Journal Excel File";
            ReadSheetNameDisplayText = "Journal Work Sheet";
            TitleDisplayText = "Please provide the journal";
        }

        protected override void ExecuteNextStep()
        {
            try
            {
                JournalGateway journalGateway = new JournalGateway(InputExcelFileName);
                var logger = new Logger();
                var journal = journalGateway.GetJournalStatements(logger, SelectedSheet);
                Bag.AddObject(BooksOfAccountGenerationWorkflowViewModel.InputJournalKey, journal);
                Bag.AddObject(BooksOfAccountGenerationWorkflowViewModel.JournalReadMessagesKey, logger.GetLogMessages());
                NextStepRequestAction.Invoke(BooksOfAccountGenerationWorkflowViewModel.DisplayJournalReadMessages);
            }
            catch (Exception e)
            {
                ComputeSheetName();
                ErrorMessage = e.Message;
            }
        }
    }
}