using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VRChat_Hue
{
    public class VRChatHueClient
    {
        private HueController _hue;
        private SimpleVRChatLogParser _parser;

        public VRChatHueClient()
        {
            _hue = new HueController();
            _parser = new SimpleVRChatLogParser();
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
            await _hue.SetToOrange(); //set default color

            _parser.StartParser(async delegate(string content) {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    string[] lines = content.Split('\n');

                    foreach (string dirtyLine in lines)
                    {
                        string line = Regex.Replace(dirtyLine
                            .Replace("\r", "")
                            .Replace("\n", "")
                            .Replace("\t", "")
                            .Trim(),
                            @"\s+", " ", RegexOptions.Multiline);

                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        if (!line.Contains("[Hue]"))
                            continue;

                        string command = line.Split("[Hue] ")[1];

                        Console.WriteLine($"[VRChat Hue Client] Command Detected: {command}");
                        switch (command)
                        {
                            case "ColorLoop":
                                await _hue.SetColorLoop(true);
                                break;
                            case "Blue":
                                await _hue.SetToBlue();
                                break;
                            case "Orange":
                                await _hue.SetToOrange();
                                break;
                            case "Purple":
                                await _hue.SetToPurple();
                                break;
                            case "On":
                                await _hue.TurnBulbsOn();
                                break;
                            case "Off":
                                await _hue.TurnBulbsOff();
                                break;
                        }
                    }
                }
            });

            Console.ReadKey();
        }
    }
}
