using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mobiray.Common;
using Mobiray.Numbers;
using UnityEngine;

namespace Mobiray.Numbers
{
    public class NumericalFormatter
    {
        public enum Type
        {
            DEFAULT,
            WITHOUT_DECIMALS,
            SYMBOLS4
        }

        public Type FormatType = Type.SYMBOLS4;

        private List<string> hackedValues = new List<string>();
        
        public string Format(long number)
        {
            return FormatNumberString(number.ToString());
        }

        public string Format(System.Numerics.BigInteger number)
        {
            return FormatNumberString(number.ToString());
        }

        /*public string FormatPrice(System.Numerics.BigInteger number)
        {
            return "$" + Format(number);
        }*/

        private string FormatNumberString(string number)
        {
            if (number.Length < 5)
            {
                return number;
            }

            if (number.Length < 7)
            {
                return FormatThousands(number);
            }

            return FormatGeneral(number);
        }

        private string FormatThousands(string number)
        {
            string leadingNumbers = number.Substring(0, number.Length - 3);
            string decimals = number.Substring(number.Length - 3);

            return CreateNumericalFormat(leadingNumbers, decimals, "K");
        }

        private string FormatGeneral(string number)
        {
            int amountOfLeadingNumbers = (number.Length - 7) % 3 + 1;
            string leadingNumbers = number.Substring(0, amountOfLeadingNumbers);
            string decimals = number.Substring(amountOfLeadingNumbers, 3);

            return CreateNumericalFormat(leadingNumbers, decimals, GetSuffixForNumber(number));
        }

        private string CreateNumericalFormat(string leadingNumbers, string decimals, string suffix)
        {
            if (FormatType == Type.WITHOUT_DECIMALS)
            {
                return string.Format("{0}{1}", leadingNumbers, suffix);
            }

            switch (FormatType)
            {
                case Type.WITHOUT_DECIMALS:
                    return $"{leadingNumbers}{suffix}";

                case Type.SYMBOLS4:
                    return $"{leadingNumbers}.{decimals}".Substring(0, 5) + suffix;
            }

            return string.Format("{0}.{1}{2}", leadingNumbers, decimals, suffix);
        }

        private string GetSuffixForNumber(string number)
        {
            int numberOfThousands = (number.Length - 1) / 3;

            switch (numberOfThousands)
            {
                case 1:
                    return "K";
                case 2:
                    return "M";
                case 3:
                    return "B";
                case 4:
                    return "T";
                case 5:
                    return "Q";
                default:
                    return GetProceduralSuffix(numberOfThousands - 5);
            }
        }

        private string GetProceduralSuffixFirst(int value)
        {
            StringBuilder sb = new StringBuilder();

            while (value > 0)
            {
                int digit = value % 26;

                sb.Append((char) ('a' + digit));
                value /= 26;
            }

            if (sb.Length == 0)
            {
                sb.Append('a');
            }

            sb.Reverse();
            return sb.ToString();
        }

        private void PrecomputeValues()
        {
            // 531441 = 27 ^ 4, i.e. the first 5-digit base 27 number.
            // That's a large enough number to ensure that the output
            // include "aaaz", and indeed almost all of the 4-digit
            // base 27 numbers
            for (int i = 0; i < 531441; i++)
            {
                string text = ToBase27AlphaString(i);

                if (!text.Contains("`"))
                {
                    hackedValues.Add(text);
                }
            }
        }

        private string GetProceduralSuffix(int value)
        {
            if (hackedValues.Count == 0)
            {
                PrecomputeValues();
            }

            return hackedValues[value];
        }

        private string ToBase27AlphaString(int value)
        {
            StringBuilder sb = new StringBuilder();

            while (value > 0)
            {
                int digit = value % 27;

                sb.Append((char) ('`' + digit));
                value /= 27;
            }

            if (sb.Length == 0)
            {
                sb.Append('`');
            }

            sb.Reverse();
            return sb.ToString();
        }
    }
}