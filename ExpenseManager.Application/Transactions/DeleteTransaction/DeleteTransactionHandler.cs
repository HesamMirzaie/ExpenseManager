using ExpenseManager.Application.Interfaces;

namespace ExpenseManager.Application.Transactions.DeleteTransaction;

public sealed class DeleteTransactionHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTransactionHandler(
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);

        if (transaction is null)
        {
            throw new TransactionNotFoundException(id);
        }

        _transactionRepository.Remove(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
