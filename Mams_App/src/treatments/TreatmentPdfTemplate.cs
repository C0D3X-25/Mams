using Mams_App.src.localizations;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;

namespace Mams_App.src.treatments;

/// <summary>
/// Defines the structure and appearance of treatment log PDFs.
/// </summary>
public class TreatmentPdfTemplate : IDocument
{
    private readonly ObservableCollection<TreatmentItem> _m_items;

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    /// <summary>
    /// Initializes a new instance of the TreatmentPdfTemplate class with treatment data.
    /// </summary>
    /// <param name="items">The collection of treatment items to include in the PDF.</param>
    public TreatmentPdfTemplate(ObservableCollection<TreatmentItem> items)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        _m_items = items;
    }

    /// <summary>
    /// Defines the structure and layout of the treatment log document.
    /// </summary>
    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
    }

    /// <summary>
    /// Composes the header section of the treatment log PDF.
    /// </summary>
    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column
                .Item().Text(Loc.Get("Pdf.TreatmentTitle"))
                .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
        });
    }

    /// <summary>
    /// Composes the main content section with the treatment table.
    /// </summary>
    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(10);
            column.Item().Element(ComposeTable);
        });
    }

    /// <summary>
    /// Composes the table section displaying treatments.
    /// </summary>
    private void ComposeTable(IContainer container)
    {
        var header_style = TextStyle.Default.SemiBold();

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(25);     // #
                columns.RelativeColumn(2);      // Date
                columns.RelativeColumn(2);      // Rucher
                columns.RelativeColumn();       // N°
                columns.RelativeColumn(2);      // Région
                columns.RelativeColumn();       // Ruches
                columns.RelativeColumn(2);      // Produit
                columns.RelativeColumn();       // Unité
                columns.RelativeColumn(1.5f);   // Dose/ruche
                columns.RelativeColumn(1.5f);   // Dose tot
            });

            table.Header(header =>
            {
                header.Cell().Text("#");
                header.Cell().Text(Loc.Get("Column.Date")).Style(header_style);
                header.Cell().Text(Loc.Get("Column.Beehive")).Style(header_style);
                header.Cell().Text(Loc.Get("Column.Number")).Style(header_style);
                header.Cell().Text(Loc.Get("Column.Region")).Style(header_style);
                header.Cell().AlignRight().Text(Loc.Get("Column.HiveCount")).Style(header_style);
                header.Cell().Text(Loc.Get("Column.Product")).Style(header_style);
                header.Cell().Text(Loc.Get("Column.DoseUnit")).Style(header_style);
                header.Cell().AlignRight().Text(Loc.Get("Column.DosePerHive")).Style(header_style);
                header.Cell().AlignRight().Text(Loc.Get("Column.DoseTotal")).Style(header_style);

                header.Cell().ColumnSpan(10).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);
            });

            foreach (var item in _m_items)
            {
                var index = _m_items.IndexOf(item) + 1;

                table.Cell().Element(CellStyle).Text($"{index}");
                table.Cell().Element(CellStyle).Text(item.treatment_date);
                table.Cell().Element(CellStyle).Text(item.beehive_name);
                table.Cell().Element(CellStyle).Text(item.beehive_number);
                table.Cell().Element(CellStyle).Text(item.region_name);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.treatment_hive_count}");
                table.Cell().Element(CellStyle).Text(item.product_name);
                table.Cell().Element(CellStyle).Text(item.dose_unit_name);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.treatment_dose_per_hive:F2}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.treatment_dose_total:F2}");

                static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
            }
        });
    }
}
