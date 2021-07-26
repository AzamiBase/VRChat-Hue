using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using static System.Environment;
using static VRChat_Hue.StringSplitExtension;

namespace VRChat_Hue
{
    public class SimpleVRChatLogParser
    {
        private bool _running;
        private bool _newLog;
        private string _file;

        public void StartParser(Action<string> onCommand)
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
                        _newLog = true;
                    }

                    if (_newLog)
                    {
                        //Give the old thread enough time to close.
                        Thread.Sleep(2000);
                        _newLog = false;
                        new Thread(() =>
                        {
                            Follow(_file, onCommand);
                        }).Start();
                    }

                    Thread.Sleep(5000);
                }
            }).Start();
        }

        //https://stackoverflow.com/questions/23306089/reading-file-and-monitor-new-line
        private void Follow(string path, Action<String> onCommand)
        {
            Console.WriteLine($"[VRChat Parser] Following new log file: {path}");
            // Note the FileShare.ReadWrite, allowing others to modify the file
            using (FileStream fileStream = File.Open(path, FileMode.Open,
                FileAccess.Read, FileShare.ReadWrite))
            {
                fileStream.Seek(0, SeekOrigin.End);
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string lastCmd = "";
                    for (; ; )
                    {
                        if (_newLog || !_running)
                            return;

                        // read every 1 second
                        Thread.Sleep(TimeSpan.FromSeconds(0.25));

                        var lines = streamReader.ReadToEnd().Split('\n');
                        foreach (var line in lines)
                        {
                            if (string.IsNullOrEmpty(line))
                                continue;

                            //implement this however, I just wanted it to ignore non-[Hue] commands to test
                            if (!line.Contains("[Hue]"))
                                continue;
                            var cmd = line.Split("[Hue] ")[1];

                            if (cmd == lastCmd)
                                continue; //tried to avoid this in the state machine but the debug prints out way too fast, oh well.

                            onCommand(cmd);
                            lastCmd = cmd;
                        }
                    }
                }
            }
        }

        public void StopParser()
        {
            Console.WriteLine("[VRChat Parser] Stopping parser");
            _running = false;
        }
    }
}
