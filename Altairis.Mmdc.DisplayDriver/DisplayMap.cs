using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Altairis.Mmdc.DisplayDriver {
    public class DisplayMap : Dictionary<PhysicalDisplayInfo, PhysicalDisplayPosition> {

        public int Width { get; set; }

        public int Height { get; set; }

        public static DisplayMap Create(IEnumerable<PhysicalDisplayInfo> displays, IMapStrategy strategy) {
            if (displays == null) throw new ArgumentNullException(nameof(displays));
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            if (!displays.Any()) throw new ArgumentException("At least one display must be supplied.", nameof(displays));

            // Create empty map
            var map = new DisplayMap();
            PhysicalDisplayInfo lastDisplay = null;
            PhysicalDisplayPosition lastPosition = null;

            // Layout display using specified strategy
            foreach (var display in displays) {
                lastPosition = strategy.GetDisplayPosition(lastDisplay, lastPosition);
                lastDisplay = display;
                map.Add(lastDisplay, lastPosition);
            }

            // Compute size
            map.Width = map.Max(d => d.Value.X + d.Key.Width);
            map.Height = map.Max(d => d.Value.Y + d.Key.Height);

            return map;
        }

        public static DisplayMap LoadFromFile(string fileName) => LoadFromJson(File.ReadAllText(fileName));

        public static DisplayMap LoadFromJson(string json) {
            if (json == null) throw new ArgumentNullException(nameof(json));
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(json));

            return JsonConvert.DeserializeObject<DisplayMap>(json);
        }

        public string SaveToJson(Formatting formatting = Formatting.Indented) => JsonConvert.SerializeObject(this, formatting);

        public void SaveToFile(string fileName, Formatting formatting = Formatting.Indented) => File.WriteAllText(fileName, this.SaveToJson(formatting));

    }
}
