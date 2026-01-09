using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mams_App.src.controllers;


public abstract class ABaseController : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void onPropertyChanged([CallerMemberName] string? property_name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
    }
}
