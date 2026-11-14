using ExpenseManager.Domain.Enums;

namespace ExpenseManager.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid CategoryId { get; private set; }

    public TransactionType Type { get; private set; }

    public decimal Amount { get; private set; }

    public string Description { get; private set; }

    public DateOnly Date { get; private set; }

    public Transaction(
        Guid categoryId,
        TransactionType type,
        decimal amount,
        string description,
        DateOnly date)
    {
        ValidateCategoryId(categoryId);
        ValidateType(type);
        ValidateAmount(amount);
        ValidateDescription(description);

        CategoryId = categoryId;
        Type = type;
        Amount = amount;
        Description = description.Trim();
        Date = date;
    }

    public void ChangeAmount(decimal newAmount)
    {
        ValidateAmount(newAmount);
        Amount = newAmount;
    }

    public void ChangeDescription(string newDescription)
    {
        ValidateDescription(newDescription);
        Description = newDescription.Trim();
    }

    public void Update(
        Guid categoryId,
        TransactionType type,
        decimal amount,
        string description,
        DateOnly date)
    {
        ValidateCategoryId(categoryId);
        ValidateType(type);
        ValidateAmount(amount);
        ValidateDescription(description);

        CategoryId = categoryId;
        Type = type;
        Amount = amount;
        Description = description.Trim();
        Date = date;
    }

    private static void ValidateCategoryId(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException(
                "Category ID cannot be empty.",
                nameof(categoryId));
    }

    private static void ValidateType(TransactionType type)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type));
    }

    private static void ValidateAmount(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Description cannot be empty.",
                nameof(description));
    }
}
