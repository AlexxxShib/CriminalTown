using System;
using System.Numerics;

namespace Mobiray.Data
{
    [Serializable]
    public class MoneyValue : IData
    {
        public Money Type;

        public long Base;

        public int Power;

        public BigInteger Value()
        {
            return new BigInteger(Base) * BigInteger.Pow(10, Power);
        }

        public long ValueLong()
        {
            return (long) (Base * Math.Pow(10, Power));
        }

        public int ValueInt()
        {
            return (int) (Base * Math.Pow(10, Power));
        }
    }
}