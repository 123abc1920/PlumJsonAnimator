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
        public bool isBone = true;

        public ObservableCollection<Node> SubNodes { get; set; }
        public Node? parent = null;

        public Node(string title, int _id, Node? _parent)
        {
            _title = title;
            SubNodes = new ObservableCollection<Node>();
            id = _id;
            parent = _parent;
        }

        public Node(bool _isBone, string title, int _id, Node? _parent)
        {
            _title = title;
            SubNodes = new ObservableCollection<Node>();
            id = _id;
            parent = _parent;
            isBone = _isBone;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
