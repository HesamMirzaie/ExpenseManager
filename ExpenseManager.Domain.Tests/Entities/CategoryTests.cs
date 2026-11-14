using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;

namespace ExpenseManager.Domain.Tests.Entities;

public class CategoryTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesCategory()
    {
        // Arrange
        const string name = "Groceries";
        const TransactionType type = TransactionType.Expense;

        // Act
        var category = new Category(name, type);

        // Assert
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal(name, category.Name);
        Assert.Equal(type, category.Type);
    }

    [Fact]
    public void Constructor_WithWhitespaceName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new Category("   ", TransactionType.Expense));
    }

    [Fact]
    public void Constructor_WithInvalidType_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Category("Groceries", (TransactionType)999));
    }
}