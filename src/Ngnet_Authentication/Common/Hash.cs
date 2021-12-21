﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public static class Hash
    {
        public static string CreatePassword(string password)
        {
            byte[] data = Encoding.UTF8.GetBytes(password);

            using (SHA512 shaM = new SHA512Managed())
            {
                byte[] hash = shaM.ComputeHash(data);
                string result = BitConverter.ToString(hash).Replace("-", "");

                return result; 
            }
        }
    }
}
