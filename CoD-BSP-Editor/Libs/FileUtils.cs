using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoD_BSP_Editor.Libs
{
    public static class FileUtils
    {
        public static string ReadAllTextCRLF(string _path)
        {
            string text = File.ReadAllText(_path);

            text = text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");

            return text;
        }
    }
}
