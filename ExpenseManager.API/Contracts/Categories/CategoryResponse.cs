namespace ExpenseManager.API.Contracts.Categories;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    int Type
);