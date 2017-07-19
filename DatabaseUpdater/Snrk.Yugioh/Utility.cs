using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Snrk
{
    public static class Utility
    {
        public static IEnumerable<Task> WaitAll(this IEnumerable<Task> tasks)
        {
            foreach (Task task in tasks)
                if (task != null && !task.IsCompleted)
                    task.Wait();
            return tasks;
        }

        public static string ToTitleCase(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return text;
            string result = "";
            bool newWord = true;
            foreach (char c in text)
            {
                if (newWord)
                {
                    result += Char.ToUpper(c);
                    newWord = false;
                }
                else
                    result += Char.ToLower(c);
                if (c == ' ' || c == '\'')
                    newWord = true;
            }
            return result;
        }

        public static string HexToString(string hex)
        {
            string StrValue = "";
            while (hex.Length > 0)
            {
                StrValue += Convert.ToChar(Convert.ToUInt32(hex.Substring(0, 2), 16)).ToString();
                hex = hex.Substring(2, hex.Length - 2);
            }
            return StrValue;
        }

        public static string StringToHex(string text)
        {
            string hex = "";
            foreach (char c in text)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        public static string RemoveBetween(this string text, char start, char end)
        {
            if (String.IsNullOrEmpty(text))
                return "";
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", start, end));
            return regex.Replace(text, string.Empty);
        }

        public static string GetBetween(this string text, string begin, string end)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(begin) || String.IsNullOrEmpty(end))
                return "";
            int posA = text.IndexOf(begin);
            int posB = text.LastIndexOf(end);
            if (posA == -1 || posB == -1)
                return "";
            int adjustedPosA = posA + begin.Length;
            if (adjustedPosA >= posB)
                return "";
            return text.Substring(adjustedPosA, posB - adjustedPosA);
        }
    }
}
