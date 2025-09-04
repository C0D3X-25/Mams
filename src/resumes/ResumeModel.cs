using Mams.src.beehives;
using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.fees;
using Mams.src.helpers;
using Mams.src.products;
using Mams.src.productsCategories;
using Mams.src.productsLots;
using Mams.src.productsShapes;
using Mams.src.productsTypes;
using Mams.src.profits;
using Mams.src.receipts;
using System.Collections.ObjectModel;

namespace Mams.src.resumes;

// This class is mainely used to manage the filter of the page Resume,
// that why this class doesn't inherit from ABaseModel
public class ResumeModel {

    private readonly ReceiptProfitDetailedModel _m_profit_model = new();
    private readonly ReceiptFeeDetailedModel _m_fee_model = new();
    private readonly ReceiptModel _m_receipt_model = new();
    private readonly EntityModel _m_entity_model = new();
    private readonly ProductModel _m_product_model = new();
    private readonly ProductShapeModel _m_product_shape_model = new();
    private readonly ProductCategoryModel _m_product_category_model = new();
    private readonly ProductTypeModel _m_product_type_model = new();
    private readonly ProductLotModel _m_product_lot_model = new();
    private readonly BeehiveModel _m_beehive_model = new();

    private ObservableCollection<ReceiptProfitDetailedItem>? _m_list_profit_items;
    private ObservableCollection<ReceiptFeeDetailedItem>? _m_list_fee_items;

    private ObservableCollection<EntityItem>? _m_list_entity_all;
    private ObservableCollection<EntityItem>? _m_list_entity_not_archived;

    private ObservableCollection<ProductItem>? _m_list_product_all;
    private ObservableCollection<ProductItem>? _m_list_product_not_archived;

    private ObservableCollection<ProductShapeItem>? _m_list_product_shape_all;
    private ObservableCollection<ProductShapeItem>? _m_list_product_shape_not_archived;

    private ObservableCollection<ProductCategoryItem>? _m_list_product_category_all;
    private ObservableCollection<ProductCategoryItem>? _m_list_product_category_not_archived;

    private ObservableCollection<ProductTypeItem>? _m_list_product_type_all;
    private ObservableCollection<ProductTypeItem>? _m_list_product_type_not_archived;

    private ObservableCollection<ProductLotItem>? _m_list_product_lot_all;
    private ObservableCollection<ProductLotItem>? _m_list_product_lot_not_archived;

    private ObservableCollection<BeehiveItem>? _m_list_beehive_all;
    private ObservableCollection<BeehiveItem>? _m_list_beehive_not_archived;

    /// <summary>
    /// Represents a predefined collection of database table names and their corresponding display names.
    /// </summary>
    public readonly ObservableCollection<DatabaseTablesNameItem> m_search_tables = new() {
        new(){ m_name_in_database = EDatabaseTableName.NONE, m_name_to_display = string.Empty }, // Search all
        new(){ m_name_in_database = EDatabaseTableName.ENTITY, m_name_to_display = "Client/Fournisseur" },
        new(){ m_name_in_database = EDatabaseTableName.BEEHIVE, m_name_to_display = "Rucher" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT, m_name_to_display = "Produit" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_SHAPE, m_name_to_display = "Forme" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_CATEGORY, m_name_to_display = "Catégorie" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_TYPE, m_name_to_display = "Type" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_LOT, m_name_to_display = "Lot" }
    };


    public ResumeModel() {
        populateListOfItems();
    }

    /// <summary>
    /// Retrieves a filtered and sorted resume item based on the specified search criteria.
    /// </summary>
    /// <param name="search">The search criteria used to filter and sort the resume item. Can be null.</param>
    /// <returns>A <see cref="ResumeItem"/> object that matches the specified search criteria, sorted by year. If no filtering
    /// criteria are provided or the internal data is unavailable, an empty <see cref="ResumeItem"/> is returned.</returns>
    public ResumeItem getFilteredResume(SearchItem? search) {

        var resume_item = new ResumeItem();

        if (_m_list_profit_items == null 
            || _m_list_fee_items == null)
            {
            return resume_item;
        }

        resume_item = filterBy(search);

        return sortByYear(resume_item, search);
    }

