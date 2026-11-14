
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;

namespace ExpenseManager.Application.Categories.CreateCategory;

public sealed class CreateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Category> HandleAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Create the Domain entity.
        var category = new Category(
            request.Name,
            request.Type);

        // 2. Stage the entity for persistence.
        _categoryRepository.Add(category);

        // 3. Commit changes to the database.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category;
    }
}
