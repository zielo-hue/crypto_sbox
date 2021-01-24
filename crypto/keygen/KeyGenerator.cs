using System.Collections.Generic;

namespace crypto.keygen
{
    public class KeyGenerator
    {
        private uint n, p, q, e, d;

        public KeyGenerator(uint p, uint q)
        {
            this.p = p;
            this.q = q;
            n = p * q;

            e = 17;
            // d = ModInverse(e, phi());
            d = 413;
        }

        /*uint ModInverse(uint e, uint phi)
        {
            List<uint> q, r;
            while (r != 1)
            {
                e = 
            }
        }*/

        public uint[] PublicKey()
            => new[] {n, e};

        public uint[] PrivateKey()
            => new[] {n, d};

        public uint phi(uint p, uint q) => lcm(p - 1, q - 1);
        public uint phi() => lcm(p - 1, q - 1);

        uint gcd(uint a, uint b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }

        uint lcm(uint a, uint b) => (a * b) / gcd(a, b);
    }
}