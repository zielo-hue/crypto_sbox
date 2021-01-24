using System.Collections.Generic;

namespace crypto.keygen
{
    public class ModInverse
    {
        public class Crt
        {
            public uint d, v, q, r;
            public Crt(uint d, uint v)
            {
                this.d = d;
                this.v = v;

                q = d / v;
                r = d % v;
            }

            public override string ToString()
                => $"d = {d}; v = {v}; q = {q}; r = {r}";
        }

        public class RCrt
        {
            public uint c1, c2, g1, g2;

            public RCrt(Crt op)
            {
                c1 = 1;
                g1 = op.d;
                c2 = op.q;
                g2 = op.v;
            }

            public void Step(Crt op)
            {
                // r = d - q(v)
                // figure out if g1 or g2 is op.r
                
                g2 = op.v;
                // 1. c1, g1, c2, g2 = 1, 6, 1, 5
                //  Step: d, v, q, r = 11, 6, 1, 5
                // 2. c1, g1, c2, g2 = 2, 6, 1, 11
                //  Step: d, v, q, r = 17, 11, 1, 6
                // 3. c1, g1, c2, g2 = 2, 17, 3, 11
                //  Step: d, v, q, r = 45, 17, 2, 11
                // 4. c1, g1, c2, g2 = 8, 17, 3, 45
                //  Step: d, v, q, r = 197, 45, 4, 17
                // 5. c1, g1, c2, g2 = 8, 197, 35, 45
                //  Step: d, v, q, r = 3000, 197, 15, 45
                // 6. c1, g1, c2, g2 = 533, 197, 35, 3000
            }
        }

        public Stack<Crt> Stack = new Stack<Crt>();
        
        public uint ModInv(uint l, uint e)
        {
            // Step 1
            Crt init = new Crt(l, e);
            Stack.Push(init);
            while (init.r > 1)
            {
                init = new Crt(init.v, init.r);
                Stack.Push(init);
            }
            
            // Step 2
            // r = d - q(v)
            RCrt rcrt = new RCrt(Stack.Pop());
            while (Stack.Count > 0)
            {
                rcrt.Step(Stack.Pop());
            }

            return rcrt.g1 == e ? rcrt.c1 : rcrt.c2;
        }
    }
}