using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Altairis.Mmdc.DisplayDriver {
    public class DisplayMap {

        public int Width { get; set; }

        public int Height { get; set; }

        public IList<DisplayMapPosition> Items { get; set; } = new List<DisplayMapPosition>();

        public static DisplayMap Create(IEnumerable<PhysicalDisplayInfo> displays, IMapBuildStrategy strategy) {
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
                map.Items.Add(new DisplayMapPosition { Display = display, Position = lastPosition });
                lastDisplay = display;
            }

            // Compute size
            map.Width = map.Items.Max(d => d.Position.X + d.Display.Width);
            map.Height = map.Items.Max(d => d.Position.Y + d.Display.Height);

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
