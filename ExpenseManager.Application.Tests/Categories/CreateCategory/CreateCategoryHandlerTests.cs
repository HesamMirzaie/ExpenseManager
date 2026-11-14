
using ExpenseManager.Application.Categories.CreateCategory;
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Enums;
using Moq;

namespace ExpenseManager.Application.Tests.Categories.CreateCategory;

public sealed class CreateCategoryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateCategoryHandler _sut;

    public CreateCategoryHandlerTests()
    {
        _categoryRepository = new Mock<ICategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _sut = new CreateCategoryHandler(
            _categoryRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidRequest_CreatesCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest(
            "Food",
            TransactionType.Expense);

        _unitOfWork
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        ExpenseManager.Domain.Entities.Category? addedCategory = null;

        _categoryRepository
            .Setup(repository => repository.Add(
                It.IsAny<ExpenseManager.Domain.Entities.Category>()))
            .Callback<ExpenseManager.Domain.Entities.Category>(
                category => addedCategory = category);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Food", result.Name);
        Assert.Equal(TransactionType.Expense, result.Type);

        Assert.NotNull(addedCategory);
        Assert.Equal(result.Id, addedCategory.Id);

        _categoryRepository.Verify(
            repository => repository.Add(
                It.IsAny<ExpenseManager.Domain.Entities.Category>()),
            Times.Once);

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_InvalidName_DoesNotPersistCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest(
            " ",
            TransactionType.Expense);

        // Act
        await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.HandleAsync(request));

        // Assert
        _categoryRepository.Verify(
            repository => repository.Add(
                It.IsAny<ExpenseManager.Domain.Entities.Category>()),
            Times.Never);

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
