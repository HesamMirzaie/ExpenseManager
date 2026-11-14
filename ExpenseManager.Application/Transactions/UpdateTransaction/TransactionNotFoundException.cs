namespace ExpenseManager.Application.Transactions.UpdateTransaction;

public sealed class TransactionNotFoundException : Exception
{
    public TransactionNotFoundException(Guid transactionId)
        : base($"Transaction '{transactionId}' was not found.")
    {
    }
}
