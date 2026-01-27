using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mams_App.src.controllers;


/// <summary>
/// Abstract base controller class that implements INotifyPropertyChanged for WPF data binding support.
/// </summary>
public abstract class ABaseController : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event to notify the UI of property value changes.
    /// </summary>
    /// <param name="property_name">The name of the property that changed. Automatically populated by the compiler.</param>
    protected void onPropertyChanged([CallerMemberName] string? property_name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
    }
}
