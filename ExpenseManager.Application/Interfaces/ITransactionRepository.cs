using ExpenseManager.Application.Common;
using ExpenseManager.Application.Transactions.GetTransactions;
using ExpenseManager.Domain.Entities;

namespace ExpenseManager.Application.Interfaces;

public interface ITransactionRepository
{
    void Add(Transaction transaction);

    Task<PagedResult<Transaction>> GetAllAsync(
        GetTransactionsQuery query,
        CancellationToken cancellationToken = default);

    Task<Transaction?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}