
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ExpenseManagerDbContext _dbContext;

    public CategoryRepository(ExpenseManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Category?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories
            .FirstOrDefaultAsync(
                category => category.Id == id,
                cancellationToken);
    }
    public void Add(Category category)
    {
        _dbContext.Categories.Add(category);
    }
    public async Task<IReadOnlyList<Category>> GetAllAsync(
    CancellationToken cancellationToken = default)
{
    return await _dbContext.Categories
        .AsNoTracking()
        .OrderBy(category => category.Name)
        .ToListAsync(cancellationToken);
}
}
