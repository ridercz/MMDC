namespace Altairis.Mmdc.DisplayDriver {
    public interface IMapBuildStrategy {
        PhysicalDisplayPosition GetDisplayPosition(PhysicalDisplayInfo previousInfo, PhysicalDisplayPosition previousPosition);
    }

    public class HorizontalMapBuildStrategy : IMapBuildStrategy {
        public PhysicalDisplayPosition GetDisplayPosition(PhysicalDisplayInfo previousInfo, PhysicalDisplayPosition previousPosition) {
            if (previousInfo == null || previousPosition == null) return new PhysicalDisplayPosition { X = 0, Y = 0 };
            return new PhysicalDisplayPosition {
                X = previousPosition.X + previousInfo.Width,
                Y = previousPosition.Y
            };
        }
    }

    public class VerticalMapBuildStrategy : IMapBuildStrategy {
        public PhysicalDisplayPosition GetDisplayPosition(PhysicalDisplayInfo previousInfo, PhysicalDisplayPosition previousPosition) {
            if (previousInfo == null || previousPosition == null) return new PhysicalDisplayPosition { X = 0, Y = 0 };
            return new PhysicalDisplayPosition {
                X = previousPosition.X,
                Y = previousPosition.Y + previousInfo.Height
            };
        }
    }

}
