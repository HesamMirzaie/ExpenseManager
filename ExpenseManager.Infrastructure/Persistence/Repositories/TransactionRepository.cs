
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Application.Common;
using ExpenseManager.Application.Transactions.GetTransactions;
using ExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.Infrastructure.Persistence.Repositories;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly ExpenseManagerDbContext _dbContext;

    public TransactionRepository(ExpenseManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Transaction transaction)
    {
        _dbContext.Transactions.Add(transaction);
    }
    public async Task<PagedResult<Transaction>> GetAllAsync(
    GetTransactionsQuery query,
    CancellationToken cancellationToken = default)
{
    IQueryable<Transaction> transactions = _dbContext.Transactions
        .AsNoTracking();

    if (query.CategoryId.HasValue)
    {
        transactions = transactions.Where(
            transaction => transaction.CategoryId == query.CategoryId.Value);
    }

    if (query.Type.HasValue)
    {
        transactions = transactions.Where(
            transaction => transaction.Type == query.Type.Value);
    }

    if (query.StartDate.HasValue)
    {
        transactions = transactions.Where(
            transaction => transaction.Date >= query.StartDate.Value);
    }

    if (query.EndDate.HasValue)
    {
        transactions = transactions.Where(
            transaction => transaction.Date <= query.EndDate.Value);
    }

    // Count before pagination
    var totalCount = await transactions.CountAsync(cancellationToken);

    // Stable ordering
    var items = await transactions
        .OrderByDescending(transaction => transaction.Date)
        .ThenByDescending(transaction => transaction.Id)
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .ToListAsync(cancellationToken);

    return new PagedResult<Transaction>(
        items,
        totalCount,
        query.Page,
        query.PageSize);
}
public Task<Transaction?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
{
    return _dbContext.Transactions
        .AsNoTracking()
        .FirstOrDefaultAsync(
            transaction => transaction.Id == id,
            cancellationToken);
}
}
