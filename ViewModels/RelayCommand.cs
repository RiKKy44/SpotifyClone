using System;
using System.Windows.Input;

namespace GLab6.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        // Konstruktor dla komend bezparametrowych
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            _execute = _ => execute();
            _canExecute = canExecute != null ? _ => canExecute() : null;
        }

        // Konstruktor dla komend z parametrem (np. usunięcie konkretnej piosenki)
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Automatyczne odświeżanie stanu kontrolki (np. aktywacja/dezaktywacja przycisku)
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        // Ręczne wymuszenie odświeżenia stanu komendy
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}