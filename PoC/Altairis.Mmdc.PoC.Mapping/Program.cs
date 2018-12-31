using System;
using System.Collections.Generic;
using System.Linq;
using Altairis.Mmdc.DisplayDriver;

namespace Altairis.Mmdc.PoC.Mapping {
    class Program {
        static void Main(string[] args) {
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

            // Create map from connected displays
            var hMap = CreateMap(displays, new HorizontalMapBuildStrategy());
            var vMap = CreateMap(displays, new VerticalMapBuildStrategy());

            // Test geometry validation
            Console.Write("Creating map with geometry errors...");
            var badMap = DisplayMap.LoadFromFile("MapWithGeometryErrors.json");
            Console.WriteLine("OK");
            ValidateMap(badMap, new MapGeometryValidator());
            Console.WriteLine();

            // Test connectivity validation
            Console.Write("Creating map with connectivity errors...");
            badMap = DisplayMap.LoadFromFile("MapWithConnectivityErrors.json");
            Console.WriteLine("OK");
            ValidateMap(badMap, new MapConnectionValidator(displays, allowExtraDisplays: false));
            Console.WriteLine();
        }

        private static DisplayMap CreateMap(IEnumerable<PhysicalDisplayInfo> displays, IMapBuildStrategy strategy) {
            Console.Write($"Creating display map with using{strategy.GetType()}...");
            var map = DisplayMap.Create(displays, strategy);
            Console.WriteLine("OK");
            ValidateMap(map, new MapGeometryValidator());
            ValidateMap(map, new MapConnectionValidator(displays));
            Console.WriteLine("This is the map JSON:");
            Console.WriteLine(map.SaveToJson());
            Console.WriteLine();
            return map;
        }

        private static void ValidateMap(DisplayMap map, IMapValidator validator) {
            Console.Write($"Validating map using {validator.GetType()}...");
            var errors = validator.Validate(map);
            if(!errors.Any()) {
                Console.WriteLine("OK");
                return;
            }
            Console.WriteLine("Failed!");

            foreach (var item in errors) {
                Console.WriteLine($"{item.Display.SerialNumber}: {item.Message}");
            }
        }

    }
}
