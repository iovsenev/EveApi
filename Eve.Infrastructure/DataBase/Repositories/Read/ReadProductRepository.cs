using Eve.Domain.Common;
using Eve.Domain.Entities.Products;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Eve.Infrastructure.DataBase.Repositories.Read;
public class ReadProductRepository : IReadProductRepository
{
    private readonly IAppDbContext _context;

    public ReadProductRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductEntity>> GetProductForId(int typeId, CancellationToken token)
    {
        var result = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == typeId);

        if (result is null) return Error.NotFound($"Product with type Id is not found for id: {typeId}");

        return result;
    }

    public async Task<Result<ICollection<ProductMaterialEntity>>> GetMaterialsForProductId(int productId, CancellationToken token)
    {
        var result = await _context.ProductMaterials
            .AsNoTracking()
            .Include(t=> t.Type)
            .Where(m => m.ProductId == productId)
            .ToListAsync();

        if (result is null || !result.Any()) 
            return Error.NotFound($"Not found materials for product with id: {productId}");

        return result;
    }
}
