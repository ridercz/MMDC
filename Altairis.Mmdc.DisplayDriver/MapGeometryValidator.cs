using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.Primitives;

namespace Altairis.Mmdc.DisplayDriver {
    public class MapGeometryValidator : IMapValidator {
        public IEnumerable<MapValidationError> Validate(DisplayMap map) {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (!map.Items.Any()) throw new ArgumentException("Map must contain at least one display.", nameof(map));

            // Check displays are not out of bounds
            var mapRectangle = new Rectangle(0, 0, map.Width, map.Height);
            var badDisplays = map.Items.Where(d => !mapRectangle.Contains(d.ToRectangle()));
            foreach (var item in badDisplays) {
                yield return new MapValidationError {
                    Display = item.Display,
                    Position = item.Position,
                    Message = "Display is out of bounds of the screen."
                };
            }

            // Check displays are not overlapping
            foreach (var item in map.Items) {
                var myRectangle = item.ToRectangle();
                var intersectingIds = map.Items.Where(x => !x.Display.SerialNumber.Equals(item.Display.SerialNumber, StringComparison.OrdinalIgnoreCase) && myRectangle.IntersectsWith(x.ToRectangle())).Select(x => x.Display.SerialNumber);
                if (intersectingIds.Any()) {
                    yield return new MapValidationError {
                        Display = item.Display,
                        Position = item.Position,
                        Message = $"Display overlaps with following display(s): {string.Join(", ", intersectingIds)}."
                    };
                }
            }
        }


    }
}
