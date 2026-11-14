
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.Infrastructure.Persistence;

public sealed class ExpenseManagerDbContext
    : DbContext, IUnitOfWork
{
    public ExpenseManagerDbContext(
        DbContextOptions<ExpenseManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ExpenseManagerDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
