using System;
using System.Text.RegularExpressions;

namespace SimServer.Helpers
{
    public static class Strings
    {
        /// <summary>
        /// Get text between two characters, index
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strStart"></param>
        /// <param name="strEnd"></param>
        /// <returns></returns>
        public static string GetTxtBetween(string strSource, string strStart, string strEnd)
        {
            if (!strSource.Contains(strStart) || !strSource.Contains(strEnd)) return "";
            var start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
            var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);
            return strSource.Substring(start, end - start);
        }

        /// <summary>
        /// Get text between two words, Regex
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strStart"></param>
        /// <param name="strEnd"></param>
        /// <returns></returns>
        public static string GetTextBetween(string strSource, string strStart, string strEnd)
        {
            return
                Regex.Match(strSource, $@"{strStart}\s(?<words>[\w\s]+)\s{strEnd}",
                    RegexOptions.IgnoreCase).Groups["words"].Value;
        }

        /// <summary>
        /// Pulls a number from a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int? GetNumberFromString(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var numbers = Regex.Split(str, @"\D+");
            foreach (var value in numbers)
            {
                if (string.IsNullOrEmpty(value)) continue;
                var ok = int.TryParse(value.Trim(), out var i);
                if (ok) { return i; }
            }
            return null;
        }

        /// <summary>
        /// Converts a Mount received hex string to type long
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long StringToLong(string str)
        {
            long value = 0;
            for (var i = 1; i + 1 < str.Length; i += 2)
            {
                value += (long)(int.Parse(str.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier) * Math.Pow(16, i - 1));
            }
            return value;
        }

    }
}
