using System.Security.Cryptography;
using System.Text;

namespace OnlineMarket.Domain.Helpers
{
    public static class Sha256Helper
    {
        public static string GetHash(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                    builder.Append(hashedBytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
