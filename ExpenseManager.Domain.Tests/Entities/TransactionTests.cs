
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;

namespace ExpenseManager.Domain.Tests.Entities;

public class TransactionTests
{
    private static readonly Guid ValidCategoryId =
        Guid.NewGuid();

    private static readonly DateOnly ValidDate =
        new(2026, 11, 14);

    [Fact]
    public void Constructor_WithValidData_CreatesTransaction()
    {
        // Arrange
        const decimal amount = 150.50m;
        const string description = "Groceries";

        // Act
        var transaction = new Transaction(
            ValidCategoryId,
            TransactionType.Expense,
            amount,
            description,
            ValidDate);

        // Assert
        Assert.NotEqual(Guid.Empty, transaction.Id);
        Assert.Equal(ValidCategoryId, transaction.CategoryId);
        Assert.Equal(TransactionType.Expense, transaction.Type);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(description, transaction.Description);
        Assert.Equal(ValidDate, transaction.Date);
    }

    [Fact]
    public void Constructor_WithEmptyCategoryId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new Transaction(
                Guid.Empty,
                TransactionType.Expense,
                100m,
                "Groceries",
                ValidDate));
    }

    [Fact]
    public void Constructor_WithInvalidType_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Transaction(
                ValidCategoryId,
                (TransactionType)999,
                100m,
                "Groceries",
                ValidDate));
    }

    [Fact]
    public void Constructor_WithZeroAmount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Transaction(
                ValidCategoryId,
                TransactionType.Expense,
                0m,
                "Groceries",
                ValidDate));
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Transaction(
                ValidCategoryId,
                TransactionType.Expense,
                -100m,
                "Groceries",
                ValidDate));
    }

    [Fact]
    public void Constructor_WithWhitespaceDescription_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new Transaction(
                ValidCategoryId,
                TransactionType.Expense,
                100m,
                "   ",
                ValidDate));
    }

    [Fact]
    public void Constructor_WithValidDescription_TrimsDescription()
    {
        var transaction = new Transaction(
            ValidCategoryId,
            TransactionType.Expense,
            100m,
            "  Groceries  ",
            ValidDate);

        Assert.Equal("Groceries", transaction.Description);
    }

    [Fact]
    public void ChangeAmount_WithValidAmount_UpdatesAmount()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        transaction.ChangeAmount(250m);

        // Assert
        Assert.Equal(250m, transaction.Amount);
    }

    [Fact]
    public void ChangeAmount_WithZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => transaction.ChangeAmount(0m));

        Assert.Equal(100m, transaction.Amount);
    }

    [Fact]
    public void ChangeDescription_WithValidValue_UpdatesDescription()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        transaction.ChangeDescription("Restaurant");

        // Assert
        Assert.Equal("Restaurant", transaction.Description);
    }

    [Fact]
    public void ChangeDescription_WithWhitespace_ThrowsArgumentException()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => transaction.ChangeDescription("   "));

        Assert.Equal("Groceries", transaction.Description);
    }

    private static Transaction CreateValidTransaction()
    {
        return new Transaction(
            ValidCategoryId,
            TransactionType.Expense,
            100m,
            "Groceries",
            ValidDate);
    }
}
