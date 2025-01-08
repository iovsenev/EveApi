using Eve.Domain.Common;
using Eve.Domain.Entities.Products;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;
public interface IProductRepository
{
    Task<Result<ICollection<ProductMaterialEntity>>> GetMaterialsForProductId(int productId, CancellationToken token);
    Task<Result<ProductEntity>> GetProductForId(int typeId, CancellationToken token);
}