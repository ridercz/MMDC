using System;
using System.Collections.Generic;
using System.Linq;

namespace Altairis.Mmdc.DisplayDriver {
    public class MapConnectionValidator : IMapValidator {

        public IEnumerable<PhysicalDisplayInfo> ConnectedDisplays { get; }

        public bool AllowExtraDisplays { get; }

        public MapConnectionValidator(bool allowExtraDisplays = true) : this(PhysicalDisplayManager.ScanPorts(), allowExtraDisplays) { }

        public MapConnectionValidator(IEnumerable<PhysicalDisplayInfo> connectedDisplays, bool allowExtraDisplays = true) {
            this.ConnectedDisplays = connectedDisplays;
            this.AllowExtraDisplays = allowExtraDisplays;
        }

        public IEnumerable<MapValidationError> Validate(DisplayMap map) {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (!map.Items.Any()) throw new ArgumentException("Map must contain at least one display.", nameof(map));

            // Create copy of list of connected displays
            var cdl = new List<PhysicalDisplayInfo>(this.ConnectedDisplays);

            // Check all mapped displays are connected
            foreach (var item in map.Items) {
                var scd = cdl.FirstOrDefault(x => x.SerialNumber.Equals(item.Display.SerialNumber, StringComparison.OrdinalIgnoreCase));
                if (scd == null) {
                    yield return new MapValidationError {
                        Display = item.Display,
                        Position = item.Position,
                        Message = "Mapped display is not connected."
                    };
                } else {
                    cdl.Remove(scd);
                }
            }

            // Check for unmapped displays
            if (!this.AllowExtraDisplays) {
                foreach (var item in cdl) {
                    yield return new MapValidationError {
                        Display = item,
                        Position = null,
                        Message = "Connected display is not mapped."
                    };
                }
            }
        }

    }
}
