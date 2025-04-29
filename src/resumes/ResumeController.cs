using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.crudOperations;
using Mams.src.navigations;
using Mams.src.productsShapes;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mams.src.resumes;

public class ResumeController : ABaseController {

    private readonly PageNavigationController _m_page_navigation;
    //private readonly ProductShapeModel _m_item_model;

    //public ICommand m_add_new_item_command { get; set; }
    //public ICommand m_modify_item_command { get; set; }
    //public ICommand m_delete_item_command { get; set; }




    public ResumeController(PageNavigationController page_navigation) {

        _m_page_navigation = page_navigation;
        //_m_item_model = new();

        //m_add_new_item_command = new RelayCommand(navigateToSavePage);
        //m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        //m_delete_item_command = new RelayCommand(deleteOrRestoreItem, isItemSelected);
    }

}