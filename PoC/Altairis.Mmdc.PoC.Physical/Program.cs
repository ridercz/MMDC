using System;
using Altairis.Mmdc.DisplayDriver;

namespace MmdcPhysicalDisplaysPoc {
    internal class Program {

        private static void Main(string[] args) {
            // List all available displays
            Console.WriteLine("Let's look for displays. It may take several minutes to complete.");
            Console.Write("Scanning ports for compatible displays...");
            var displays = PhysicalDisplayManager.ScanPorts();
            if (displays.Count == 0) {
                Console.WriteLine("Failed!");
                Console.WriteLine("No displays found.");
                return;
            }
            Console.WriteLine($"OK, found {displays.Count} displays.");
            Console.WriteLine();

            // Show table with parameters
            Console.WriteLine("----------------+------+------+------------------+-----------------------------");
            Console.WriteLine("Port            |    W |    H | Serial Number    | Version                     ");
            Console.WriteLine("----------------|------|------|------------------|-----------------------------");
            foreach (var display in displays) {
                Console.WriteLine($"{display.PortName,-15} | {display.Width,4} | {display.Height,4} | {display.SerialNumber} | {display.Version}");
            }
            Console.WriteLine("----------------+------+------+------------------+-----------------------------");
            Console.WriteLine();

            // Test displays one by one
            foreach (var display in displays) {
                TestSingleDisplay(display.PortName);
            }
        }

        private static void TestSingleDisplay(string portName) {
            if (portName == null) throw new ArgumentNullException(nameof(portName));
            if (string.IsNullOrWhiteSpace(portName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(portName));

            using (var display = new PhysicalDisplay(portName)) {
                // Connect
                Console.Write($"Connecting to {portName}...");
                display.Open();
                Console.WriteLine("OK");
                Console.WriteLine($"  {display.Properties.Version} (SN: {display.Properties.SerialNumber}), resolution {display.Properties.Width} x {display.Properties.Height}");

                // Measure fps
                var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
                var frame = new byte[display.Properties.Width * display.Properties.Height * 3];
                var frameCount = 0;
                var sw = new System.Diagnostics.Stopwatch();
                Console.WriteLine("  Sending random frames to measure fps...");
                sw.Start();
                while (true) {
                    rng.GetBytes(frame);
                    display.SendFrame(frame);
                    frameCount++;
                    if (sw.ElapsedMilliseconds > 3000) break;
                }
                sw.Stop();
                Console.WriteLine($"  Sent {frameCount} frames in {sw.Elapsed.TotalSeconds:N2} s = {frameCount / sw.Elapsed.TotalSeconds:N2} fps.");

                // Turn off
                display.SendColor(0, 0, 0);

                // Close
                Console.Write("  Closing display...");
                display.Close();
                Console.WriteLine("OK");
            }
        }
    }
}
