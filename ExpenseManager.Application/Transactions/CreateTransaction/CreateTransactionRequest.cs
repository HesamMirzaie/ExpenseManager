namespace ExpenseManager.Application.Transactions.CreateTransaction;

public sealed record CreateTransactionRequest(
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateOnly Date
);