
using ExpenseManager.Domain.Enums;

namespace ExpenseManager.API.Contracts.Categories;

public sealed record CreateCategoryRequest(
    string Name,
    TransactionType Type
);
