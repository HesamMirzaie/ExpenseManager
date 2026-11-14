
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Application.Transactions.GetTransactionById;
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;
using Moq;

namespace ExpenseManager.Application.Tests.Transactions.GetTransactionById;

public sealed class GetTransactionByIdHandlerTests
{
    private readonly Mock<ITransactionRepository> _transactionRepository;
    private readonly GetTransactionByIdHandler _sut;

    public GetTransactionByIdHandlerTests()
    {
        _transactionRepository = new Mock<ITransactionRepository>();

        _sut = new GetTransactionByIdHandler(
            _transactionRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ExistingId_ReturnsTransaction()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var expectedTransaction = new Transaction(
            categoryId,
            TransactionType.Expense,
            150.50m,
            "Groceries",
            new DateOnly(2026, 11, 14));

        _transactionRepository
            .Setup(repository => repository.GetByIdAsync(
                expectedTransaction.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTransaction);

        // Act
        var result = await _sut.HandleAsync(expectedTransaction.Id);

        // Assert
        Assert.Same(expectedTransaction, result);

        _transactionRepository.Verify(
            repository => repository.GetByIdAsync(
                expectedTransaction.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        _transactionRepository
            .Setup(repository => repository.GetByIdAsync(
                transactionId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction?)null);

        // Act
        var result = await _sut.HandleAsync(transactionId);

        // Assert
        Assert.Null(result);

        _transactionRepository.Verify(
            repository => repository.GetByIdAsync(
                transactionId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
