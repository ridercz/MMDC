using System;
using System.Threading;
using Altairis.Mmdc.DisplayDriver;

namespace MmdcPoc {
    internal class Program {
        private const string PORT_NAME = "COM4";

        private static void Main(string[] args) {
            var display = new PhysicalDisplay(PORT_NAME);

            // Connect
            Console.Write("Connecting to display...");
            display.Open();
            Console.WriteLine($"{display.Properties.Version} (SN {display.Properties.SerialNumber}), resolution {display.Properties.Width} x {display.Properties.Height}");

            // Measure fps
            var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var frame = new byte[display.Properties.Width * display.Properties.Height * 3];
            var frameCount = 0;
            var sw = new System.Diagnostics.Stopwatch();
            Console.WriteLine("Sending random frames. Press any key to stop.");
            sw.Start();
            while (true) {
                rng.GetBytes(frame);
                display.SendFrame(frame);
                frameCount++;
                if (Console.KeyAvailable) break;
            }
            sw.Stop();
            Console.WriteLine($"Sent {frameCount} in {sw.Elapsed.TotalSeconds:N2} ms = {frameCount/ sw.Elapsed.TotalSeconds:N2} fps.");

            // Turn off
            display.SendColor(0, 0, 0);

            // Close
            Console.Write("Closing display...");
            display.Close();
            Console.WriteLine("OK");
        }
    }
}
