
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;
using ExpenseManager.Infrastructure.Persistence;
using ExpenseManager.Infrastructure.Persistence.Repositories;
using ExpenseManager.Infrastructure.Tests.Fixtures;

namespace ExpenseManager.Infrastructure.Tests.Repositories;

public sealed class CategoryRepositoryTests
{
    [Fact]
    public async Task Add_Category_PersistsCategory()
    {
        // Arrange
        await using var fixture = new SqliteTestFixture();

        var repository = new CategoryRepository(fixture.DbContext);

        var category = new Category(
            "Food",
            TransactionType.Expense);

        // Act
        repository.Add(category);
        await fixture.DbContext.SaveChangesAsync();

        // Assert
        var savedCategory = await fixture.DbContext.Categories
            .FindAsync(category.Id);

        Assert.NotNull(savedCategory);
        Assert.Equal("Food", savedCategory.Name);
        Assert.Equal(TransactionType.Expense, savedCategory.Type);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCategory_ReturnsCategory()
    {
        // Arrange
        await using var fixture = new SqliteTestFixture();

        var category = new Category(
            "Salary",
            TransactionType.Income);

        fixture.DbContext.Categories.Add(category);
        await fixture.DbContext.SaveChangesAsync();

        var repository = new CategoryRepository(fixture.DbContext);

        // Act
        var result = await repository.GetByIdAsync(category.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal("Salary", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingCategory_ReturnsNull()
    {
        // Arrange
        await using var fixture = new SqliteTestFixture();

        var repository = new CategoryRepository(fixture.DbContext);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCategoriesOrderedByName()
    {
        // Arrange
        await using var fixture = new SqliteTestFixture();

        fixture.DbContext.Categories.AddRange(
            new Category("Transport", TransactionType.Expense),
            new Category("Food", TransactionType.Expense),
            new Category("Salary", TransactionType.Income));

        await fixture.DbContext.SaveChangesAsync();

        var repository = new CategoryRepository(fixture.DbContext);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);

        Assert.Equal(
            new[] { "Food", "Salary", "Transport" },
            result.Select(category => category.Name).ToArray());
    }
}
