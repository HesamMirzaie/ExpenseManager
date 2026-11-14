
using ExpenseManager.Application.Categories.GetCategoryById;
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;
using Moq;

namespace ExpenseManager.Application.Tests.Categories.GetCategoryById;

public sealed class GetCategoryByIdHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly GetCategoryByIdHandler _sut;

    public GetCategoryByIdHandlerTests()
    {
        _categoryRepository = new Mock<ICategoryRepository>();

        _sut = new GetCategoryByIdHandler(
            _categoryRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ExistingId_ReturnsCategory()
    {
        // Arrange
        var category = new Category(
            "Food",
            TransactionType.Expense);

        _categoryRepository
            .Setup(repository => repository.GetByIdAsync(
                category.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _sut.HandleAsync(category.Id);

        // Assert
        Assert.Same(category, result);

        _categoryRepository.Verify(
            repository => repository.GetByIdAsync(
                category.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _categoryRepository
            .Setup(repository => repository.GetByIdAsync(
                categoryId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _sut.HandleAsync(categoryId);

        // Assert
        Assert.Null(result);
    }
}
