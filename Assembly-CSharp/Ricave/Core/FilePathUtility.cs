using System;
using System.IO;

namespace Ricave.Core
{
    public static class FilePathUtility
    {
        public static string GetRelativePath(string fullPath, string relativeTo)
        {
            string text = FilePathUtility.NormalizeToForwardSlashes(Path.GetFullPath(fullPath));
            string text2 = FilePathUtility.NormalizeToForwardSlashes(Path.GetFullPath(relativeTo));
            if (text.StartsWith(text2))
            {
                return text.Substring(text2.Length).TrimStart(new char[] { '/', '\\' });
            }
            string text3 = text2;
            int num = 0;
            while (!text3.NullOrEmpty())
            {
                int num2 = text3.LastIndexOf('/');
                if (num2 <= 0)
                {
                    break;
                }
                text3 = text3.Substring(0, num2);
                num++;
                if (text.StartsWith(text3))
                {
                    string text4 = "";
                    for (int i = 0; i < num; i++)
                    {
                        text4 += "../";
                    }
                    return text4 + text.Substring(text3.Length).TrimStart(new char[] { '/', '\\' });
                }
            }
            return fullPath;
        }

        public static string NormalizeToForwardSlashes(string path)
        {
            return path.Replace("\\", "/");
        }

        public static string GetFilePathWithoutExtension(string filePath)
        {
            int num = filePath.LastIndexOf('.');
            if (num < 0)
            {
                return filePath;
            }
            if (filePath.LastIndexOf('/') > num)
            {
                return filePath;
            }
            if (filePath.LastIndexOf('\\') > num)
            {
                return filePath;
            }
            return filePath.Substring(0, num);
        }
    }
}