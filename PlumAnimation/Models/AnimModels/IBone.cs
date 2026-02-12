using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace AnimModels
{
    public class IBone : INotifyPropertyChanged
    {
        private string _name = "";
        public virtual string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual double x { get; set; }
        public virtual double y { get; set; }
        public virtual double a { get; set; }
        public int id;
        public bool isBone;

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void move(double x, double y) { }

        public virtual void scale(double x, double y) { }

        public virtual void rotate(double a) { }

        public virtual IEnumerable<IBone> CombinedChildren
        {
            get { yield break; }
        }
    }
}
