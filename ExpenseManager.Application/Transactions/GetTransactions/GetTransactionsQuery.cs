using ExpenseManager.Domain.Enums;

namespace ExpenseManager.Application.Transactions.GetTransactions;

public sealed record GetTransactionsQuery(
    Guid? CategoryId = null,
    TransactionType? Type = null,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null,
    int Page = 1,
    int PageSize = 20
);
