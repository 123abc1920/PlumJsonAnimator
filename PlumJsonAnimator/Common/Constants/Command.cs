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

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute(parameter);
    }
}


// 2^n0-2=компьютеров -- маски подсети n0 нулей в маске
// на 63 комп в сети 2.2.2.0 7 нолей маска /25 это значит адреса от 2.2.2.0 до 2.2.2.127. первый зарезервирован как адрес подсети, последний broadcast
// 30 2.2.2.128/27 2.2.2.159 broadcast подсеть всегда четный адрес, broadcast всегда нет
// 20 2.2.2.160/27 2.2.2.191
// 2 2.2.2.192/30 2.2.2.195
// в работе компьютер символизирует подсеть для упрощения