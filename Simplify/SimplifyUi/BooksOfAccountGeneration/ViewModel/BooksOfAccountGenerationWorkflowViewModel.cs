using SimplifyUi.Common.ViewModel;

namespace SimplifyUi.BooksOfAccountGeneration.ViewModel
{
    public class BooksOfAccountGenerationWorkflowViewModel : WorkflowViewModel
    {
        public static readonly int ReadJournal = 1;
        public static readonly int DisplayJournalReadMessages = 2;
        public static readonly int ReadPreviousPeriodBalanceSheet = 3;
        public static readonly int DisplayBalanceSheetReadMessages = 4;
        public static readonly int GenerateBooksOfAccount = 5;

        public static readonly string InputJournalKey = nameof(InputJournalKey);
        public static readonly string JournalReadMessagesKey = nameof(JournalReadMessagesKey);
        public static readonly string InputBalanceSheetKey = nameof(InputBalanceSheetKey);
        public static readonly string BalanceSheetReadMessagesKey = nameof(BalanceSheetReadMessagesKey);


        public BooksOfAccountGenerationWorkflowViewModel()
        {
            RegisterNextStep(ReadJournal, () =>
            {
                WorkflowStepViewModel = new ReadJournalViewModel(Bag, GoToNextStep);
            });
            RegisterNextStep(DisplayJournalReadMessages, () =>
            {
                WorkflowStepViewModel = new DisplayMessagesViewModel(Bag, JournalReadMessagesKey,
                    "Please check the journal read logs", ReadPreviousPeriodBalanceSheet, GoToNextStep);
            });
            RegisterNextStep(ReadPreviousPeriodBalanceSheet, () =>
            {
                WorkflowStepViewModel = new ReadPreviousPeriodBalanceSheetViewModel(Bag, GoToNextStep);
            });
            RegisterNextStep(DisplayBalanceSheetReadMessages, () =>
            {
                WorkflowStepViewModel = new DisplayMessagesViewModel(Bag, BalanceSheetReadMessagesKey,
                    "Please check the balance sheet read logs", GenerateBooksOfAccount, GoToNextStep);
            });
            RegisterNextStep(GenerateBooksOfAccount, () =>
            {
                WorkflowStepViewModel = new BooksOfAccountGenerationStatusViewModel(Bag);
            });
            GoToNextStep(ReadJournal);
        }
    }
}
