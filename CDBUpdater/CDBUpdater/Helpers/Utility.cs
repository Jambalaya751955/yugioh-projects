using System;
using System.Web;
using System.Linq;
using System.Text.RegularExpressions;

namespace CDBUpdater.Helpers
{
    static class Utility
    {
        public static bool IsEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }

        public static string UriEscape(this string str)
        {
            if (Utility.IsEmpty(str)) return "";
            return Uri.EscapeDataString(str);
        }

        public static string UrlDecode(this string str)
        {
            if (Utility.IsEmpty(str)) return "";
            return HttpUtility.UrlDecode(str);
        }

        public static string HtmlDecode(this string str)
        {
            if (Utility.IsEmpty(str)) return "";
            return HttpUtility.HtmlDecode(str);
        }

        public static string RemoveWhitespace(this string str)
        {
            if (str.IsEmpty())
                return "";
            return new string(str.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }

        public static string RemoveBetween(this string str, char begin, char end)
        {
            if (Utility.IsEmpty(str)) return "";
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(str, string.Empty);
        }

        public static string GetBetween(this string str, string begin, string end)
        {
            if (str.IsEmpty() || begin.IsEmpty() || end.IsEmpty()) return "";

            int posA = str.IndexOf(begin);
            int posB = str.LastIndexOf(end);
            if (posA == -1 || posB == -1)
                return "";

            int adjustedPosA = posA + begin.Length;
            if (adjustedPosA >= posB)
                return "";

            return str.Substring(adjustedPosA, posB - adjustedPosA);
        }
    }
}
