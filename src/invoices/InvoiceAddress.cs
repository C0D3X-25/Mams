using Mams.src.entities;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Mams.src.invoices;

public class InvoiceAddress : IComponent {
    private string _m_title { get; }
    private EntityItem _m_entity { get; }

    public InvoiceAddress(string title, EntityItem entity) {
        _m_title = title;
        _m_entity = entity;
    }

    public void Compose(IContainer container) {
        container.ShowEntire().Column(column => {
            column.Spacing(2);

            column.Item().Text(_m_title).SemiBold();
            column.Item().PaddingBottom(5).LineHorizontal(1);

            column.Item().Text(_m_entity.entity_name);
            column.Item().Text(_m_entity.entity_city);
            column.Item().Text(_m_entity.entity_address);
            column.Item().Text(_m_entity.entity_email);
            column.Item().Text(_m_entity.entity_phone);
        });
    }
}
