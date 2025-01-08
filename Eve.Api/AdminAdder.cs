using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Eve.Api;

public class AdminAdder
{
    private readonly UserManager<IdentityUser> _manager;
    private readonly AppIdentityDbContext _context;
    private readonly IConfiguration _config;

    public AdminAdder(UserManager<IdentityUser> manager, AppIdentityDbContext context, IConfiguration config)
    {
        _manager = manager;
        _context = context;
        _config = config;
    }

    public async Task CreateAdmin()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.MigrateAsync();

        var user = new IdentityUser
        {
            UserName = _config["Admin:Name"],
        };
        var password = _config["Admin:Password"];
        var result = await _manager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception("not created User");

    }
}
