using System.ComponentModel;
using System.Runtime.CompilerServices;
using Simplify.Properties;

namespace SimplifyUi.ViewModel
{
    public class ConsolidatedBooksGenerationWorkflowViewModel : INotifyPropertyChanged
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

        Bag _bag = new Bag();

        private void GoToNextStep(int nextStepIndex)
        {
            if (ReadJournal == nextStepIndex)
            {
                WorkflowStepViewModel = new ReadJournalViewModel(_bag, GoToNextStep);
            }
            else if (nextStepIndex == DisplayJournalReadMessages)
            {
                WorkflowStepViewModel = new DisplayMessagesViewModel(_bag, JournalReadMessagesKey, 
                    "Please check the journal read logs", ReadPreviousPeriodBalanceSheet, GoToNextStep);
            }
            else if (nextStepIndex == ReadPreviousPeriodBalanceSheet)
            {
                WorkflowStepViewModel = new ReadPreviousPeriodBalanceSheetViewModel(_bag, GoToNextStep);
            }
            else if (nextStepIndex == DisplayBalanceSheetReadMessages)
            {
                WorkflowStepViewModel = new DisplayMessagesViewModel(_bag, BalanceSheetReadMessagesKey,
                   "Please check the balance sheet read logs", GenerateBooksOfAccount, GoToNextStep);
            }else if (nextStepIndex == GenerateBooksOfAccount)
            {
                WorkflowStepViewModel = new BooksOfAccountGenerationStatusViewModel(_bag);
            }
        }

        private object workflowStepViewModel;

        public object WorkflowStepViewModel
        {
            get { return workflowStepViewModel; }
            set
            {
                if (workflowStepViewModel != value)
                {
                    workflowStepViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        private ReadJournalViewModel readJournalViewModel;

        public ConsolidatedBooksGenerationWorkflowViewModel()
        {
            GoToNextStep(ReadJournal);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
