namespace Eve.Tests.UnitTests.InfrastructureTests.RepositoriesTests.Common;

public abstract class BaseReadRepositoryTest<TRepository> : RepositoryTestBase<TRepository> 
    where TRepository : class
{
    protected override async Task SeedCommonDataAsync()
    {
        var storage = new DataStorage();

        var types = storage.GetTypes();
        var materials = storage.GetReprocessMaterials();
        var categories = storage.GetCategories();
        var groups = storage.GetGroups();
        var marketGroups = storage.GetMarketGroups();
        var productsMaterials = storage.GetProductMaterials();
        var products = storage.GetProducts();
        var regions = storage.GetRegions();
        var stations = storage.GetStation();

        await Context.Types.AddRangeAsync(types);
        await Context.ReprocessMaterials.AddRangeAsync(materials);
        await Context.Categories.AddRangeAsync(categories);
        await Context.Groups.AddRangeAsync(groups);
        await Context.MarketGroups.AddRangeAsync(marketGroups);
        await Context.ProductMaterials.AddRangeAsync(productsMaterials);
        await Context.Products.AddRangeAsync(products);
        await Context.Regions.AddRangeAsync(regions);
        await Context.Stations.AddRangeAsync(stations);

        await Context.SaveChangesAsync();
    }
}
