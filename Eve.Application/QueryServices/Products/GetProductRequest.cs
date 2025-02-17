using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices.Products;

public record GetProductRequest(int TypeId, float BlueprintEff, float StructEff) : IRequest;