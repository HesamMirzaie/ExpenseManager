using ExpenseManager.Application.Common;
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;

namespace ExpenseManager.Application.Transactions.GetTransactions;

public sealed class GetTransactionsHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionsHandler(
        ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public Task<PagedResult<Transaction>> HandleAsync(
        GetTransactionsQuery query,
        CancellationToken cancellationToken = default)
    {
        ValidateQuery(query);

        return _transactionRepository.GetAllAsync(
            query,
            cancellationToken);
    }

    private static void ValidateQuery(GetTransactionsQuery query)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(query.Page, 1);

        ArgumentOutOfRangeException.ThrowIfLessThan(query.PageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(query.PageSize, 100);

        if (query.StartDate.HasValue &&
            query.EndDate.HasValue &&
            query.StartDate.Value > query.EndDate.Value)
        {
            throw new ArgumentException(
                "'startDate' cannot be later than 'endDate'.");
        }

        if (query.Type.HasValue &&
            !Enum.IsDefined(query.Type.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(query.Type));
        }
    }
}
