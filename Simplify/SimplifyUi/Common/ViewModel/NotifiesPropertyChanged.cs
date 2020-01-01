using System.ComponentModel;
using System.Runtime.CompilerServices;
using SimplifyUi.Annotations;

namespace SimplifyUi.Common.ViewModel
{
    public class NotifiesPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void FirePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}