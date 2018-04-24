using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngMeta
{
    static class StringUtils
    {
        public static string SplitStringIntoLines(string str, int size)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i += size)
            {
                sb.Append(str.Substring(i, Math.Min(size, str.Length - i)).Replace('\n', '⏎'));
                if (i < str.Length) sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}
