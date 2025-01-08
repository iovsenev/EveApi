using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.Types.GetTypesSearch;
public record GetTypesSearchRequest(string Query) : IRequest;