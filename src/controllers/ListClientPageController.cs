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

    private readonly PageNavigationController _m_page_navigation;
    public ObservableCollection<EntityItem> m_clients { get; set; }
    public ICommand m_add_new_client_command { get; set; }


    public ListClientPageController(PageNavigationController page_navigation) {
        _m_page_navigation = page_navigation;
        EntityModel client_model = new();
        m_clients = client_model.getTable();
        m_add_new_client_command = new RelayCommand(navigateToSaveClient);
    }


    private void navigateToSaveClient(object? obj) {
        _m_page_navigation.navigateTo(new SaveClientPage());
    }
}
