
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests.Common;

public abstract class RepositoryTestBase<TRepository> : IAsyncLifetime where TRepository : class
{
    protected AppDbContext Context { get; private set; }
    public TRepository Repository { get; private set; }

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"{typeof(TRepository).Name}_TestDb_{Guid.NewGuid()}")
            .Options;

        Context = new AppDbContext(options);
        await Context.Database.EnsureCreatedAsync();

        Repository = CreateRepository(Context);

        await SeedCommonDataAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.DisposeAsync();
    }

    protected abstract TRepository CreateRepository(IAppDbContext context);

    protected virtual async Task SeedCommonDataAsync() => await Task.CompletedTask;

}
