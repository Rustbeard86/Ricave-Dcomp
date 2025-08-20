using System;
using System.IO;

namespace Ricave.Core
{
    public static class DirectoryUtility
    {
        public static void CopyDirectory(string source, string dest, bool skipMetaFiles = false)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(source);
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            Directory.CreateDirectory(dest);
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                if (!skipMetaFiles || (!(fileInfo.Extension == ".meta") && !(fileInfo.Extension == ".DS_Store")))
                {
                    string text = Path.Combine(dest, fileInfo.Name);
                    fileInfo.CopyTo(text);
                }
            }
            foreach (DirectoryInfo directoryInfo2 in directories)
            {
                string text2 = Path.Combine(dest, directoryInfo2.Name);
                DirectoryUtility.CopyDirectory(directoryInfo2.FullName, text2, skipMetaFiles);
            }
        }
    }
}