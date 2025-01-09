using Microsoft.AspNetCore.Identity;

namespace Eve.Application.Services.AuthServices;
public interface ITokenService
{
    string GenerateJwtToken(IdentityUser user);
}