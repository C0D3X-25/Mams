using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using Mams.src.productsCategories;
using Mams.src.productsShapes;
using Mams.src.productsTypes;
using Mams.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.products;

public class SaveProductController : ABaseController {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly ProductModel _m_product_model = new();
    private readonly ProductTypeModel _m_product_type_model = new();
    private readonly ProductCategoryModel _m_product_category_model = new();
    private readonly ProductShapeModel _m_product_shape_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductItem _m_product;
    public ProductItem m_product {
        get => _m_product;
        set {
            _m_product = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<ProductTypeItem> _m_list_product_type;
    public ObservableCollection<ProductTypeItem> m_list_product_type {
        get { return _m_list_product_type; }
        set {
            _m_list_product_type = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<ProductCategoryItem> _m_list_product_category;
    public ObservableCollection<ProductCategoryItem> m_list_product_category {
        get { return _m_list_product_category; }
        set {
            _m_list_product_category = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<ProductShapeItem> _m_list_product_shape;
    public ObservableCollection<ProductShapeItem> m_list_product_shape {
        get { return _m_list_product_shape; }
        set {
            _m_list_product_shape = value;
            onPropertyChanged();
        }
    }


    private ProductTypeItem? _m_selected_product_type;
    public ProductTypeItem? m_selected_product_type {
        get { return _m_selected_product_type; }
        set {
            _m_selected_product_type = value;
            onPropertyChanged();
        }
    }


    private ProductCategoryItem? _m_selected_product_category;
    public ProductCategoryItem? m_selected_product_category {
        get { return _m_selected_product_category; }
        set {
            _m_selected_product_category = value;
            onPropertyChanged();
        }
    }


    private ProductShapeItem? _m_selected_product_shape;
    public ProductShapeItem? m_selected_product_shape {
        get { return _m_selected_product_shape; }
        set {
            _m_selected_product_shape = value;
            onPropertyChanged();
        }
    }


    public SaveProductController(int id_to_load = 0) {

        _m_list_product_category = _m_product_category_model.getTable();
        _m_list_product_type = _m_product_type_model.getTable();
        _m_list_product_shape = _m_product_shape_model.getTable();

        _m_product = new();

        if (id_to_load != 0) {

            _m_product = _m_product_model.getItemByID(id_to_load.ToString()) ?? new ProductItem();

            // Find the matching product in the list and set it as selected or create a new one
            _m_selected_product_category = _m_list_product_category.FirstOrDefault(b => 
                b.product_category_id == _m_product.fk_product_category_id) ?? new ProductCategoryItem();
            _m_selected_product_type = _m_list_product_type.FirstOrDefault(b => 
                b.product_type_id == _m_product.fk_product_type_id) ?? new ProductTypeItem();
            _m_selected_product_shape = _m_list_product_shape.FirstOrDefault(b => 
                b.product_shape_id == _m_product.fk_product_shape_id) ?? new ProductShapeItem();
        }
        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product.product_name)
            && m_selected_product_category != null
            && m_selected_product_type != null
            && _m_product.product_weight >= 0;
    }


    private void saveProduct(object? obj) {
        if (_m_selected_product_type != null) {
            m_product.fk_product_type_id = _m_selected_product_type.product_type_id;
        }
        if (_m_selected_product_category != null) {
            m_product.fk_product_category_id = _m_selected_product_category.product_category_id;
        }
        if (_m_selected_product_shape != null) {
            m_product.fk_product_shape_id = _m_selected_product_shape.product_shape_id;
        }

        if (_m_product_model.saveItem(m_product) > 0) {
            SPageNavigationController.navigateBack();
        }
        else { MessageBox.Show("Un produit avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); }
    }


    private void abortProduct(object? obj) {
        SPageNavigationController.navigateBack();
    }
}
