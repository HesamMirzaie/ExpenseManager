
using ExpenseManager.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.Infrastructure.Tests.Fixtures;

public sealed class SqliteTestFixture : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ExpenseManagerDbContext> _options;

    public ExpenseManagerDbContext DbContext { get; }

    public SqliteTestFixture()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<ExpenseManagerDbContext>()
            .UseSqlite(_connection)
            .Options;

        DbContext = new ExpenseManagerDbContext(_options);

        DbContext.Database.EnsureCreated();
    }

    public ExpenseManagerDbContext CreateDbContext()
    {
        return new ExpenseManagerDbContext(_options);
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
