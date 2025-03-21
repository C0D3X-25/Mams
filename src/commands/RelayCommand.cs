using System.Windows.Input;

namespace Mams.src.commands;

public class RelayCommand : ICommand {

    public event EventHandler? CanExecuteChanged {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    private Action<object?> _m_execute { get; set; } // return void
    private Func<object?, bool> _m_can_execute; // return a boolean


    public RelayCommand(Action<object?> execute, Func<object?, bool> can_execute = null) {
        _m_execute = execute;
        _m_can_execute = can_execute;
    }

    public bool CanExecute(object? parameter) {
        return _m_can_execute == null || _m_can_execute(parameter);
    }

    public void Execute(object? parameter) {
        _m_execute(parameter);
    }
}
