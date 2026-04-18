using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PlumJsonAnimator.Models.Interfaces
{
    /// <summary>
    /// Interface for all observable objects
    /// </summary>
    public abstract class INotifyable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
