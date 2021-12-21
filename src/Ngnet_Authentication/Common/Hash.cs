using System;
using System.Security.Cryptography;

namespace Common
{
    public static class Hash
    {
        public static string CreatePassword(string password)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, Global.HashBytes))
            {
                byte[] salt = deriveBytes.Salt;
                byte[] key = deriveBytes.GetBytes(Global.HashBytes);

                return BitConverter.ToString(salt) + BitConverter.ToString(key);
            }
        }
    }
}
