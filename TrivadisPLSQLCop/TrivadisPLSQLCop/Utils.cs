using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrivadisPLSQLCop
{
    static class Utils
    {
        static Encoding utf8WithoutBom = new System.Text.UTF8Encoding(false);
        public unsafe static string PtrUTF8StrToString(this IntPtr element)
        {
            System.IO.MemoryStream w = new System.IO.MemoryStream(10 * 1024);
            var p = (byte*)element;
            while (*p != 0)
            {
                w.WriteByte(*p);
                p++;
            }
            return w.Length == 0 ? string.Empty : utf8WithoutBom.GetString(w.ToArray(), 0, (int)w.Length);
        }

        public static byte[] ToUTF8ByteArray(this string element)
        {
            if (null == element)
            {
                return Array.Empty<byte>();
            }
            return utf8WithoutBom.GetBytes(element);
        }

        public static void CopyTo(string source, string destination, Encoding enc)
        {
            using (var r = new StreamReader(source, true))
            {
                const int bufferLength = 10 * 1024;
                char[] buffer = new char[bufferLength]; int read;
                using (var w = new StreamWriter(destination, false, enc))
                {
                    while (0 != (read = r.Read(buffer, 0, bufferLength)))
                    {
                        w.Write(buffer, 0, read);
                    }
                }
            }
        }

        public static void MapToDictionary(this string element, ConcurrentDictionary<string, string> items)
        {
            foreach (var item in element.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var keyValue = item.ToLower().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (keyValue.Length == 2)
                {
                    items[keyValue[0]] = keyValue[1];
                }
            }
        }
    }
}
