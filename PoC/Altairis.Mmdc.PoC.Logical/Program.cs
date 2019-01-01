using System;
using System.Threading;
using Altairis.Mmdc.DisplayDriver;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Altairis.Mmdc.PoC.Logical {
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

            // Create display map
            Console.Write("Creating display map...");
            var map = DisplayMap.Create(displays, new VerticalMapBuildStrategy());
            Console.WriteLine($"OK, size {map.Width} x {map.Height}");

            // Create logical display
            Console.Write("Creating and connecting logical display...");
            var display = new LogicalDisplay(map);
            display.Open();
            Console.WriteLine("OK");

            // Create image
            Console.WriteLine("Sending image:");
            var image = new Image<Rgb24>(display.Width, display.Height);
            for (var x = 0; x < image.Width; x++) {
                for (var y = 0; y < image.Height; y++) {
                    Console.Write(".");
                    image[x, y] = new Rgb24(0x33, 0x00, 0x00); // Red
                    display.SendFrame(image);
                    //Thread.Sleep(10);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
