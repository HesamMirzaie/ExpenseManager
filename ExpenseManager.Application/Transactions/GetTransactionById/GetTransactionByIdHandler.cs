using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;

namespace ExpenseManager.Application.Transactions.GetTransactionById;

public sealed class GetTransactionByIdHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionByIdHandler(
        ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public Task<Transaction?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _transactionRepository.GetByIdAsync(
            id,
            cancellationToken);
    }
}