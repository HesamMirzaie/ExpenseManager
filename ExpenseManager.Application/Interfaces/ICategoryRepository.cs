using ExpenseManager.Domain.Entities;

namespace ExpenseManager.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken cancellationToken = default);

    void Add(Category category);
}