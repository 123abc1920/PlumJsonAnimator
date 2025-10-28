using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TreeModel
{
    public class Node : INotifyPropertyChanged
    {
        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public int id = 0;

        public ObservableCollection<Node> SubNodes { get; set; }

        public Node(string title, int _id)
        {
            _title = title;
            SubNodes = new ObservableCollection<Node>();
            id = _id;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
