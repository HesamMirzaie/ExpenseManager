
using ExpenseManager.API.Contracts.Transactions;
using ExpenseManager.API.Contracts.Common;
using ExpenseManager.Application.Transactions.GetTransactions;
using ExpenseManager.Application.Transactions.GetTransactionById;
using ExpenseManager.Application.Transactions.UpdateTransaction;
using ExpenseManager.Application.Transactions.DeleteTransaction;
using ExpenseManager.Domain.Enums;
using CreateTransactionHandler = ExpenseManager.Application.Transactions.CreateTransaction.CreateTransactionHandler;
using GetTransactionsHandler = ExpenseManager.Application.Transactions.GetTransactions.GetTransactionsHandler;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TransactionsController : ControllerBase
{
    private readonly CreateTransactionHandler _createTransactionHandler;

    private readonly GetTransactionsHandler _getTransactionsHandler;
    private readonly GetTransactionByIdHandler _getTransactionByIdHandler;
    private readonly UpdateTransactionHandler _updateTransactionHandler;
    private readonly DeleteTransactionHandler _deleteTransactionHandler;

    public TransactionsController(
        CreateTransactionHandler createTransactionHandler,
        GetTransactionsHandler getTransactionsHandler,
        GetTransactionByIdHandler getTransactionByIdHandler,
        UpdateTransactionHandler updateTransactionHandler,
        DeleteTransactionHandler deleteTransactionHandler)
    {
        _createTransactionHandler = createTransactionHandler;
        _getTransactionsHandler = getTransactionsHandler;
        _getTransactionByIdHandler = getTransactionByIdHandler;
        _updateTransactionHandler = updateTransactionHandler;
        _deleteTransactionHandler = deleteTransactionHandler;
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

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TransactionResponse>>> GetAll(
        [FromQuery] Guid? categoryId,
        [FromQuery] TransactionType? type,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTransactionsQuery(
            categoryId,
            type,
            startDate,
            endDate,
            page,
            pageSize);

        var result = await _getTransactionsHandler.HandleAsync(
            query,
            cancellationToken);

        var response = new PagedResponse<TransactionResponse>(
            result.Items.Select(transaction => new TransactionResponse(
                transaction.Id,
                transaction.CategoryId,
                (int)transaction.Type,
                transaction.Amount,
                transaction.Description,
                transaction.Date)).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize,
            (int)Math.Ceiling(
                (double)result.TotalCount / result.PageSize));

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var transaction = await _getTransactionByIdHandler.HandleAsync(
            id,
            cancellationToken);

        if (transaction is null)
        {
            return NotFound();
        }

        var response = new TransactionResponse(
            transaction.Id,
            transaction.CategoryId,
            (int)transaction.Type,
            transaction.Amount,
            transaction.Description,
            transaction.Date);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TransactionResponse>> Update(
        Guid id,
        [FromBody] ExpenseManager.API.Contracts.Transactions.UpdateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var applicationRequest =
            new Application.Transactions.UpdateTransaction.UpdateTransactionRequest(
                request.CategoryId,
                request.Amount,
                request.Description,
                request.Date);

        var transaction = await _updateTransactionHandler.HandleAsync(
            id,
            applicationRequest,
            cancellationToken);

        return Ok(new TransactionResponse(
            transaction.Id,
            transaction.CategoryId,
            (int)transaction.Type,
            transaction.Amount,
            transaction.Description,
            transaction.Date));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _deleteTransactionHandler.HandleAsync(id, cancellationToken);
        return NoContent();
    }
}
