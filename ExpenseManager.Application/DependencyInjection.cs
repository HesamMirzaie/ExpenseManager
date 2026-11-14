using ExpenseManager.Application.Categories.CreateCategory;
using ExpenseManager.Application.Transactions.CreateTransaction;
using Microsoft.Extensions.DependencyInjection;
using ExpenseManager.Application.Categories.GetCategories;
using ExpenseManager.Application.Categories.GetCategoryById;
using ExpenseManager.Application.Transactions.GetTransactions;
using ExpenseManager.Application.Transactions.GetTransactionById;
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

        return services;
    }
}