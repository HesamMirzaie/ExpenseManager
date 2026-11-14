
using ExpenseManager.Application.Transactions.GetTransactions;
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;
using ExpenseManager.Infrastructure.Persistence;
using ExpenseManager.Infrastructure.Persistence.Repositories;
using ExpenseManager.Infrastructure.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.Infrastructure.Tests.Repositories;

public sealed class TransactionRepositoryTests
{
    [Fact]
    public async Task Add_Transaction_PersistsTransaction()
    {
        // Arrange
        await using var fixture = new SqliteTestFixture();

        var category = new Category(
            "Food",
            TransactionType.Expense);

        fixture.DbContext.Categories.Add(category);
        await fixture.DbContext.SaveChangesAsync();

        var transaction = new Transaction(
            category.Id,
            category.Type,
            150.50m,
            "Groceries",
            new DateOnly(2026, 11, 14));

        var repository = new TransactionRepository(fixture.DbContext);

        // Act
        repository.Add(transaction);
        await fixture.DbContext.SaveChangesAsync();

        // Assert: verify using a fresh DbContext
        await using var verificationContext = fixture.CreateDbContext();

        var savedTransaction = await verificationContext.Transactions
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == transaction.Id);

        Assert.NotNull(savedTransaction);
        Assert.Equal(category.Id, savedTransaction.CategoryId);
        Assert.Equal(150.50m, savedTransaction.Amount);
        Assert.Equal("Groceries", savedTransaction.Description);
        Assert.Equal(TransactionType.Expense, savedTransaction.Type);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingTransaction_ReturnsTransaction()
    {
        // Arrange
        await using var fixture = new SqliteTestFixture();

        var category = new Category(
            "Food",
            TransactionType.Expense);

        fixture.DbContext.Categories.Add(category);
        await fixture.DbContext.SaveChangesAsync();

        var transaction = new Transaction(
            category.Id,
            category.Type,
            150m,
            "Groceries",
            new DateOnly(2026, 11, 14));

        fixture.DbContext.Transactions.Add(transaction);
        await fixture.DbContext.SaveChangesAsync();

        var repository = new TransactionRepository(fixture.DbContext);

        // Act
        var result = await repository.GetByIdAsync(transaction.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(150m, result.Amount);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingTransaction_ReturnsNull()
    {
        // Arrange
        await using var fixture = new SqliteTestFixture();

        var repository = new TransactionRepository(fixture.DbContext);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WithFilters_ReturnsMatchingTransactions()
    {
        // Arrange
        await using var fixture = new SqliteTestFixture();

        var foodCategory = new Category(
            "Food",
            TransactionType.Expense);

        var salaryCategory = new Category(
            "Salary",
            TransactionType.Income);

        fixture.DbContext.Categories.AddRange(
            foodCategory,
            salaryCategory);

        await fixture.DbContext.SaveChangesAsync();

        fixture.DbContext.Transactions.AddRange(
            new Transaction(
                foodCategory.Id,
                foodCategory.Type,
                100m,
                "Groceries",
                new DateOnly(2026, 11, 1)),

            new Transaction(
                foodCategory.Id,
                foodCategory.Type,
                50m,
                "Restaurant",
                new DateOnly(2026, 11, 5)),

            new Transaction(
                salaryCategory.Id,
                salaryCategory.Type,
                2000m,
                "Monthly salary",
                new DateOnly(2026, 11, 3)));

        await fixture.DbContext.SaveChangesAsync();

        var repository = new TransactionRepository(fixture.DbContext);

        var query = new GetTransactionsQuery(
            CategoryId: foodCategory.Id,
            Type: TransactionType.Expense,
            StartDate: new DateOnly(2026, 11, 1),
            EndDate: new DateOnly(2026, 11, 5));

        // Act
        var result = await repository.GetAllAsync(query);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);

        Assert.All(
            result.Items,
            transaction =>
            {
                Assert.Equal(foodCategory.Id, transaction.CategoryId);
                Assert.Equal(TransactionType.Expense, transaction.Type);
            });

        Assert.Equal(
            new[] { 50m, 100m },
            result.Items.Select(t => t.Amount).ToArray());
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        await using var fixture = new SqliteTestFixture();

        var category = new Category(
            "Food",
            TransactionType.Expense);

        fixture.DbContext.Categories.Add(category);
        await fixture.DbContext.SaveChangesAsync();

        for (var day = 1; day <= 5; day++)
        {
            fixture.DbContext.Transactions.Add(
                new Transaction(
                    category.Id,
                    category.Type,
                    day * 10m,
                    $"Transaction {day}",
                    new DateOnly(2026, 11, day)));
        }

        await fixture.DbContext.SaveChangesAsync();

        var repository = new TransactionRepository(fixture.DbContext);

        var query = new GetTransactionsQuery(
            Page: 2,
            PageSize: 2);

        // Act
        var result = await repository.GetAllAsync(query);

        // Assert
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(2, result.Items.Count);

        // Descending date order: day 3, then day 2.
        Assert.Equal(30m, result.Items[0].Amount);
        Assert.Equal(20m, result.Items[1].Amount);
    }
}
