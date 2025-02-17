using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices.Types.GetTypesSearch;
public record GetTypesSearchRequest(string Query) : IRequest;