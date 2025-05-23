﻿using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Repositories.Read;

public class ReadGroupRepository : IReadGroupRepository
{
    private readonly IAppDbContext _appDbContext;

    public ReadGroupRepository(IAppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Result<ICollection<GroupEntity>>> GetGroupsForCategoryIdWithProducts(int id, CancellationToken token)
    {
        var groups = await _appDbContext.Groups
            .AsNoTracking()
            .Include(g => g.Types)
            .Where(g => g.CategoryId == id && g.Types.Any(t => t.IsProduct && t.Published))
            .ToListAsync();

        if (groups is null || !groups.Any())
            return Error.NotFound($"Not found groups for category id = {id} with products");

        return groups;
    }
}
