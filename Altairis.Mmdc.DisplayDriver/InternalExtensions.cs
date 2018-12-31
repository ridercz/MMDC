using System;
using System.Collections.Generic;
using System.IO.Ports;
using SixLabors.Primitives;

namespace Altairis.Mmdc.DisplayDriver {
    internal static class InternalExtensions {
        private const string DEVICE_SIGNATURE = "MMDC Display Connected";
        private const int SN_LENGTH = 8;

        public static bool ReadFromOpenPort(this PhysicalDisplayInfo info, SerialPort port) {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (port == null) throw new ArgumentNullException(nameof(port));

            // Read intro line
            var line = port.ReadLine();
            if (!line.Equals(DEVICE_SIGNATURE, StringComparison.OrdinalIgnoreCase)) return false;

            while (true) {
                line = port.ReadLine();
                if (line.Equals("OK", StringComparison.OrdinalIgnoreCase)) break;   // End of header
                if (line.Equals("SN?", StringComparison.OrdinalIgnoreCase)) {
                    CreateRandomSerialNumber(port);
                    continue;
                }

                // Parse header line
                var data = line.Split(new char[] { '=' }, 2);
                if (data.Length != 2) throw new Exception($"Unexpected data received: '{line}'.");

                // Parse known headers
                if (data[0].Equals("VERSION", StringComparison.OrdinalIgnoreCase)) info.Version = data[1];
                if (data[0].Equals("SN", StringComparison.OrdinalIgnoreCase)) info.SerialNumber = data[1];
                if (data[0].Equals("WIDTH", StringComparison.OrdinalIgnoreCase)) info.Width = int.Parse(data[1]);
                if (data[0].Equals("HEIGHT", StringComparison.OrdinalIgnoreCase)) info.Height = int.Parse(data[1]);
            }

            return true;
        }

        private static void CreateRandomSerialNumber(SerialPort port) {
            if (port == null) throw new ArgumentNullException(nameof(port));

            // Generate random serial number
            var sn = new byte[SN_LENGTH];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create()) {
                rng.GetBytes(sn);
            }

            // Save it to device
            port.Write(sn, 0, sn.Length);
        }

        public static Rectangle ToRectangle(this DisplayMapPosition item) => new Rectangle(item.Position.X, item.Position.Y, item.Display.Width, item.Display.Height);

    }
}
