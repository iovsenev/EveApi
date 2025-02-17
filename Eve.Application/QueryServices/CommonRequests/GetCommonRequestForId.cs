using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices.CommonRequests;
public record GetCommonRequestForId(int Id) : IRequest;
