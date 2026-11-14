
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;
using ExpenseManager.Infrastructure.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.Infrastructure.Tests.Repositories;

public sealed class RelationshipConstraintTests
{
    [Fact]
    public async Task SaveChanges_TransactionWithNonExistingCategory_ThrowsDbUpdateException()
    {
        // Arrange
        await using var fixture = new SqliteTestFixture();

        var transaction = new Transaction(
            Guid.NewGuid(),
            TransactionType.Expense,
            100m,
            "Invalid transaction",
            new DateOnly(2026, 11, 14));

        fixture.DbContext.Transactions.Add(transaction);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(
            () => fixture.DbContext.SaveChangesAsync());
    }

   
[Fact]
public async Task Delete_CategoryWithTransactions_ThrowsDbUpdateException()
{
    // Arrange
    await using var fixture = new SqliteTestFixture();

    var category = new Category(
        "Food",
        TransactionType.Expense);

    fixture.DbContext.Categories.Add(category);

    var transaction = new Transaction(
        category.Id,
        category.Type,
        100m,
        "Groceries",
        new DateOnly(2026, 11, 14));

    fixture.DbContext.Transactions.Add(transaction);

    await fixture.DbContext.SaveChangesAsync();

    // Act: use a fresh context so EF doesn't track the transaction.
    await using var deleteContext = fixture.CreateDbContext();

    var categoryToDelete = await deleteContext.Categories
        .SingleAsync(c => c.Id == category.Id);

    deleteContext.Categories.Remove(categoryToDelete);

    // Assert: SQLite should reject the DELETE due to the FK.
    await Assert.ThrowsAsync<DbUpdateException>(
        () => deleteContext.SaveChangesAsync());
}
}
