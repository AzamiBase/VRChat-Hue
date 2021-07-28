using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using static VRChat_Hue.TailSubscription;
using static System.Environment;
using static VRChat_Hue.StringSplitExtension;

namespace VRChat_Hue
{
    public class SimpleVRChatLogParser
    {
        private bool _running;
        private string _file;
        private TailSubscription _reader;

        public void StartParser(OnUpdate onCommand)
        {
            Console.WriteLine("[VRChat Parser] Starting parser");
            _running = true;

            new Thread(() =>
            {
                while (_running)
                {
                    //every 5 seconds check for a new log file
                    //Console.WriteLine("[VRChat Parser] Checking for new log file...");
                    var logFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\VRChat\VRChat";
                    var file = new DirectoryInfo(logFolder)
                                .GetFiles()
                                .OrderByDescending(x => x.LastWriteTime)
                                .FirstOrDefault(x => x.Name.Contains("output_log_"));

                    if (file != null && file.FullName != _file)
                    {
                        _file = file.FullName;
                        _reader?.Dispose();
                        _reader = new TailSubscription(_file, onCommand, 0, 100);
                    }

                    Thread.Sleep(5000);
                }
            }).Start();
        }
        public void StopParser()
        {
            Console.WriteLine("[VRChat Parser] Stopping parser");
            _reader.Dispose();
        }
    }
}
