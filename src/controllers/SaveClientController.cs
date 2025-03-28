using Mams.src.commands;
using Mams.src.items;
using Mams.src.views.pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.controllers;

//public class SaveClientController : INotifyPropertyChanged {

//    private readonly PageNavigationController _m_page_navigation;

//    public event PropertyChangedEventHandler? PropertyChanged;


//    private EntityItem _m_client;
//    public EntityItem m_client {
//        get { return _m_client; }
//        set { 
//            _m_client = value;
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("m_client"));
//        }
//    }


//    public ICommand m_save_command { get; set; }
//    public ICommand m_abort_command { get; set; }


//    public SaveClientController(PageNavigationController page_navigation) {
//        _m_page_navigation = page_navigation;
//        _m_client = new EntityItem();
//        //_m_client.entity_name = "New Client";
//        m_save_command = new RelayCommand(saveClient, canSaveClient);
//        m_abort_command = new RelayCommand(abortClient);
//    }




//    private bool canSaveClient(object? obj) {
//        return !string.IsNullOrEmpty(_m_client.entity_name);
//    }

public class SaveClientController : INotifyPropertyChanged {
    private readonly PageNavigationController _m_page_navigation;
    private EntityItem _client;
    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public EntityItem m_client {
        get => _client;
        set {
            _client = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(m_client)));
        }
    }

    public SaveClientController(PageNavigationController page_navigation) {
        _m_page_navigation = page_navigation;
        _client = new EntityItem();
        m_client.PropertyChanged += Client_PropertyChanged; // Subscribe to client property changes
        m_save_command = new RelayCommand(saveClient, canSaveClient);
        m_abort_command = new RelayCommand(abortClient);
    }

    private void Client_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        // Refresh the command's CanExecute state
        (m_save_command as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private bool canSaveClient(object? obj) {
        return !string.IsNullOrEmpty(m_client.entity_name);
    }
    private void saveClient(object? obj) {

        MessageBox.Show("Client saved");
    }


    private void abortClient(object? obj) {
        _m_page_navigation.navigateTo(new ListClientPage());
    }
}
