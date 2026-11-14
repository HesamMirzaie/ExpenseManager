namespace ExpenseManager.API.Contracts.Transactions;

public sealed record TransactionResponse(
    Guid Id,
    Guid CategoryId,
    int Type,
    decimal Amount,
    string Description,
    DateOnly Date
);