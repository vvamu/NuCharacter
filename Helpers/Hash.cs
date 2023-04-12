using System;
using System.Security.Cryptography;
using System.Text;

namespace NuCharacter.Helpers
{
    internal static class Hash
    {
        //private static readonly string Key = "@;de/2d2]\\deqv09.";
        public static string Encrypt(string password)
        {
            if(password == null) return null;
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                //string passKey = string.Concat(password, Key);
                byte[] data = md5.ComputeHash(utf8.GetBytes(password));
                return Convert.ToBase64String(data);
            }
        }
    }
}
