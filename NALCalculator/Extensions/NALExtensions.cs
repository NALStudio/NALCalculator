using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;
using System.Drawing;

public static class StringExtensions
{
    /// <summary>
    /// Capitalizes the string.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Capitalize(this string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    /// <summary>
    /// Capitalizes the string.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="lower">If true; will lower all other characters</param>
    /// <returns></returns>
    public static string Capitalize(this string str, bool lower)
    {
        return Capitalize(lower ? str.ToLower() : str);
    }

    /// <summary>
    /// If the string ends with the suffix string, return the string with the end suffix removed. Otherwise, return the original string.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static string RemoveSuffix(this string str, string suffix)
    {
        if (str.EndsWith(suffix))
            return str.Substring(0, str.Length - suffix.Length);
        return str;
    }

    /// <summary>
    /// If the string starts with the prefix string, return the string with the start prefix removed. Otherwise, return the original string.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static string RemovePrefix(this string str, string prefix)
    {
        if (str.StartsWith(prefix))
            return str.Substring(prefix.Length);
        return str;
    }

    /// <remarks>
    /// Note that this is based on Sten Hjelmqvist'str "Fast, memory efficient" algorithm, described
    /// <see href="https://www.codeproject.com/Articles/13525/Fast-memory-efficient-Levenshtein-algorithm-2">here</see>.
    /// This version differs by including some optimizations, and extending it to the Damerau-Levenshtein algorithm.
    /// Note that this is the simpler and faster optimal string alignment (aka restricted edit) distance
    /// that difers slightly from the classic Damerau-Levenshtein algorithm by imposing the restriction
    /// that no substring is edited more than once. So for example, "CA" to "ABC" has an edit distance
    /// of 2 by a complete application of Damerau-Levenshtein, but a distance of 3 by this method that
    /// uses the optimal string alignment algorithm. See <see href="https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance">wikipedia article</see>
    /// for more detail on this distinction.
    /// </remarks>
    /// <summary>Computes the Damerau-Levenshtein Distance between two strings</summary>
    /// <param name="str">String being compared for distance.</param>
    /// <param name="value">String being compared against other string.</param>
    /// <param name="maxDistance">The maximum edit distance of interest.</param>
    /// <returns>int edit distance, >= 0 representing the number of edits required
    /// to transform one string to the other, or -1 if the distance is greater than the specified maxDistance.</returns>
    public static int DamerauLevenshteinDistance(this string str, string value, int maxDistance = int.MaxValue)
    {
        if (string.IsNullOrEmpty(str))
            return ((value ?? "").Length <= maxDistance) ? (value ?? "").Length : -1;
        if (string.IsNullOrEmpty(value))
            return (str.Length <= maxDistance) ? str.Length : -1;

        // if strings of different lengths, ensure shorter string is in str. This can result in a little
        // faster speed by spending more time spinning just the inner loop during the main processing.
        if (str.Length > value.Length)
        {
            var temp = str; str = value; value = temp; // swap str and value
        }
        int sLen = str.Length; // this is also the minimun length of the two strings
        int tLen = value.Length;

        // suffix common to both strings can be ignored
        while ((sLen > 0) && (str[sLen - 1] == value[tLen - 1])) { sLen--; tLen--; }

        int start = 0;
        if ((str[0] == value[0]) || (sLen == 0))
        { // if there'str a shared prefix, or all str matches value'str suffix
            // prefix common to both strings can be ignored
            while ((start < sLen) && (str[start] == value[start])) start++;
            sLen -= start; // length of the part excluding common prefix and suffix
            tLen -= start;

            // if all of shorter string matches prefix and/or suffix of longer string, then
            // edit distance is just the delete of additional characters present in longer string
            if (sLen == 0)
                return (tLen <= maxDistance) ? tLen : -1;

            value = value.Substring(start, tLen); // faster than value[start+j] in inner loop below
        }
        int lenDiff = tLen - sLen;
        if ((maxDistance < 0) || (maxDistance > tLen))
			maxDistance = tLen;
		else if (lenDiff > maxDistance)
            return -1;

        var v0 = new int[tLen];
        var v2 = new int[tLen]; // stores one level further back (offset by +1 position)
        int j;
        for (j = 0; j < maxDistance; j++) v0[j] = j + 1;
        for (; j < tLen; j++) v0[j] = maxDistance + 1;

        int jStartOffset = maxDistance - (tLen - sLen);
        bool haveMax = maxDistance < tLen;
        int jStart = 0;
        int jEnd = maxDistance;
        char sChar = str[0];
        int current = 0;
        for (int i = 0; i < sLen; i++)
        {
            char prevsChar = sChar;
            sChar = str[start + i];
            char tChar = value[0];
            int left = i;
            current = left + 1;
            int nextTransCost = 0;
            // no need to look beyond window of lower right diagonal - maxDistance cells (lower right diag is i - lenDiff)
            // and the upper left diagonal + maxDistance cells (upper left is i)
            jStart += (i > jStartOffset) ? 1 : 0;
            jEnd += (jEnd < tLen) ? 1 : 0;
            for (j = jStart; j < jEnd; j++)
            {
                int above = current;
                int thisTransCost = nextTransCost;
                nextTransCost = v2[j];
                v2[j] = current = left; // cost of diagonal (substitution)
                left = v0[j];    // left now equals current cost (which will be diagonal at next iteration)
                char prevtChar = tChar;
                tChar = value[j];
                if (sChar != tChar)
                {
                    if (left < current) current = left;   // insertion
                    if (above < current) current = above; // deletion
                    current++;
                    if ((i != 0) && (j != 0)
                        && (sChar == prevtChar)
                        && (prevsChar == tChar))
                    {
                        thisTransCost++;
                        if (thisTransCost < current) current = thisTransCost; // transposition
                    }
                }
                v0[j] = current;
            }
            if (haveMax && (v0[i + lenDiff] > maxDistance)) return -1;
        }
        return (current <= maxDistance) ? current : -1;
    }
}

public static class NumberExtensions
{
    public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return ((value - fromMin) / (fromMax - fromMin) * (toMax - toMin)) + toMin;
    }

    public static double Remap(this double value, double fromMin, double fromMax, double toMin, double toMax)
	{
        return ((value - fromMin) / (fromMax - fromMin) * (toMax - toMin)) + toMin;
	}
}