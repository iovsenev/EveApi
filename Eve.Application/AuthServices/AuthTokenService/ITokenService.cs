using Microsoft.AspNetCore.Identity;

namespace Eve.Application.AuthServices.AuthTokenService;
public interface ITokenService
{
    string GenerateJwtToken(IdentityUser user);
}