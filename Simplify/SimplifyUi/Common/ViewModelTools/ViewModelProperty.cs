using System.ComponentModel;
using System.Runtime.CompilerServices;
using SimplifyUi.Annotations;

namespace SimplifyUi.Common.ViewModelTools
{
    public class ViewModelProperty<T> : INotifyPropertyChanged
    {
        private T t;

        public T Property
        {
            get { return t; }
            set
            {
                if (t == null && (value == null)) return;
                if (value == null || !value.Equals(t))
                {
                    t = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ViewModelDoubleProperty : ViewModelProperty<double>
    {
    }

    public class ViewModelNullableDoubleProperty : ViewModelProperty<double?>
    {
    }

}