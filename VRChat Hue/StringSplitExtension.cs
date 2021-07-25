using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat_Hue
{
    public static class StringSplitExtension
    {
        public static string[] Split(this string input, string splitBy)
        {
            return input.Split(new string[] { splitBy }, StringSplitOptions.None);
        }
    }
}
