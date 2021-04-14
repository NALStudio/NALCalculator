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
        public static string ToString(this BigRational r, int precision)
        {
            var fraction = r.GetFractionPart();

            // Case where the rational number is a whole number
            // Niko: Incorrect check? Fuck it... This works...
            if (fraction.Numerator == 0 && fraction.Denominator == 1)
				return r.GetWholePart().ToString();

			BigInteger adjustedNumerator = fraction.Numerator * BigInteger.Pow(10, precision);
            BigInteger decimalPlaces = adjustedNumerator / fraction.Denominator;

            // Case where precision wasn't large enough.
            if (decimalPlaces == 0)
				return "0,0";

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
                // Abs fixes decimals having minuses in front of them
                sb.Insert(0, BigInteger.Abs(decimalPlaces) % 10);
				decimalPlaces /= 10;
            }

            // Insert the whole part and decimal
            sb.Insert(0, ",");
            sb.Insert(0, r.GetWholePart());

            return sb.ToString();
        }

        public static string ToString(this BigRational r, bool allowRational, ushort precisionLimit)
		{
            string output = ToString(r, precisionLimit + (allowRational ? 1 : 0));
            if (!allowRational)
                return output;
            if (output.Length > precisionLimit && !r.Denominator.IsOne)
                return r.ToString();
            return output;
        }
    }
}
