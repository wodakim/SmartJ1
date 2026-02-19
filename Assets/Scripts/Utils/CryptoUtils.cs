using System;
using System.Security.Cryptography;
using System.Text;

namespace EntropySyndicate.Utils
{
    public static class CryptoUtils
    {
        private const string Pepper = "EntropySyndicate::SigilPepper";

        public static string Protect(string plainJson)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(plainJson + Pepper);
            return Convert.ToBase64String(utf8);
        }

        public static string Unprotect(string encoded)
        {
            byte[] bytes = Convert.FromBase64String(encoded);
            string value = Encoding.UTF8.GetString(bytes);
            return value.Replace(Pepper, string.Empty);
        }

        public static string ComputeDigest(string content)
        {
            using SHA256 sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(content + Pepper);
            byte[] hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
