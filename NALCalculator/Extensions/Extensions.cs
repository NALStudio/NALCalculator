using NALCalculator.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NALCalculator.Extensions
{
    public static class Extensions
    {
        static string InternalToString(BigRational r, int precision)
        {
            var fraction = r.GetFractionPart();

            // Case where the rational number is a whole number
            // Niko: Incorrect check? Fuck it... This works...
            if (fraction.Numerator == 0 && fraction.Denominator == 1)
                return r.GetWholePart().ToString();

            BigInteger adjustedNumerator = fraction.Numerator * BigInteger.Pow(10, precision);
                                        // Abs fixes decimals having minuses in front of them
            BigInteger decimalPlaces = BigInteger.Abs(adjustedNumerator / fraction.Denominator);

            // Case where precision wasn't large enough.
            if (decimalPlaces == 0)
                return null;

            // Give it the capacity for around what we should need for 
            // the whole part and total precision
            // (this is kinda sloppy, but does the trick)
            var sb = new StringBuilder(precision + r.ToString().Length);

            bool noMoreTrailingZeros = false;
            for (int i = precision; i > 0; i--)
            {
                if (!noMoreTrailingZeros)
                {
                    if ((decimalPlaces % 10) == 0)
                    {
                        decimalPlaces /= 10;
                        continue;
                    }

                    noMoreTrailingZeros = true;
                }

                // Add the right most decimal to the string
                sb.Insert(0, decimalPlaces % 10);
                decimalPlaces /= 10;
            }

            // Insert the whole part and decimal
            sb.Insert(0, ",");
            sb.Insert(0, r.GetWholePart());

            return sb.ToString();
        }

        public static string ToString(this BigRational r, int precision)
        {
            string output = InternalToString(r, precision);
            if (!string.IsNullOrEmpty(output))
                return output;
            return "0,0";
        }

        public static string ToString(this BigRational r, bool allowRational, ushort precisionLimit)
        {
            string output = InternalToString(r, precisionLimit + (allowRational ? 1 : 0));
            if (!allowRational)
                return output;
            if (string.IsNullOrEmpty(output) || (output.Length > precisionLimit && !r.Denominator.IsOne))
                return r.ToString();
            return output;
        }

        public static string AddSpacesToNumber(string number)
        {
            string[] calcSplit = number.Split(',');

            int iterStart = calcSplit[0].Length - 3;
            int iterEnd = 0;
            if (calcSplit[0].EndsWith('%'))
                iterStart--;
            if (calcSplit[0].StartsWith('-'))
                iterEnd++;

            for (int i = iterStart; i > iterEnd; i -= 3)
                calcSplit[0] = calcSplit[0].Insert(i, " ");

            return string.Join(',', calcSplit);
        }
    }
}
