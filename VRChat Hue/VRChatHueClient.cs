using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat_Hue
{
    public class VRChatHueClient
    {
        private HueController _hue;

        public VRChatHueClient()
        {
            _hue = new HueController();
        }

        public void Start()
        {
            //Setup hue bridge if the app key has not been found
            if (!System.IO.File.Exists("hueapp.key"))
            {
                Task.Run(async () =>
                {
                    await _hue.SetupHueAsync();
                }).GetAwaiter().GetResult();
                return;
            }

            Task.Run(async () =>
            {
                await RunClientAsync();
            }).GetAwaiter().GetResult();
        }

        private async Task RunClientAsync()
        {
            Console.WriteLine("[VRChat Hue Client] Found API Key. Initializing hue client");
            await _hue.Initialize();
            await _hue.SetColorLoop(false); //for some reason this persists, unsure why don't really care though just disable it at startup

            //await _hue.SetToOrange();
            await _hue.SetToBlue();
            //await _hue.SetToPurple();

            Console.ReadKey();
        }
    }
}
