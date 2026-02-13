using System;
using System.ComponentModel;

namespace JsonValidator
{
    public class JsonError : INotifyPropertyChanged
    {
        private string _errorText = "";

        public string ErrorText
        {
            get => _errorText;
            set
            {
                _errorText = value;
                if (value == "JSON is valid")
                {
                    this.isOk = true;
                }
                else
                {
                    this.isOk = false;
                }
                OnPropertyChanged(nameof(ErrorText));
            }
        }

        public bool isOk = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
