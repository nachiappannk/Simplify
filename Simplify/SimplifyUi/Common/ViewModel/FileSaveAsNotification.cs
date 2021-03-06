using Prism.Interactivity.InteractionRequest;

namespace SimplifyUi.Common.ViewModel
{
    public class FileSaveAsNotification : INotification
    {
        public string Title { get; set; }
        public object Content { get; set; }
        public string DefaultFileName { get; set; }
        public string OutputFileName { get; set; }
    }
}