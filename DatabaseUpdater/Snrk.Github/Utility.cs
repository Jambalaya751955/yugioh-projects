using System;
using System.IO;
using System.Linq;

namespace Snrk.Github
{
    static class Utility
    {
        public static string RemoveWhitespace(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            return new string(str.Where(c => !Char.IsWhiteSpace(c)).ToArray());
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
