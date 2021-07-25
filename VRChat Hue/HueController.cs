using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.HSB;
using Q42.HueApi.Interfaces;

namespace VRChat_Hue
{
    /**
     *  https://github.com/Q42/Q42.HueApi/tree/v3.18.1
     **/

    public class HueController
    {
        private HueClient _client;

        public async Task SetupHueAsync()
        {
            Console.WriteLine("[Hue Controller] No API Key found. Setting up Hue Bridge with application");
            Console.WriteLine("[Hue Controller] Locating bridges...");

            IBridgeLocator locator = new HttpBridgeLocator();
            var bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));

            string ip = "";
            foreach (var bridge in bridges)
            {
                Console.WriteLine($"[Hue Controller] Bridge Discovered {bridge.IpAddress}");
                ip = bridge.IpAddress;
            }

            Console.WriteLine("[Hue Controller] Registering application. Please press the center button on your Hue Bridge then hit enter to continue...");
            Console.ReadLine();
            //I only use 1 bridge but it should be obvious how to extend this for multiple bridge use
            ILocalHueClient client = new LocalHueClient(ip);
            var appKey = await client.RegisterAsync("VRChat-Hue", "LocalPC");
            Console.WriteLine($"[Hue Controller] Your app key is {appKey}");
            System.IO.File.WriteAllText("hueapp.key", appKey);
            Console.WriteLine($"[Hue Controller] Saved key to hueapp.key press any key to exit...");
            Console.ReadKey();
        }

        public async Task Initialize()
        {
            Console.WriteLine("[Hue Controller] Locating bridges...");
            IBridgeLocator locator = new HttpBridgeLocator();
            var bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));

            var bridge = bridges.ToArray()[0];
            _client = new LocalHueClient(bridge.IpAddress);

            Console.WriteLine($"[Hue Controller] Connected to bridge {bridge.IpAddress}");
            _client.Initialize(System.IO.File.ReadAllText("hueapp.key"));

            Console.WriteLine("[Hue Controller] Application Initialized. Ready for use.");
        }

        public async Task SetColor(string hex)
        {
            Console.WriteLine($"[Hue Controller] Changing color to #{hex.ToUpper()}");
            var command = new LightCommand();
            command.On = true;
            command.TurnOn().SetColor(new RGBColor(hex));
            await _client.SendCommandAsync(command);
        }


        #region Light Control Testing
        public async Task SetColorLoop(bool enabled)
        {
            Console.WriteLine($"[Hue Controller] Setting color loop enabled: {enabled}");
            var command = new LightCommand();
            command.On = enabled;
            command.Effect = Effect.ColorLoop;
            await _client.SendCommandAsync(command);
        }

        public async Task SetToBlue()
        {
            await SetColor("34a1eb");
        }

        public async Task SetToPurple()
        {
            await SetColor("9b34eb");
        }

        public async Task SetToOrange()
        {
            await SetColor("ebb434");
        }
        #endregion //Light Control Testing
    }
}
