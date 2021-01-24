using System;
using crypto.keygen;

namespace crypto
{
    class Program
    {
        static void Main(string[] args)
        {
            KeyGenerator keygen = new KeyGenerator(61, 53);
            /*Console.WriteLine(keygen.phi());
            Console.WriteLine(keygen.PublicKey()[0]);
            Console.WriteLine(keygen.PublicKey()[1]);
            Console.WriteLine(keygen.PrivateKey()[0]);
            Console.WriteLine(keygen.PrivateKey()[1]);*/
            
            ModInverse mi = new ModInverse();
            Console.WriteLine(mi.ModInv(3000, 197)); // 533
            Console.WriteLine(mi.ModInv(780, 17));   // 413
        }
    }
}