using System.ComponentModel;

namespace AnimModels
{
    public class Attachment : IBone, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
