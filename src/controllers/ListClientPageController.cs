using Mams.src.items;
using Mams.src.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.controllers;

public class ListClientPageController {

    public ObservableCollection<ClientItem> m_clients { get; set; }

    public ListClientPageController() {
        ClientModel client_model = new();
        m_clients = client_model.getTable();
    }

}
