using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.Products;

public record GetProductResponse(
    ProductDto Product , 
    double BuyPrice, 
    double SellPrice,
    double BuyPriceMaterials,
    double SellPriceMaterials
    ) ;