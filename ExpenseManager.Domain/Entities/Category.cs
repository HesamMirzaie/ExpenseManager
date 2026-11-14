using ExpenseManager.Domain.Enums;

namespace ExpenseManager.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    private string _name = string.Empty;
     public string Name
    {
        get => _name;
        set
        {
            ValidateName(value);
            _name = value.Trim();
        }
    }

    public TransactionType Type { get; private set; }

    public Category(string name, TransactionType type)
    {
        ValidateName(name);
        ValidateType(type);

        Name = name.Trim();
        Type = type;
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