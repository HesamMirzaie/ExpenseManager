
using ExpenseManager.API.Contracts.Transactions;
using CreateTransactionHandler = ExpenseManager.Application.Transactions.CreateTransaction.CreateTransactionHandler;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TransactionsController : ControllerBase
{
    private readonly CreateTransactionHandler _createTransactionHandler;

    public TransactionsController(
        CreateTransactionHandler createTransactionHandler)
    {
        _createTransactionHandler = createTransactionHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var applicationRequest =
            new Application.Transactions.CreateTransaction.CreateTransactionRequest(
                request.CategoryId,
                request.Amount,
                request.Description,
                request.Date);

        var transaction = await _createTransactionHandler.HandleAsync(
            applicationRequest,
            cancellationToken);

        return Created(
            $"/api/transactions/{transaction.Id}",
            transaction);
    }
}
