using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;

namespace ExpenseManager.Application.Categories.GetCategoryById;

public sealed class GetCategoryByIdHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<Category?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _categoryRepository.GetByIdAsync(id, cancellationToken);
    }
}