using System.Windows.Input;

namespace Mams.commands;

public class RelayCommand : ICommand {

    public event EventHandler? CanExecuteChanged;
    private Action<object?> _m_execute { get; set; } // return void
    private Predicate<object?> _m_can_execute { get; set; } // return a boolean

    public RelayCommand(Action<object?> execute_method, Predicate<object?> can_execute_method) {
        _m_execute = execute_method;
        _m_can_execute = can_execute_method;
    }

    public bool CanExecute(object? parameter) {
        return _m_can_execute(parameter);
    }

    public void Execute(object? parameter) {
        _m_execute(parameter);
    }
}
