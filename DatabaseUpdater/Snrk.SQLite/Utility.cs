using System;

namespace Snrk.SQLite
{
    static class Utility
    {
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparisonType)
        {
            newValue = newValue ?? string.Empty;
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(oldValue) || oldValue.Equals(newValue, comparisonType))
            {
                return str;
            }
            int foundAt;
            while ((foundAt = str.IndexOf(oldValue, 0, comparisonType)) != -1)
            {
                str = str.Remove(foundAt, oldValue.Length).Insert(foundAt, newValue);
            }
            return str;
        }
    }
}
