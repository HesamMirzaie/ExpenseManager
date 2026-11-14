
namespace ExpenseManager.API.Contracts.Transactions;

public sealed record CreateTransactionRequest(
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateOnly Date
);
