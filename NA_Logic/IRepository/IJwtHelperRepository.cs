using Microsoft.AspNetCore.Http;
using System.Security.Claims;
namespace NA_Logic.IRepository
{
    public interface IJwtHelperRepository
    {
        bool IsValidToken(string token);
        string GetTokenFromRequest(HttpRequest request);
        bool IsValidTokenFromRequest(HttpRequest request);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
        string GenerateJwtToken(string userId);
    }
}
