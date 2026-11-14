using ExpenseManager.Domain.Enums;

namespace ExpenseManager.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; }

    public TransactionType Type { get; private set; }

    public Category(string name, TransactionType type)
    {
        ValidateName(name);
        ValidateType(type);

        Name = name.Trim();
        Type = type;
    }

    public void ChangeName(string newName)
    {
        ValidateName(newName);
        Name = newName.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Category name cannot be empty.",
                nameof(name));
    }

    private static void ValidateType(TransactionType type)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type));
    }
}