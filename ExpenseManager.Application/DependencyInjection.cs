using ExpenseManager.Application.Categories.CreateCategory;
using ExpenseManager.Application.Transactions.CreateTransaction;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CreateTransactionHandler>();
        services.AddScoped<CreateCategoryHandler>();

        return services;
    }
}