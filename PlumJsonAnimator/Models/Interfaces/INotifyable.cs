using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PlumJsonAnimator.Models.Interfaces
{
    public abstract class INotifyable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
