using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EncrypDecryp
{
    public class EncryptorDecryptor
    {
        
    }

    interface IComparator
    {
        void PsswdCompare();
    }
    public class PsswdModel : IComparator
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string CodeEncode(string text, int k)
        {
            var fullAlphabet = alphabet + alphabet.ToLower();
            var letterQty = fullAlphabet.Length;
            var retVal = "";
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var index = fullAlphabet.IndexOf(c);
                if (index < 0)
                {
                    //если символ не найден, то добавляем его в неизменном виде
                    retVal += c.ToString();
                }
                else
                {
                    var codeIndex = (letterQty + index + k) % letterQty;
                    retVal += fullAlphabet[codeIndex];
                }
            }

            return retVal;
        }

        //шифрование текста
        public string Encrypt(string plainMessage, int key)
            => CodeEncode(plainMessage, key);

        //дешифрование текста
        public string Decrypt(string encryptedMessage, int key)
            => CodeEncode(encryptedMessage, -key);

        public void PsswdCompare()
        {
            string IsPsswdEqual = "pivo"; 
        }
    }
    class ProxyPsswd : IComparator
    {
        PsswdModel realOne;
        public void PsswdCompare()
        {
            if (realOne == null)
            {
                realOne = new PsswdModel();
            }
            realOne.PsswdCompare();

        }
    }
    class Client
    {
        public void zapros(IComparator subject)
        {
            subject.PsswdCompare();
        }
    }
}


