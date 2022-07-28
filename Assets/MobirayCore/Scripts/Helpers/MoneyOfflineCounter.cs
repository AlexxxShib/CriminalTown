using System.Numerics;
using UnityEngine;

namespace Mobiray.Helpers
{
    public class MoneyOfflineCounter
    {
        private BigInteger moneyPerMinute = 0;
        private BigInteger tempMoneyPerMinute = 0;
        private int minuteCounter = 0;

        public BigInteger MoneyPerMinute { get; private set; }

        public BigInteger TempMoneyPerMinute { get; private set; }

        public void AddMoneyPerMinute(BigInteger value,float time)
        {
            TempMoneyPerMinute += value;
            if (time > minuteCounter)
            {
                minuteCounter++;
                MoneyPerMinute = TempMoneyPerMinute;
                TempMoneyPerMinute = 0;
            }
        }
    }
}