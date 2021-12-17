using System;
using System.Security.Cryptography;

namespace Common
{
    public static class Hash
    {
        public static string CreatePassword(string password)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, 20))
            {
                byte[] salt = deriveBytes.Salt;
                byte[] key = deriveBytes.GetBytes(20);  // derive a 20-byte key

                return BitConverter.ToString(salt) + BitConverter.ToString(key);
            }

        }
    }
}
