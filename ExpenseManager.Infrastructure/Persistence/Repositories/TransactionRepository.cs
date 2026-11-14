
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;

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
}
