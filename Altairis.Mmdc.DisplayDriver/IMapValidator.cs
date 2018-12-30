using System;
using System.Collections.Generic;
using System.Text;

namespace Altairis.Mmdc.DisplayDriver {
    public interface IMapValidator {

        IEnumerable<MapValidationError> Validate(LogicalDisplayMap map);

    }

    public class MapValidationError {

        public PhysicalDisplayInfo Display { get; set; }

        public PhysicalDisplayPosition Position { get; set; }

        public string Message { get; set; }

    }
}
