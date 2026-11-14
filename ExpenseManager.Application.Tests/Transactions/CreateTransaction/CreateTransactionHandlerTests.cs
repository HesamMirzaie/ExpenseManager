
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Application.Transactions.CreateTransaction;
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;
using Moq;

namespace ExpenseManager.Application.Tests.Transactions.CreateTransaction;

public sealed class CreateTransactionHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly Mock<ITransactionRepository> _transactionRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly CreateTransactionHandler _sut;

    public CreateTransactionHandlerTests()
    {
        _categoryRepository = new Mock<ICategoryRepository>();
        _transactionRepository = new Mock<ITransactionRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _sut = new CreateTransactionHandler(
            _categoryRepository.Object,
            _transactionRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidRequest_CreatesTransaction()
    {
        // Arrange
        var category = new Category(
            "Food",
            TransactionType.Expense);

        var request = new CreateTransactionRequest(
            category.Id,
            150.50m,
            "Groceries",
            new DateOnly(2026, 11, 14));

        _categoryRepository
            .Setup(repository => repository.GetByIdAsync(
                category.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _unitOfWork
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Transaction? addedTransaction = null;

        _transactionRepository
            .Setup(repository => repository.Add(
                It.IsAny<Transaction>()))
            .Callback<Transaction>(
                transaction => addedTransaction = transaction);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.CategoryId);
        Assert.Equal(TransactionType.Expense, result.Type);
        Assert.Equal(150.50m, result.Amount);
        Assert.Equal("Groceries", result.Description);
        Assert.Equal(new DateOnly(2026, 11, 14), result.Date);

        Assert.NotNull(addedTransaction);
        Assert.Equal(result.Id, addedTransaction.Id);

        _categoryRepository.Verify(
            repository => repository.GetByIdAsync(
                category.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _transactionRepository.Verify(
            repository => repository.Add(
                It.IsAny<Transaction>()),
            Times.Once);

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CategoryDoesNotExist_ThrowsCategoryNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var request = new CreateTransactionRequest(
            categoryId,
            150.50m,
            "Groceries",
            new DateOnly(2026, 11, 14));

        _categoryRepository
            .Setup(repository => repository.GetByIdAsync(
                categoryId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => _sut.HandleAsync(request));

        // Assert
        Assert.Contains(categoryId.ToString(), exception.Message);
    }

    [Fact]
    public async Task HandleAsync_CategoryDoesNotExist_DoesNotPersistTransaction()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var request = new CreateTransactionRequest(
            categoryId,
            150.50m,
            "Groceries",
            new DateOnly(2026, 11, 14));

        _categoryRepository
            .Setup(repository => repository.GetByIdAsync(
                categoryId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => _sut.HandleAsync(request));

        // Assert
        _transactionRepository.Verify(
            repository => repository.Add(
                It.IsAny<Transaction>()),
            Times.Never);

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_InvalidAmount_DoesNotPersistTransaction()
    {
        // Arrange
        var category = new Category(
            "Food",
            TransactionType.Expense);

        var request = new CreateTransactionRequest(
            category.Id,
            0,
            "Groceries",
            new DateOnly(2026, 11, 14));

        _categoryRepository
            .Setup(repository => repository.GetByIdAsync(
                category.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _sut.HandleAsync(request));

        // Assert
        _transactionRepository.Verify(
            repository => repository.Add(
                It.IsAny<Transaction>()),
            Times.Never);

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
