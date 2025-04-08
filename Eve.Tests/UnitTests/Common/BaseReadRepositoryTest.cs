namespace Eve.Tests.UnitTests.Common;

public abstract class BaseReadRepositoryTest<TRepository> : RepositoryTestBase<TRepository> 
    where TRepository : class
{
    protected override async Task SeedCommonDataAsync()
    {
        var types = DataStorage.GetTypes();
        var materials = DataStorage.GetReprocessMaterials();
        var categories = DataStorage.GetCategories();
        var groups = DataStorage.GetGroups();
        var marketGroups = DataStorage.GetMarketGroups();
        var productsMaterials = DataStorage.GetProductMaterials();
        var products = DataStorage.GetProducts();
        var regions = DataStorage.GetRegions();
        var stations = DataStorage.GetStation();

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
