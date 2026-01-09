using System.Windows.Input;

namespace Mams_App.src.commands;

/// <summary>
/// Represents a command that can be bound to UI elements and executed in response to user interactions.
/// </summary>
/// <remarks>This class implements the <see cref="ICommand"/> interface, allowing it to be used in data binding
/// scenarios such as commanding in WPF or other XAML-based frameworks. The command's ability to execute is determined
/// by the provided <see cref="Func{TResult}"/> delegate, if specified, and its execution logic is defined by the <see
/// cref="Action{T}"/> delegate.</remarks>
public class RelayCommand : ICommand
{

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    private Action<object?> _m_execute { get; set; } // return void
    private readonly Func<object?, bool>? _m_can_execute; // return a boolean


    public RelayCommand(Action<object?> execute, Func<object?, bool>? can_execute = null)
    {
        _m_execute = execute;
        _m_can_execute = can_execute;
    }


    public bool CanExecute(object? parameter)
    {
        return _m_can_execute == null
            || _m_can_execute(parameter);
    }


    public void Execute(object? parameter)
    {
        _m_execute(parameter);
    }
}
