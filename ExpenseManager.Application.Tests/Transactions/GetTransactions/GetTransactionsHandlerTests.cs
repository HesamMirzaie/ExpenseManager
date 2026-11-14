
using ExpenseManager.Application.Common;
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Application.Transactions.GetTransactions;
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;
using Moq;

namespace ExpenseManager.Application.Tests.Transactions.GetTransactions;

public sealed class GetTransactionsHandlerTests
{
    private readonly Mock<ITransactionRepository> _transactionRepository;
    private readonly GetTransactionsHandler _sut;

    public GetTransactionsHandlerTests()
    {
        _transactionRepository = new Mock<ITransactionRepository>();

        _sut = new GetTransactionsHandler(
            _transactionRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidQuery_ReturnsPagedTransactions()
    {
        // Arrange
        var query = new GetTransactionsQuery(
            Page: 1,
            PageSize: 20);

        var expectedResult = new PagedResult<Transaction>(
            Items: Array.Empty<Transaction>(),
            TotalCount: 0,
            Page: 1,
            PageSize: 20);

        _transactionRepository
            .Setup(repository => repository.GetAllAsync(
                query,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _sut.HandleAsync(query);

        // Assert
        Assert.Same(expectedResult, result);

        _transactionRepository.Verify(
            repository => repository.GetAllAsync(
                query,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_InvalidPage_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var query = new GetTransactionsQuery(Page: 0);

        // Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _sut.HandleAsync(query));

        // Assert
        _transactionRepository.Verify(
            repository => repository.GetAllAsync(
                It.IsAny<GetTransactionsQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_InvalidPageSize_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var query = new GetTransactionsQuery(PageSize: 101);

        // Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _sut.HandleAsync(query));

        // Assert
        _transactionRepository.Verify(
            repository => repository.GetAllAsync(
                It.IsAny<GetTransactionsQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_FromDateAfterToDate_ThrowsArgumentException()
    {
        // Arrange
        var query = new GetTransactionsQuery(
            StartDate: new DateOnly(2026, 11, 20),
            EndDate: new DateOnly(2026, 11, 10));

        // Act
        await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.HandleAsync(query));

        // Assert
        _transactionRepository.Verify(
            repository => repository.GetAllAsync(
                It.IsAny<GetTransactionsQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_InvalidTransactionType_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var query = new GetTransactionsQuery(
            Type: (TransactionType)999);

        // Act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _sut.HandleAsync(query));

        // Assert
        _transactionRepository.Verify(
            repository => repository.GetAllAsync(
                It.IsAny<GetTransactionsQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ValidQuery_PassesCancellationTokenToRepository()
    {
        // Arrange
        var query = new GetTransactionsQuery();
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _transactionRepository
            .Setup(repository => repository.GetAllAsync(
                query,
                cancellationToken))
            .ReturnsAsync(new PagedResult<Transaction>(
                Array.Empty<Transaction>(),
                0,
                1,
                20));

        // Act
        await _sut.HandleAsync(query, cancellationToken);

        // Assert
        _transactionRepository.Verify(
            repository => repository.GetAllAsync(
                query,
                cancellationToken),
            Times.Once);
    }
}
