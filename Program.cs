using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace digitalcompostbin
{
    class Program
    {
        static System.Timers.Timer mainTimer;
        static string[] pathsUser;
        static void Main(string[] args)
        {
            pathsUser = args;
            mainTimer = new System.Timers.Timer(60*60*1000);
            mainTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            mainTimer.Start();
            Run(pathsUser);
            while (true);
        }
        private static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            Run(pathsUser);
        }
        
        static void Run(string[] paths) {
            foreach (string path in paths) {
                if (!Directory.Exists(path + "/old")) {
                    Directory.CreateDirectory(path + "/old");
                }
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo f in di.EnumerateFiles()) {
                    if (!File.Exists(path + "/old/" + f.Name)) {
                        File.Copy(f.FullName, path + "/old/" + f.Name);
                    }
                    BitArray current_orig = new BitArray(File.ReadAllBytes(path + "/old/" + f.Name));
                    BitArray current = new BitArray(File.ReadAllBytes(f.FullName));
                    foreach (int i in IndicesToFlip(current.Count)) {
                        current[i] = !current[i];
                    }
                    byte[] result = new byte[current.Count / 8];
                    current.CopyTo(result, 0);
                    File.WriteAllBytes(f.FullName, result);
                }
            }
        }
        static Int32[] IndicesToFlip(Int32 upperBound) {
            List<Int32> result = new List<Int32>();
            int numberOfAddresses = RandomNumberGenerator.GetInt32((int)Math.Ceiling(10*Math.Log(upperBound)));

            for (int i = 0; i < numberOfAddresses; i++) {
                result.Add(RandomNumberGenerator.GetInt32(upperBound));
            }
            return result.ToArray();
        }
    }
}
