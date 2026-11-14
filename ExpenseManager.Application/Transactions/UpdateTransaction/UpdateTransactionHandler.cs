using ExpenseManager.Application.Interfaces;
using ExpenseManager.Application.Transactions.CreateTransaction;
using ExpenseManager.Domain.Entities;

namespace ExpenseManager.Application.Transactions.UpdateTransaction;

public sealed class UpdateTransactionHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTransactionHandler(
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Transaction> HandleAsync(
        Guid id,
        UpdateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);

        if (transaction is null)
        {
            throw new TransactionNotFoundException(id);
        }

        var category = await _categoryRepository.GetByIdAsync(
            request.CategoryId,
            cancellationToken);

        if (category is null)
        {
            throw new CategoryNotFoundException(request.CategoryId);
        }

        transaction.Update(
            category.Id,
            category.Type,
            request.Amount,
            request.Description,
            request.Date);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return transaction;
    }
}
