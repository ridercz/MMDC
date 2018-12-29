using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Altairis.Mmdc.DisplayDriver {
    public class PhysicalDisplayInfo {
        private const string DEVICE_SIGNATURE = "MMDC Display Connected";
        private const int SN_LENGTH = 8;

        // Public properties

        public string PortName { get; set; }

        public string Version { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string SerialNumber { get; set; }

        // Helper methods

        internal bool ReadFromOpenPort(SerialPort port) {
            if (port == null) throw new ArgumentNullException(nameof(port));

            // Read intro line
            var line = port.ReadLine();
            if (!line.Equals(DEVICE_SIGNATURE, StringComparison.OrdinalIgnoreCase)) return false;

            while (true) {
                line = port.ReadLine();
                if (line.Equals("OK", StringComparison.OrdinalIgnoreCase)) break;   // End of header
                if (line.Equals("SN?", StringComparison.OrdinalIgnoreCase)) {
                    this.CreateRandomSerialNumber(port);
                    continue;
                }

                // Parse header line
                var data = line.Split(new char[] { '=' }, 2);
                if (data.Length != 2) throw new Exception($"Unexpected data received: '{line}'.");

                // Parse known headers
                if (data[0].Equals("VERSION", StringComparison.OrdinalIgnoreCase)) this.Version = data[1];
                if (data[0].Equals("SN", StringComparison.OrdinalIgnoreCase)) this.SerialNumber = data[1];
                if (data[0].Equals("WIDTH", StringComparison.OrdinalIgnoreCase)) this.Width = int.Parse(data[1]);
                if (data[0].Equals("HEIGHT", StringComparison.OrdinalIgnoreCase)) this.Height = int.Parse(data[1]);
            }

            return true;
        }

        private void CreateRandomSerialNumber(SerialPort port) {
            if (port == null) throw new ArgumentNullException(nameof(port));

            // Generate random serial number
            var sn = new byte[SN_LENGTH];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create()) {
                rng.GetBytes(sn);
            }

            // Save it to device
            port.Write(sn, 0, sn.Length);
        }

    }
}
