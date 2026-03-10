using System;
using System.Windows.Input;

namespace Command
{
    public class Command : ICommand
    {
        private readonly Action<object> _execute;

        public Command(Action<object> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        event EventHandler? ICommand.CanExecuteChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute(parameter);
    }
}
