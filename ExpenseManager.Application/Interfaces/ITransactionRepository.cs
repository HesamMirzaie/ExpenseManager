using ExpenseManager.Domain.Entities;

namespace ExpenseManager.Application.Interfaces;

public interface ITransactionRepository
{
    void Add(Transaction transaction);
}