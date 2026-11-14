using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;

namespace ExpenseManager.Application.Transactions.CreateTransaction;

public sealed class CreateTransactionHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTransactionHandler(
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Transaction> HandleAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Find the category.
        var category = await _categoryRepository.GetByIdAsync(
            request.CategoryId,
            cancellationToken);

        // 2. Ensure that the category exists.
        if (category is null)
        {
            throw new CategoryNotFoundException(request.CategoryId);
        }

        // 3. Create the domain entity.
        // Type comes from the category, not from the client.
        var transaction = new Transaction(
            category.Id,
            category.Type,
            request.Amount,
            request.Description,
            request.Date);

        // 4. Stage the transaction for persistence.
        _transactionRepository.Add(
            transaction);

        // 5. Commit the operation.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return transaction;
    }
}