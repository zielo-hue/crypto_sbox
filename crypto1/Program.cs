using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;

namespace crypto1
{
    class Program
    {
        static void Main(string[] args)
        {
            // import/export the keys..
            var rsa = RSA.Create(2048);
            var text = Encoding.ASCII.GetBytes("abc");

            var encrypted = rsa.Encrypt(text, RSAEncryptionPadding.Pkcs1);
            Console.WriteLine(Convert.ToBase64String(encrypted));
            
            File.WriteAllBytes("thing.pub", rsa.ExportRSAPublicKey());
            File.WriteAllBytes("output", encrypted);
            
            var input = File.ReadAllBytes("output");
            var decrypted = rsa.Decrypt(input, RSAEncryptionPadding.Pkcs1);
            Console.WriteLine(Encoding.ASCII.GetString(decrypted));
        }
    }
}