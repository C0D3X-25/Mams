using Mams.src.controllers;
using Mams.src.products;
using System.Collections.ObjectModel;

namespace Mams.src.resumes;

public class ResumeController : ABaseController {

    //private readonly ProductShapeModel _m_item_model;


    private ObservableCollection<ProductItem>? _m_list_tables;
    public ObservableCollection<ProductItem>? m_list_tables {
        get { return _m_list_tables; }
        set {
            _m_list_tables = value;
            onPropertyChanged();
        }
    }

    private ProductItem? _m_selected_table;
    public ProductItem? m_selected_table {
        get { return _m_selected_table; }
        set {
            _m_selected_table = value;
            onPropertyChanged();
        }
    }


    public ResumeController() {

        //_m_item_model = new();

    }
}