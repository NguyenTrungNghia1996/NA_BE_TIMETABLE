using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NA_Entities.DBContext;
using NA_Logic.IRepository;

namespace NA_Logic.Repository
{
    public class ClaimHelperRepository : IClaimHelperRepository
    {
        private readonly NA_DbContext _context;
        public ClaimHelperRepository(NA_DbContext context)
        {
            _context = context;
        }
        public int GetIdDonvi(ClaimsPrincipal user)
        {
            try
            {
                if (user != null)
                {
                    if (!CheckUserExists(user))
                    {
                        return 0;
                    }
                    var userId = GetUserId(user);
                    var dbUser = _context.Auth_Users.Find(userId);
                    if (dbUser == null)
                    {
                        return 0;
                    }
                    return dbUser.Id_Donvi;
                }
                return 0;
            }
            catch
            {
                return 0;
            }

        }

        public int GetUserId(ClaimsPrincipal user)
        {
            try
            {
                var claimValue = user.FindFirst("UserId")?.Value;
                return int.TryParse(claimValue, out int idUser) ? idUser : 0;
            }
            catch
            {
                return 0;
            }
        }

        public bool CheckUserExists(ClaimsPrincipal user)
        {
            try
            {
                int userId = GetUserId(user);
                if (userId <= 0)
                {
                    return false;
                }

                bool exists = _context.Auth_Users.Any(u => u.Id == userId);
                if (!exists)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIdDonvi(ClaimsPrincipal user)
        {
            try
            {
                int idDonvi = GetIdDonvi(user);
                if (idDonvi <= 0)
                {
                    return false;
                }

                bool exists = _context.DM_Donvi.Any(u => u.Id == idDonvi);
                if (!exists)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}