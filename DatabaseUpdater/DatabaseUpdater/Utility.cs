using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace DatabaseUpdater
{
    static class Utility
    {
        public static string ReplaceOutsideChar(this string str, char delimiter, string oldValue, string newValue)
        {
            string[] parts = str.Split(delimiter);
            for (int i = 0; i < parts.Length; i += 2)
                parts[i] = parts[i].Replace(oldValue, newValue);
            return string.Join(delimiter.ToString(), parts);
        }

        public static string GetZuluString()
        {
            var oldCultureInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            DateTime universalTime = DateTime.UtcNow.ToUniversalTime();
            string time = universalTime.ToShortDateString().Replace('/', '-') + 'T';
            Thread.CurrentThread.CurrentCulture = oldCultureInfo;
            time += universalTime.ToShortTimeString().Replace(" ", "");
            time += ':' + DateTime.UtcNow.Second.ToString("00");
            time += '.' + DateTime.UtcNow.Millisecond.ToString("00") + "Z";
            return time;
        }
    }

    static class DirectoryExt
    {
        public static void ForceDelete(this DirectoryInfo directory, bool recursive)
        {
            directory.Attributes = FileAttributes.Normal;

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(recursive);
        }

        public static void ForceDelete(string path, bool recursive)
        {
            var directory = new DirectoryInfo(path);
            directory.ForceDelete(true);
        }

        public static void ForceDeleteContent(string path)
        {
            var directory = new DirectoryInfo(path);

            foreach (FileInfo file in directory.GetFiles())
            {
                file.ForceDelete();
            }
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                dir.ForceDelete(true);
            }
        }
    }

    static class FileExt
    {
        public static void ForceDelete(this FileInfo file)
        {
            file.Attributes = FileAttributes.Normal;
            file.Delete();
        }

        public static void ForceDelete(string path)
        {
            var file = new FileInfo(path)
            {
                Attributes = FileAttributes.Normal
            };

            file.Delete();
        }
    }
}
