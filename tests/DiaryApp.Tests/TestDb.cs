namespace DiaryApp.Tests;

using DiaryApp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public sealed class TestDb : IDisposable
{
    public DiaryDbContext Context { get; }
    private readonly SqliteConnection _connection;

    public TestDb()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<DiaryDbContext>()
            .UseSqlite(_connection)
            .Options;
        Context = new DiaryDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
