using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APS.Common.Cryptography
{
    public interface ICryptoLibrary
    {
        string Decrypt(string ciphertext, string key);
        string Encrypt(string plainText, string key);
    }
}
