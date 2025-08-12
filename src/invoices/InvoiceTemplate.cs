using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Mams.src.invoices;

/// <summary>
/// What the PDF will look like
/// </summary>
public class InvoiceTemplate : IDocument {

    private const string _m_path_image = "";
    private InvoiceItem _m_item { get; set; } 

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;


    public InvoiceTemplate(InvoiceItem item) {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
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
                    .Item().Text($"Facture #{_m_item.InvoiceNumber}")
                    .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                column.Item().Text(text =>
                {
                    text.Span("Date d'émission: ").SemiBold();
                    text.Span($"{_m_item.IssueDate:d}");
                });

                column.Item().Text(text =>
                {
                    text.Span("Date d'échéance: ").SemiBold();
                    text.Span($"{_m_item.DueDate:d}");
                });
            });

            // To avoid a crash if the image path is invalid or empty
            if (!string.IsNullOrWhiteSpace(_m_path_image)) {
                row.ConstantItem(175).Image(_m_path_image);
            }
        });
    }


    void ComposeContent(IContainer container) {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(20);

            column.Item().Row(row =>
            {
                row.RelativeItem().Component(new AddressComponent("De", _m_item.SellerAddress));
                row.ConstantItem(50);
                row.RelativeItem().Component(new AddressComponent("Pour", _m_item.CustomerAddress));
            });

            column.Item().Element(ComposeTable);

            var totalPrice = _m_item.Items.Sum(x => x.Price * x.Quantity);
            column.Item().PaddingRight(5).AlignRight().Text($"Total final: {totalPrice:C}").SemiBold();

            if (!string.IsNullOrWhiteSpace(_m_item.Comments))
                column.Item().PaddingTop(25).Element(ComposeComments);
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

            foreach (var item in _m_item.Items) {
                var index = _m_item.Items.IndexOf(item) + 1;

                table.Cell().Element(CellStyle).Text($"{index}");
                table.Cell().Element(CellStyle).Text(item.Name);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price:C}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price * item.Quantity:C}");

                static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
            }
        });
    }


    void ComposeComments(IContainer container) {
        container.ShowEntire().Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
        {
            column.Spacing(5);
            column.Item().Text("Commentaire").FontSize(14).SemiBold();
            column.Item().Text(_m_item.Comments);
        });
    }


    public class AddressComponent : IComponent {
        private string Title { get; }
        private AddressItem Address { get; }

        public AddressComponent(string title, AddressItem address) {
            Title = title;
            Address = address;
        }

        public void Compose(IContainer container) {
            container.ShowEntire().Column(column => {
                column.Spacing(2);

                column.Item().Text(Title).SemiBold();
                column.Item().PaddingBottom(5).LineHorizontal(1);

                column.Item().Text(Address.CompanyName);
                column.Item().Text(Address.Street);
                column.Item().Text($"{Address.City}, {Address.State}");
                column.Item().Text(Address.Email);
                column.Item().Text(Address.Phone);
            });
        }
    }
}