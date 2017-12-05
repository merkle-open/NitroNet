using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NitroNet.ViewEngine.IO;

namespace NitroNet.ModelBuilder.Generator.Utilities
{
    internal class FileSystemUtility
    {
        public static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static IEnumerable<FileInfo> DirSearch(string sDir)
        {
            try
            {
                var fileList = new List<FileInfo>();
                foreach (var d in Directory.GetDirectories(sDir))
                {
                    fileList.AddRange(Directory.GetFiles(d, "*.json").Select(f => new FileInfo(f)));
                    fileList.AddRange(DirSearch(d));
                }

                return fileList;
            }
            catch
            {
                return new List<FileInfo>();
            }
        }
    }
}
