using ExpenseManager.API.Controllers;
using ExpenseManager.API.Contracts.Common;
using ExpenseManager.API.Contracts.Transactions;
using ExpenseManager.Application.Common;
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Application.Transactions.CreateTransaction;
using ExpenseManager.Application.Transactions.DeleteTransaction;
using ExpenseManager.Application.Transactions.GetTransactionById;
using ExpenseManager.Application.Transactions.GetTransactions;
using ExpenseManager.Application.Transactions.UpdateTransaction;
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ExpenseManager.API.Tests.Controllers;

public sealed class TransactionsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsPagedMappedResponse()
    {
        var transaction = new Transaction(Guid.NewGuid(), TransactionType.Expense, 25.50m, "Coffee", new DateOnly(2026, 11, 14));
        var repository = new Mock<ITransactionRepository>();
        repository.Setup(x => x.GetAllAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Transaction>([transaction], 3, 2, 2));
        var controller = CreateController(transactionRepository: repository.Object, getAll: new GetTransactionsHandler(repository.Object));

        var result = await controller.GetAll(null, null, null, null, 2, 2, CancellationToken.None);

        var response = Assert.IsType<PagedResponse<TransactionResponse>>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(3, response.TotalCount);
        Assert.Equal(2, response.Page);
        Assert.Equal(2, response.TotalPages);
        Assert.Equal(transaction.Id, response.Items.Single().Id);
        Assert.Equal((int)TransactionType.Expense, response.Items.Single().Type);
    }

    [Fact]
    public async Task GetById_MissingTransaction_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<ITransactionRepository>();
        repository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Transaction?)null);
        var controller = CreateController(transactionRepository: repository.Object, getById: new GetTransactionByIdHandler(repository.Object));

        var result = await controller.GetById(id, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    private static TransactionsController CreateController(
        CreateTransactionHandler? create = null,
        GetTransactionsHandler? getAll = null,
        GetTransactionByIdHandler? getById = null,
        UpdateTransactionHandler? update = null,
        DeleteTransactionHandler? delete = null,
        ITransactionRepository? transactionRepository = null) {
        transactionRepository ??= Mock.Of<ITransactionRepository>();
        var categoryRepository = Mock.Of<ICategoryRepository>();
        var unitOfWork = Mock.Of<IUnitOfWork>();
        return new TransactionsController(
            create ?? new CreateTransactionHandler(categoryRepository, transactionRepository, unitOfWork),
            getAll ?? new GetTransactionsHandler(transactionRepository),
            getById ?? new GetTransactionByIdHandler(transactionRepository),
            update ?? new UpdateTransactionHandler(categoryRepository, transactionRepository, unitOfWork),
            delete ?? new DeleteTransactionHandler(transactionRepository, unitOfWork));
    }
}
