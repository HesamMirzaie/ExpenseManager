
using ExpenseManager.Application.Categories.GetCategories;
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;
using Moq;

namespace ExpenseManager.Application.Tests.Categories.GetCategories;

public sealed class GetCategoriesHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly GetCategoriesHandler _sut;

    public GetCategoriesHandlerTests()
    {
        _categoryRepository = new Mock<ICategoryRepository>();

        _sut = new GetCategoriesHandler(
            _categoryRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ReturnsAllCategories()
    {
        // Arrange
        IReadOnlyList<Category> expectedCategories =
        [
            new Category("Food", TransactionType.Expense),
            new Category("Salary", TransactionType.Income)
        ];

        _categoryRepository
            .Setup(repository => repository.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCategories);

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(expectedCategories, result);

        _categoryRepository.Verify(
            repository => repository.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NoCategories_ReturnsEmptyList()
    {
        // Arrange
        IReadOnlyList<Category> emptyCategories = [];

        _categoryRepository
            .Setup(repository => repository.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyCategories);

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.Empty(result);
    }
}
