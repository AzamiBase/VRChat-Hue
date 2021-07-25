﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat_Hue
{
    public class VRChatHueClient
    {
        private HueController _hue;
        private SimpleVRChatConsoleParser _parser;

        public VRChatHueClient()
        {
            _hue = new HueController();
            _parser = new SimpleVRChatConsoleParser();
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

            _parser.StartParser(async delegate(string command) {
                //this is proof of concept stuff. implement this however you want
              
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
            });

            Console.ReadKey();
        }
    }
}
