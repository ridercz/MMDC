using System.Collections.Generic;

namespace Altairis.Mmdc.DisplayDriver {
    public interface IMapValidator {

        IEnumerable<MapValidationError> Validate(DisplayMap map);

    }

    public class MapValidationError {

        public PhysicalDisplayInfo Display { get; set; }

        public PhysicalDisplayPosition Position { get; set; }

        public string Message { get; set; }

    }
}
