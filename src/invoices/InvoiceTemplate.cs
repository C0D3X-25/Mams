using Mams.src.clients;
using Mams.src.entities;
using Mams.src.receipts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Mams.src.invoices;

/// <summary>
/// What the PDF will look like
/// </summary>
public class InvoiceTemplate : IDocument {

    private readonly EntityModel _m_entity_model;
    private readonly ClientModel _m_client_model;

    private const string _m_path_image = "";
    private ReceiptHandlerItem _m_item { get; set; }

    
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;


    public InvoiceTemplate(ReceiptHandlerItem item) {

        // QuestPDF licence NEED to be present
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        _m_entity_model = new();
        _m_client_model = new();

        _m_item = item;
    }

    /// <summary>
    /// Is automatically called when the PDF is generated.
    /// </summary>
    /// <param name="container"></param>
    public void Compose(IDocumentContainer container) {
        container
            .Page(page => {
                page.Margin(50);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter().Text(text => {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
    }


    void ComposeHeader(IContainer container) {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column
                    .Item().Text($"Facture #{_m_item.receipt_item.receipt_number}")
                    .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                column.Item().Text(text =>
                {
                    text.Span("Date de la vente: ").SemiBold();
                    text.Span($"{_m_item.receipt_item.receipt_date_created:d}");
                });
            });

            // To avoid a crash if the image path is invalid or empty
            if (!string.IsNullOrWhiteSpace(_m_path_image)) {
                row.ConstantItem(175).Image(_m_path_image);
            }
        });
    }


    void ComposeContent(IContainer container) {

        container.PaddingVertical(40).Column(column => {
            column.Spacing(20);

            column.Item().Row(row => {
                var client_item = _m_client_model.getItemByID(_m_item.receipt_client_item.fk_client_id.ToString());
                EntityItem? entity_item = null;
                if (client_item != null)
                {
                    entity_item = _m_entity_model.getItemByID(client_item.fk_entity_id.ToString());
                }

                row.RelativeItem().Component(new AddressComponent("De", entity_item ?? new EntityItem()));
                row.ConstantItem(50);
                row.RelativeItem().Component(new AddressComponent("Pour", entity_item ?? new EntityItem()));
            });

            column.Item().Element(ComposeTable);

            var total_price = _m_item.receipt_product_items.Sum(x => x.receipt_product_unity_price* x.receipt_product_quantity);
            column.Item().PaddingRight(5).AlignRight().Text($"Total final: {total_price:C}").SemiBold();
        });
    }


    void ComposeTable(IContainer container) {
        var headerStyle = TextStyle.Default.SemiBold();

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(25);
                columns.RelativeColumn(3);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Text("#");
                header.Cell().Text("Produit").Style(headerStyle);
                header.Cell().AlignRight().Text("Prix unité").Style(headerStyle);
                header.Cell().AlignRight().Text("Quantité").Style(headerStyle);
                header.Cell().AlignRight().Text("Total").Style(headerStyle);

                header.Cell().ColumnSpan(5).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);
            });

            foreach (var item in _m_item.receipt_product_items) {
                var index = _m_item.receipt_product_items.IndexOf(item) + 1;

                table.Cell().Element(CellStyle).Text($"{index}");
                table.Cell().Element(CellStyle).Text(item.product_item.product_name);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.receipt_product_unity_price:C}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.receipt_product_quantity}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.receipt_product_unity_price * item.receipt_product_quantity:C}");

                static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
            }
        });
    }


    public class AddressComponent : IComponent {
        private string Title { get; }
        private EntityItem entity { get; }

        public AddressComponent(string title, EntityItem address) {
            Title = title;
            entity = address;
        }

        public void Compose(IContainer container) {
            container.ShowEntire().Column(column => {
                column.Spacing(2);

                column.Item().Text(Title).SemiBold();
                column.Item().PaddingBottom(5).LineHorizontal(1);

                column.Item().Text(entity.entity_name);
                column.Item().Text(entity.entity_address);
                column.Item().Text(entity.entity_email);
                column.Item().Text(entity.entity_phone);
            });
        }
    }
}