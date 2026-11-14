namespace ExpenseManager.API.Contracts.Transactions;

public sealed record UpdateTransactionRequest(
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateOnly Date
);
