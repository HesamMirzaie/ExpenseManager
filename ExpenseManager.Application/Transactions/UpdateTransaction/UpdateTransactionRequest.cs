namespace ExpenseManager.Application.Transactions.UpdateTransaction;

public sealed record UpdateTransactionRequest(
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateOnly Date
);
