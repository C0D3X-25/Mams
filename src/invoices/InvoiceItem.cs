
namespace Mams.src.invoices;

public class InvoiceItem {
    public int InvoiceNumber { get; set; } = 0;
    public DateTime IssueDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(14);

    public AddressItem SellerAddress { get; set; } = new();
    public AddressItem CustomerAddress { get; set; } = new();

    public List<OrderItem> Items { get; set; } = new();
    public string Comments { get; set; } = string.Empty;
}

public class OrderItem {
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0.0m;
    public int Quantity { get; set; } = 1;
}

public class AddressItem {
    public string CompanyName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
