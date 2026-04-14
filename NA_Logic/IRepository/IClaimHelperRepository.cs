using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NA_Entities.Entities.Auth;

namespace NA_Logic.IRepository
{
    public interface IClaimHelperRepository
    {
        int GetIdDonvi(ClaimsPrincipal user);
        int GetUserId(ClaimsPrincipal user);
        bool CheckIdDonvi(ClaimsPrincipal user);
        bool CheckUserExists(ClaimsPrincipal user);
        bool IsDemoSite();
        bool IsLiveSite();
    }
}