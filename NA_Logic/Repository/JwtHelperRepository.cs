using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NA_Logic.IRepository;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace NA_Logic.Repository
{
    public class JwtHelperRepository : IJwtHelperRepository
    {
        private readonly IConfiguration _config;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string[] _audiences;

        public JwtHelperRepository(IConfiguration config)
        {
            _config = config;

            var jwtSection = _config.GetSection("JwtSettings");
            _secretKey = jwtSection["SecretKey"]!;
            _issuer = jwtSection["Issuer"]!;
            _audiences = new[] { jwtSection["Audience"]! };
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của token JWT
        /// </summary>
        public bool IsValidToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                // Định nghĩa các thông số xác thực token
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidIssuer = _issuer,
                    //ValidAudiences = _audiences,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Không cho phép sai lệch thời gian (nghĩa là token hết hạn là không hợp lệ ngay lập tức)
                };

                // Kiểm tra và xác thực token
                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                // Nếu không có ngoại lệ, token hợp lệ
                return true;
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra, token không hợp lệ
                Console.WriteLine($"Token không hợp lệ: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy token từ header Authorization của request
        /// </summary>
        public string GetTokenFromRequest(HttpRequest request)
        {
            return request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của token từ request
        /// </summary>
        public bool IsValidTokenFromRequest(HttpRequest request)
        {
            var token = GetTokenFromRequest(request);
            return !string.IsNullOrEmpty(token) && IsValidToken(token);
        }

        // Hàm lấy giá trị ClaimsPrincipal trong token 
        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var parameters = new TokenValidationParameters
            {
                //ValidateIssuer = true,
                //ValidateAudience = true,
                //ValidIssuer = _issuer,
                //ValidAudiences = _audiences,
                //IssuerSigningKey = new SymmetricSecurityKey(key),
                //ValidateLifetime = true,
                //ClockSkew = TimeSpan.Zero
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidIssuer = _issuer,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, parameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string GenerateJwtToken(string userId, string Id_Donvi)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

            // Lấy thời gian hết hạn từ cấu hình (ExpireTime - tính bằng phút)
            int expireMinutes = int.Parse(jwtSettings["ExpireTime"]!);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(secretKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim("UserId", userId),
            new Claim("Id_Donvi", Id_Donvi)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
