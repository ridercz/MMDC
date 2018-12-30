using System;
using System.Collections.Generic;
using System.Text;

namespace Altairis.Mmdc.DisplayDriver {
    public interface IMapStrategy {
        PhysicalDisplayPosition GetDisplayPosition(PhysicalDisplayInfo previousInfo, PhysicalDisplayPosition previousPosition);
    }

    public class HorizontalMapStrategy : IMapStrategy {
        public PhysicalDisplayPosition GetDisplayPosition(PhysicalDisplayInfo previousInfo, PhysicalDisplayPosition previousPosition) {
            if (previousInfo == null || previousPosition == null) return new PhysicalDisplayPosition { X = 0, Y = 0 };
            return new PhysicalDisplayPosition {
                X = previousPosition.X + previousInfo.Width,
                Y = previousPosition.Y
            };
        }
    }

    public class VerticalMapStrategy : IMapStrategy {
        public PhysicalDisplayPosition GetDisplayPosition(PhysicalDisplayInfo previousInfo, PhysicalDisplayPosition previousPosition) {
            if (previousInfo == null || previousPosition == null) return new PhysicalDisplayPosition { X = 0, Y = 0 };
            return new PhysicalDisplayPosition {
                X = previousPosition.X,
                Y = previousPosition.Y + previousInfo.Height
            };
        }
    }

}
