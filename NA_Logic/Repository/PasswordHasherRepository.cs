using System.Security.Cryptography;
using System.Text;
using System.Linq;
using NA_Logic.IRepository;

namespace NA_Logic.Repository
{
    public class PasswordHasherRepository : IPasswordHasherRepository
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32;  // 256 bit
        private const int Iterations = 10000;

        public string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256).GetBytes(KeySize);

            var hashBytes = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(key, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            try
            {
                var hashBytes = Convert.FromBase64String(hashedPassword);
                var salt = new byte[SaltSize];
                Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);
                var key = new byte[KeySize];
                Buffer.BlockCopy(hashBytes, SaltSize, key, 0, KeySize);

                var providedKey = new Rfc2898DeriveBytes(providedPassword, salt, Iterations, HashAlgorithmName.SHA256).GetBytes(KeySize);

                return key.SequenceEqual(providedKey);
            }
            catch
            {
                return false;
            }
        }
    }

}