    /// <summary>
    /// Retrieves a collection of search items based on the specified database table.
    /// </summary>
    /// <param name="selected_table">The database table from which to retrieve search items. The table is identified by its name in the database.</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="SearchItem"/> objects, where each item represents an
    /// entity from the specified table. The collection will be empty if the table contains no items or if the
    /// corresponding data source is null.</returns>
    public ObservableCollection<SearchItem> getListSearchItems(DatabaseTablesNameItem selected_table) {

        ObservableCollection<SearchItem> list_search_item = new();

        switch (selected_table.m_name_in_database) {
            case EDatabaseTableName.ENTITY:
                if (_m_list_entity_not_archived != null) {
                    foreach (EntityItem item in _m_list_entity_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.entity_id,
                            search_item_to_display = item.entity_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.BEEHIVE:
                if (_m_list_beehive_not_archived != null) {
                    foreach (BeehiveItem item in _m_list_beehive_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.beehive_id,
                            search_item_to_display = item.beehive_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT:
                if (_m_list_product_not_archived != null) {
                    foreach (ProductItem item in _m_list_product_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_id,
                            search_item_to_display = item.product_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_SHAPE:
                if (_m_list_product_shape_not_archived != null) {
                    foreach (ProductShapeItem item in _m_list_product_shape_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_shape_id,
                            search_item_to_display = item.product_shape_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_CATEGORY:
                if (_m_list_product_category_not_archived != null) {
                    foreach (ProductCategoryItem item in _m_list_product_category_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_category_id,
                            search_item_to_display = item.product_category_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_TYPE:
                if (_m_list_product_type_not_archived != null) {
                    foreach (ProductTypeItem item in _m_list_product_type_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_type_id,
                            search_item_to_display = item.product_type_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_LOT:
                if (_m_list_product_lot_not_archived != null) {
                    foreach (ProductLotItem item in _m_list_product_lot_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_lot_id,
                            search_item_to_display = item.product_lot_name
                        });
                    }
                }
                break;
        }
        return list_search_item;
    }

    /// <summary>
    /// Retrieves a collection of search items representing available years.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="SearchItem"/> objects, where each item represents a year
    /// retrieved from the underlying data source. If no years are available, the collection will be empty except for a
    /// default item.</returns>
    public ObservableCollection<SearchItem> getListYears() {

        ObservableCollection<SearchItem> list_search_year = new();
        var years = _m_receipt_model.getExistingYear();

        if (years == null) {
            return list_search_year;
        }

        list_search_year.Add(new SearchItem {
            search_year = string.Empty,
            search_item_to_display = string.Empty
        });
        foreach (string year in years) {
            list_search_year.Add(new SearchItem {
                search_year = year,
                search_item_to_display = string.Empty
            });
        }

        return list_search_year;
    }

    /// <summary>
    /// Filters profit and fee items based on the specified search criteria.
    /// </summary>
    /// <remarks>The filtering behavior depends on the <see cref="EDatabaseTableName"/> specified in the
    /// <paramref name="search"/> parameter: <list type="bullet"> <item> <description> If <see
    /// cref="EDatabaseTableName.NONE"/> is specified, all profit and fee items are returned. </description> </item>
    /// <item> <description> For other table names, filtering is applied based on the corresponding identifier in the
    /// <paramref name="search"/> parameter. </description> </item> </list> If the identifier in <paramref
    /// name="search"/> is zero, no filtering is applied for that table.</remarks>
    /// <param name="search">The search criteria used to filter the items. If <paramref name="search"/> is <see langword="null"/>, all profit
    /// and fee items are returned. If <paramref name="search"/> contains specific criteria, the filtering is applied
    /// based on the <see cref="EDatabaseTableName"/> and associated identifiers.</param>
    /// <returns>A <see cref="ResumeItem"/> containing the filtered profit and fee items. If no matching items are found or the
    /// input parameters are invalid, the returned <see cref="ResumeItem"/> will contain empty collections.</returns>
    private ResumeItem filterBy(SearchItem? search) {

        ResumeItem filtered_data = new();

        if (_m_list_profit_items == null
            || _m_list_fee_items == null 
            ){
            return filtered_data;
        }
        if (search == null) {
            filtered_data.profit_items = _m_list_profit_items;
            filtered_data.fee_items = _m_list_fee_items;
            return filtered_data;
        }
        if (search.search_id == 0
            && search.search_table != EDatabaseTableName.NONE
            ) {
            return filtered_data;
        }

        switch (search.search_table) {
            case EDatabaseTableName.NONE:
                filtered_data.profit_items = _m_list_profit_items;
                filtered_data.fee_items = _m_list_fee_items;
                break;
            case EDatabaseTableName.ENTITY:
                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit => 
                        profit.entity.entity_id == search.search_id
                    )
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee => 
                        fee.entity.entity_id == search.search_id
                    )
                );
                break;
            case EDatabaseTableName.PRODUCT:
                filtered_data.profit_items = new(
                    _m_list_profit_items
                        .Where(profit => profit.receipt_products.Any(product => 
                            product.product_item.product_id == search.search_id))
                        .Select(profit => {
                            // Create a completely new object to avoid reference issues
                            var filteredItem = new ReceiptProfitDetailedItem {
                                // Copy all properties explicitly
                                receipt = new ReceiptItem {
                                    receipt_id = profit.receipt.receipt_id,
                                    receipt_number = profit.receipt.receipt_number,
                                    receipt_date_created = profit.receipt.receipt_date_created,
                                    // Only calculate total price for filtered products
                                    receipt_total_price = profit.receipt_products
                                        .Where(p => p.product_item.product_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = profit.entity,
                                client = profit.client,
                                receipt_client = profit.receipt_client,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    profit.receipt_products
                                        .Where(product => product.product_item.product_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items
                        .Where(fee => fee.receipt_products.Any(product => 
                            product.product_item.product_id == search.search_id))
                        .Select(fee => {
                            var filteredItem = new ReceiptFeeDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = fee.receipt.receipt_id,
                                    receipt_number = fee.receipt.receipt_number,
                                    receipt_date_created = fee.receipt.receipt_date_created,
                                    receipt_total_price = fee.receipt_products
                                        .Where(p => p.product_item.product_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = fee.entity,
                                supplier = fee.supplier,
                                receipt_supplier = fee.receipt_supplier,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    fee.receipt_products
                                        .Where(product => product.product_item.product_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                break;
            case EDatabaseTableName.PRODUCT_TYPE:
                filtered_data.profit_items = new(
                    _m_list_profit_items
                        .Where(profit => profit.receipt_products.Any(product =>
                            product.product_item.fk_product_type_id == search.search_id))
                        .Select(profit => {
                            // Create a completely new object to avoid reference issues
                            var filteredItem = new ReceiptProfitDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = profit.receipt.receipt_id,
                                    receipt_number = profit.receipt.receipt_number,
                                    receipt_date_created = profit.receipt.receipt_date_created,
                                    // Only calculate total price for filtered products
                                    receipt_total_price = profit.receipt_products
                                        .Where(p => p.product_item.fk_product_type_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = profit.entity,
                                client = profit.client,
                                receipt_client = profit.receipt_client,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    profit.receipt_products
                                        .Where(product => product.product_item.fk_product_type_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items
                        .Where(fee => fee.receipt_products.Any(product =>
                            product.product_item.fk_product_type_id == search.search_id))
                        .Select(fee => {
                            var filteredItem = new ReceiptFeeDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = fee.receipt.receipt_id,
                                    receipt_number = fee.receipt.receipt_number,
                                    receipt_date_created = fee.receipt.receipt_date_created,
                                    receipt_total_price = fee.receipt_products
                                        .Where(p => p.product_item.fk_product_type_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = fee.entity,
                                supplier = fee.supplier,
                                receipt_supplier = fee.receipt_supplier,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    fee.receipt_products
                                        .Where(product => product.product_item.fk_product_type_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                break;
            case EDatabaseTableName.PRODUCT_CATEGORY:
                filtered_data.profit_items = new(
                    _m_list_profit_items
                        .Where(profit => profit.receipt_products.Any(product =>
                            product.product_item.fk_product_category_id == search.search_id))
                        .Select(profit => {
                            var filteredItem = new ReceiptProfitDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = profit.receipt.receipt_id,
                                    receipt_number = profit.receipt.receipt_number,
                                    receipt_date_created = profit.receipt.receipt_date_created,
                                    receipt_total_price = profit.receipt_products
                                        .Where(p => p.product_item.fk_product_category_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = profit.entity,
                                client = profit.client,
                                receipt_client = profit.receipt_client,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    profit.receipt_products
                                        .Where(product => product.product_item.fk_product_category_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items
                        .Where(fee => fee.receipt_products.Any(product =>
                            product.product_item.fk_product_category_id == search.search_id))
                        .Select(fee => {
                            var filteredItem = new ReceiptFeeDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = fee.receipt.receipt_id,
                                    receipt_number = fee.receipt.receipt_number,
                                    receipt_date_created = fee.receipt.receipt_date_created,
                                    receipt_total_price = fee.receipt_products
                                        .Where(p => p.product_item.fk_product_category_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = fee.entity,
                                supplier = fee.supplier,
                                receipt_supplier = fee.receipt_supplier,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    fee.receipt_products
                                        .Where(product => product.product_item.fk_product_category_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                break;
            case EDatabaseTableName.PRODUCT_SHAPE:
                filtered_data.profit_items = new(
                    _m_list_profit_items
                        .Where(profit => profit.receipt_products.Any(product =>
                            product.product_item.fk_product_shape_id == search.search_id))
                        .Select(profit => {
                            var filteredItem = new ReceiptProfitDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = profit.receipt.receipt_id,
                                    receipt_number = profit.receipt.receipt_number,
                                    receipt_date_created = profit.receipt.receipt_date_created,
                                    receipt_total_price = profit.receipt_products
                                        .Where(p => p.product_item.fk_product_shape_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = profit.entity,
                                client = profit.client,
                                receipt_client = profit.receipt_client,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    profit.receipt_products
                                        .Where(product => product.product_item.fk_product_shape_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items
                        .Where(fee => fee.receipt_products.Any(product =>
                            product.product_item.fk_product_shape_id == search.search_id))
                        .Select(fee => {
                            var filteredItem = new ReceiptFeeDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = fee.receipt.receipt_id,
                                    receipt_number = fee.receipt.receipt_number,
                                    receipt_date_created = fee.receipt.receipt_date_created,
                                    receipt_total_price = fee.receipt_products
                                        .Where(p => p.product_item.fk_product_shape_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = fee.entity,
                                supplier = fee.supplier,
                                receipt_supplier = fee.receipt_supplier,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    fee.receipt_products
                                        .Where(product => product.product_item.fk_product_shape_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                break;
            case EDatabaseTableName.PRODUCT_LOT:
                filtered_data.profit_items = new(
                    _m_list_profit_items
                        .Where(profit => profit.receipt_products.Any(product =>
                            product.product_lot_item.product_lot_id == search.search_id))
                        .Select(profit => {
                            var filteredItem = new ReceiptProfitDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = profit.receipt.receipt_id,
                                    receipt_number = profit.receipt.receipt_number,
                                    receipt_date_created = profit.receipt.receipt_date_created,
                                    receipt_total_price = profit.receipt_products
                                        .Where(p => p.product_lot_item.product_lot_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = profit.entity,
                                client = profit.client,
                                receipt_client = profit.receipt_client,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    profit.receipt_products
                                        .Where(product => product.product_lot_item.product_lot_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items
                        .Where(fee => fee.receipt_products.Any(product =>
                            product.product_lot_item.product_lot_id == search.search_id))
                        .Select(fee => {
                            var filteredItem = new ReceiptFeeDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = fee.receipt.receipt_id,
                                    receipt_number = fee.receipt.receipt_number,
                                    receipt_date_created = fee.receipt.receipt_date_created,
                                    receipt_total_price = fee.receipt_products
                                        .Where(p => p.product_lot_item.product_lot_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = fee.entity,
                                supplier = fee.supplier,
                                receipt_supplier = fee.receipt_supplier,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    fee.receipt_products
                                        .Where(product => product.product_lot_item.product_lot_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                break;
            case EDatabaseTableName.BEEHIVE:
                filtered_data.profit_items = new(
                    _m_list_profit_items
                        .Where(profit => profit.receipt_products.Any(product =>
                            product.product_lot_item.fk_beehive_id == search.search_id))
                        .Select(profit => {
                            var filteredItem = new ReceiptProfitDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = profit.receipt.receipt_id,
                                    receipt_number = profit.receipt.receipt_number,
                                    receipt_date_created = profit.receipt.receipt_date_created,
                                    receipt_total_price = profit.receipt_products
                                        .Where(p => p.product_lot_item.fk_beehive_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = profit.entity,
                                client = profit.client,
                                receipt_client = profit.receipt_client,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    profit.receipt_products
                                        .Where(product => product.product_lot_item.fk_beehive_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items
                        .Where(fee => fee.receipt_products.Any(product =>
                            product.product_lot_item.fk_beehive_id == search.search_id))
                        .Select(fee => {
                            var filteredItem = new ReceiptFeeDetailedItem {
                                receipt = new ReceiptItem {
                                    receipt_id = fee.receipt.receipt_id,
                                    receipt_number = fee.receipt.receipt_number,
                                    receipt_date_created = fee.receipt.receipt_date_created,
                                    receipt_total_price = fee.receipt_products
                                        .Where(p => p.product_lot_item.fk_beehive_id == search.search_id)
                                        .Sum(p => p.receipt_product_quantity * p.receipt_product_unity_price)
                                },
                                entity = fee.entity,
                                supplier = fee.supplier,
                                receipt_supplier = fee.receipt_supplier,
                                receipt_products = new ObservableCollection<ReceiptProductItem>(
                                    fee.receipt_products
                                        .Where(product => product.product_lot_item.fk_beehive_id == search.search_id)
                                        .Select(p => new ReceiptProductItem {
                                            receipt_product_id = p.receipt_product_id,
                                            receipt_product_quantity = p.receipt_product_quantity,
                                            receipt_product_unity_price = p.receipt_product_unity_price,
                                            fk_receipt_id = p.fk_receipt_id,
                                            product_item = p.product_item,
                                            product_lot_item = p.product_lot_item
                                        })
                                )
                            };
                            return filteredItem;
                        })
                );
                break;
        }

        return filtered_data;
    }

    /// <summary>
    /// Sorts the profit and fee items within a <see cref="ResumeItem"/> by year and date in descending order.
    /// </summary>
    /// <param name="data_to_sort">The <see cref="ResumeItem"/> containing the profit and fee items to be sorted.</param>
    /// <param name="search">An optional <see cref="SearchItem"/> specifying the year to filter the items by. If <c>null</c>, no filtering is
    /// applied.</param>
    /// <returns>A new <see cref="ResumeItem"/> containing the sorted profit and fee items. If both profit and fee items are
    /// empty, an empty <see cref="ResumeItem"/> is returned.</returns>
    private ResumeItem sortByYear(ResumeItem data_to_sort, SearchItem? search) {

        if (data_to_sort.profit_items.Count == 0 && data_to_sort.fee_items.Count == 0) {
            return new ResumeItem();
        }

        var query_profit = data_to_sort.profit_items.AsQueryable();
        var query_fee = data_to_sort.fee_items.AsQueryable();

        // Filter by year if specified in search
        if (search != null) {
            if (search.search_year != string.Empty) {
                string year_to_match = SFormatData.getYearFromDate(search.search_year);
                query_profit = query_profit.Where(item => SFormatData.getYearFromDate(item.receipt.receipt_date_created) == year_to_match);
                query_fee = query_fee.Where(item => SFormatData.getYearFromDate(item.receipt.receipt_date_created) == year_to_match);
            }
        }

        // Sort by parsed date for correct chronological order
        var sorted_items = new ResumeItem();

        if (data_to_sort.profit_items.Count != 0) {
            sorted_items.profit_items = new ObservableCollection<ReceiptProfitDetailedItem>(
                query_profit.OrderByDescending(item =>
                DateTime.ParseExact(item.receipt.receipt_date_created,
                globals.SGlobals.g_EU_DATE_FORMAT,
                System.Globalization.CultureInfo.InvariantCulture).Date)
            );
        }
        if (data_to_sort.fee_items.Count != 0) {

            sorted_items.fee_items = new ObservableCollection<ReceiptFeeDetailedItem>(
                query_fee.OrderByDescending(item =>
                DateTime.ParseExact(item.receipt.receipt_date_created,
                globals.SGlobals.g_EU_DATE_FORMAT,
                System.Globalization.CultureInfo.InvariantCulture).Date)
            );
        }

        return sorted_items;
    }

    /// <summary>
    /// Populates various collections with data retrieved from their respective models, filtering out archived items
    /// where applicable.
    /// </summary>
    /// <remarks>This method initializes collections for entities, products, product shapes, product
    /// categories, product types, product lots, beehives, profits, and fees. For collections that support archiving,
    /// only non-archived items are included. Archived items are identified by an empty string in their respective
    /// archive fields.</remarks>
    private void populateListOfItems() {
        // Entity
        Thread thread_entity = new(new ThreadStart(() => {
            _m_list_entity_all = _m_entity_model.getTable();
            _m_list_entity_not_archived = new(
                _m_list_entity_all.Where(item => item.entity_archive == string.Empty)
            );
        }));

        // Product 
        Thread thread_product = new(new ThreadStart(() => {
            _m_list_product_all = _m_product_model.getTable();
            _m_list_product_not_archived = new(
                _m_list_product_all.Where(item => item.product_archive == string.Empty)
            );
        }));

        // Product Shape
        Thread thread_product_shape = new(new ThreadStart(() => {
            _m_list_product_shape_all = _m_product_shape_model.getTable();
            _m_list_product_shape_not_archived = new(
                _m_list_product_shape_all.Where(item => item.product_shape_archive == string.Empty)
            );
        }));

        // Product Category
        Thread thread_product_category = new(new ThreadStart(() => {
            _m_list_product_category_all = _m_product_category_model.getTable();
            _m_list_product_category_not_archived = new(
                _m_list_product_category_all.Where(item => item.product_category_archive == string.Empty)
            );
        }));


        // Product Type
        Thread thread_product_type = new(new ThreadStart(() => {
            _m_list_product_type_all = _m_product_type_model.getTable();
            _m_list_product_type_not_archived = new(
                _m_list_product_type_all.Where(item => item.product_type_archive == string.Empty)
            );
        }));

        // Product Lot
        Thread thread_product_lot = new(new ThreadStart(() => {
            _m_list_product_lot_all = _m_product_lot_model.getTable();
            _m_list_product_lot_not_archived = new(
                _m_list_product_lot_all.Where(item => item.product_lot_archive == string.Empty)
            );
        }));

        // Beehive
        Thread thread_beehive = new(new ThreadStart(() => {
            _m_list_beehive_all = _m_beehive_model.getTable();
            _m_list_beehive_not_archived = new(
                _m_list_beehive_all.Where(item => item.beehive_archive == string.Empty)
            );
        }));

        // Profit
        Thread thread_profit = new(new ThreadStart(() => {
            _m_list_profit_items = _m_profit_model.getTable();
        }));

        // Fee
        Thread thread_fee = new(new ThreadStart(() => {
            _m_list_fee_items = _m_fee_model.getTable();
        }));

        // Start all threads
        thread_entity.Start();
        thread_product.Start();
        thread_product_shape.Start();
        thread_product_category.Start();
        thread_product_type.Start();
        thread_product_lot.Start();
        thread_beehive.Start();
        thread_profit.Start();
        thread_fee.Start();

        // Wait for all threads to complete
        thread_entity.Join();
        thread_product.Join();
        thread_product_shape.Join();
        thread_product_category.Join();
        thread_product_type.Join();
        thread_product_lot.Join();
        thread_beehive.Join();
        thread_profit.Join();
        thread_fee.Join();
    }
}
