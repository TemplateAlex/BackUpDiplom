using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EncryptorLib
{
    interface IEncryptor
    {
        string HashPassword(string password);
    }
    public class md5Real: IEncryptor
    {
        public string HashPassword(string password)
        {
            MD5 md5 = MD5.Create();
            byte[] b = Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(b);
            StringBuilder sb = new StringBuilder();
            foreach (var a in hash)
            {
                sb.Append(a.ToString("X2"));
            }
            return sb.ToString();
        }
    }
    public class md5Proxy: IEncryptor
    {
        private md5Real _md5real;
        public md5Proxy()
        {
            this._md5real = new md5Real();
        }
        public string HashPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                string patternIdIncoming = @"[0-9]+";
                Regex regex = new Regex(patternIdIncoming);
                Match match = regex.Match(password);
                if (password.Length >= 5 && match.Success)
                {
                    return this._md5real.HashPassword(password);
                }
            }
            return null;
        }
    }
}


