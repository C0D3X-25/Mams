using System.Windows.Input;

namespace Mams.commands;

public class RelayCommand : ICommand {

    public event EventHandler? CanExecuteChanged {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
    private Action<object?> _m_execute { get; set; } // return void

    private Func<object?, bool> _m_can_execute; // return a boolean

    public RelayCommand(Action<object?> execute_method, Func<object?, bool> can_execute_method = null) {
        _m_execute = execute_method;
        _m_can_execute = can_execute_method;
    }

    public bool CanExecute(object? parameter) {
        return _m_can_execute == null || _m_can_execute(parameter);
    }

    public void Execute(object? parameter) {
        _m_execute(parameter);
    }
}
