using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace Altairis.Mmdc.DisplayDriver {
    public static class PhysicalDisplayManager {
        public const int DEFAULT_TIMEOUT = 2000; // ms
        public const int DEFAULT_SPEED = 115200;

        public static IReadOnlyList<PhysicalDisplayInfo> ScanPorts(int speed = DEFAULT_SPEED, int timeout = DEFAULT_TIMEOUT) => ScanPorts(SerialPort.GetPortNames(), speed, timeout);

        public static IReadOnlyList<PhysicalDisplayInfo> ScanPorts(IEnumerable<string> portNames, int speed = DEFAULT_SPEED, int timeout = DEFAULT_TIMEOUT) {
            var foundDevices = new List<PhysicalDisplayInfo>();
            foreach (var portName in portNames) {
                var info = ScanPort(portName, speed, timeout);
                if (info != null) foundDevices.Add(info);
            }
            return foundDevices.AsReadOnly();
        }

        public static PhysicalDisplayInfo ScanPort(string portName, int speed = DEFAULT_SPEED, int timeout = DEFAULT_TIMEOUT) {
            if (portName == null) throw new ArgumentNullException(nameof(portName));
            if (string.IsNullOrWhiteSpace(portName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(portName));

            try {
                using (var port = new SerialPort(portName, speed)) {
                    port.ReadTimeout = timeout;
                    port.NewLine = "\r\n";
                    port.DtrEnable = true;
                    port.RtsEnable = true;
                    port.Open();

                    // Try to read device information
                    var info = new PhysicalDisplayInfo { PortName = portName };
                    var result = info.ReadFromOpenPort(port);
                    if (!result) return null; // Uknown device

                    // Cleanup
                    port.Close();
                    return info;
                }
            }
            catch (Exception) {
                return null;
            }
        }

    }
}
