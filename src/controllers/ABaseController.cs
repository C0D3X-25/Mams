using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mams.src.controllers;


public abstract class ABaseController : INotifyPropertyChanged {


    public event PropertyChangedEventHandler? PropertyChanged;

    protected void onPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
