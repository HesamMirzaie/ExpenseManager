namespace ExpenseManager.Application.Transactions.DeleteTransaction;

public sealed class TransactionNotFoundException : Exception
{
    public TransactionNotFoundException(Guid transactionId)
        : base($"Transaction '{transactionId}' was not found.")
    {
    }
}
