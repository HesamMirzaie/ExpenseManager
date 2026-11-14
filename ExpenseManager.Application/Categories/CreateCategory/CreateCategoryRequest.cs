
using ExpenseManager.Domain.Enums;

namespace ExpenseManager.Application.Categories.CreateCategory;

public sealed record CreateCategoryRequest(
    string Name,
    TransactionType Type
);
