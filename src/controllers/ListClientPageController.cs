using Mams.src.commands;
using Mams.src.items;
using Mams.src.models;
using Mams.src.views.pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mams.src.controllers;

public class ListClientPageController {

    public ObservableCollection<ClientItem> m_clients { get; set; }

    public ICommand m_add_new_client_command { get; set; }

    public ListClientPageController() {
        ClientModel client_model = new();
        m_clients = client_model.getTable();
        m_add_new_client_command = new RelayCommand(navigateToSaveClient);
    }


    private void navigateToSaveClient(object? obj) {
        //_m_page_navigation.navigateTo(new SaveFeePage());
    }
}
