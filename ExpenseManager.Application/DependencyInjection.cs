using ExpenseManager.Application.Categories.CreateCategory;
using ExpenseManager.Application.Transactions.CreateTransaction;
using Microsoft.Extensions.DependencyInjection;
using ExpenseManager.Application.Categories.GetCategories;
using ExpenseManager.Application.Categories.GetCategoryById;
using ExpenseManager.Application.Transactions.GetTransactions;
using ExpenseManager.Application.Transactions.GetTransactionById;
using ExpenseManager.Application.Transactions.UpdateTransaction;
using ExpenseManager.Application.Transactions.DeleteTransaction;
namespace ExpenseManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CreateTransactionHandler>();
        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<GetCategoriesHandler>();
        services.AddScoped<GetCategoryByIdHandler>();
        services.AddScoped<GetTransactionsHandler>();
        services.AddScoped<GetTransactionByIdHandler>();
        services.AddScoped<UpdateTransactionHandler>();
        services.AddScoped<DeleteTransactionHandler>();

        return services;
    }
}
