using System;
using System.Collections.Generic;
using System.Text;

namespace Altairis.Mmdc.DisplayDriver {
    public interface IDisplay {

        void SendFrame(byte[] rawData);

        void SendColor(byte r, byte g, byte b);

        int Width { get;  }

        int Height { get; }

        string DisplayId { get; }

    }
}
