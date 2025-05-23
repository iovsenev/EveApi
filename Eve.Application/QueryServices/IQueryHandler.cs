﻿using Eve.Domain.Common;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices;
public interface IQueryHandler
{
    Task<Result<TResponse>> Send<TResponse>(IRequest request, CancellationToken token = default);
}