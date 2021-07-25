using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat_Hue
{
    class Program
    {
        static void Main(string[] args)
        {
            VRChatHueClient client = new VRChatHueClient();
            client.Start();
        }
    }
}
