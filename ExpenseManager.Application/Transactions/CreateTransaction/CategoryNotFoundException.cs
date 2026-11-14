namespace ExpenseManager.Application.Transactions.CreateTransaction;

public sealed class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(Guid categoryId)
        : base($"Category with ID '{categoryId}' was not found.")
    {
    }
}