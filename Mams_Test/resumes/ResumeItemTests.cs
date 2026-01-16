using Mams_App.src.resumes;
using Mams_App.src.fees;
using Mams_App.src.profits;
using System.Collections.ObjectModel;

namespace Mams_Test.resumes;

public class ResumeItemTests
{
    [Fact]
    public void ResumeItem_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var item = new ResumeItem();

        // Assert
        Assert.NotNull(item.profit_items);
        Assert.NotNull(item.fee_items);
        Assert.Empty(item.profit_items);
        Assert.Empty(item.fee_items);
    }

    [Fact]
    public void ResumeItem_SetProfitItems_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ResumeItem();
        var profits = new ObservableCollection<ReceiptProfitDetailedItem>
        {
            new ReceiptProfitDetailedItem(),
            new ReceiptProfitDetailedItem()
        };

        // Act
        item.profit_items = profits;

        // Assert
        Assert.Equal(2, item.profit_items.Count);
    }

    [Fact]
    public void ResumeItem_SetFeeItems_ShouldReturnCorrectValue()
    {
        // Arrange
        var item = new ResumeItem();
        var fees = new ObservableCollection<ReceiptFeeDetailedItem>
        {
            new ReceiptFeeDetailedItem(),
            new ReceiptFeeDetailedItem()
        };

        // Act
        item.fee_items = fees;

        // Assert
        Assert.Equal(2, item.fee_items.Count);
    }

    [Fact]
    public void ResumeItem_AddProfitItem_ShouldAddToCollection()
    {
        // Arrange
        var item = new ResumeItem();
        var profit = new ReceiptProfitDetailedItem();

        // Act
        item.profit_items.Add(profit);

        // Assert
        Assert.Single(item.profit_items);
    }

    [Fact]
    public void ResumeItem_AddFeeItem_ShouldAddToCollection()
    {
        // Arrange
        var item = new ResumeItem();
        var fee = new ReceiptFeeDetailedItem();

        // Act
        item.fee_items.Add(fee);

        // Assert
        Assert.Single(item.fee_items);
    }

    [Fact]
    public void ResumeItem_ClearProfitItems_ShouldEmptyCollection()
    {
        // Arrange
        var item = new ResumeItem();
        item.profit_items.Add(new ReceiptProfitDetailedItem());
        item.profit_items.Add(new ReceiptProfitDetailedItem());

        // Act
        item.profit_items.Clear();

        // Assert
        Assert.Empty(item.profit_items);
    }

    [Fact]
    public void ResumeItem_ClearFeeItems_ShouldEmptyCollection()
    {
        // Arrange
        var item = new ResumeItem();
        item.fee_items.Add(new ReceiptFeeDetailedItem());
        item.fee_items.Add(new ReceiptFeeDetailedItem());

        // Act
        item.fee_items.Clear();

        // Assert
        Assert.Empty(item.fee_items);
    }
}
