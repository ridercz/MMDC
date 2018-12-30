using System;
using System.IO.Ports;

namespace Altairis.Mmdc.DisplayDriver {
    public class PhysicalDisplay : IDisplay, IDisposable {
        private const byte ACK_CHAR = 0x06;

        public const int DEFAULT_SPEED = 115200;
        public const int DEFAULT_TIMEOUT = 10000; //ms

        private readonly SerialPort port;

        public PhysicalDisplayInfo Properties { get; } = new PhysicalDisplayInfo();

        public int Width => this.Properties.Width;

        public int Height => this.Properties.Height;

        public string DisplayId => this.Properties.SerialNumber;

        public PhysicalDisplay(string portName, int speed = DEFAULT_SPEED, int timeout = DEFAULT_TIMEOUT) {
            this.port = new SerialPort(portName, speed) {
                ReadTimeout = timeout,
                WriteTimeout = timeout,
                NewLine = "\r\n",
                DtrEnable = true,
                RtsEnable = true
            };
            this.Properties.PortName = portName;
        }

        public void Open() {
            if (this.port.IsOpen) throw new InvalidOperationException("Port already open.");

            this.port.Open();
            var result = this.Properties.ReadFromOpenPort(this.port);
            if (!result) throw new InvalidOperationException("Unknown device - unrecognized signature.");
        }

        public void SendFrame(byte[] rawData) {
            if (rawData == null) throw new ArgumentNullException(nameof(rawData));
            if (!this.port.IsOpen) throw new InvalidOperationException("Port not open.");

            // Compute and compare expected frame length
            var frameLength = this.Properties.Width * this.Properties.Height * 3;
            if (rawData.Length != frameLength) throw new ArgumentException($"Invalid data length. Expected {frameLength} bytes, got {rawData.Length} bytes.", nameof(rawData));

            // Send raw data
            this.port.Write(rawData, 0, frameLength);
            this.WaitForAck();
        }

        public void SendColor(byte r, byte g, byte b) {
            if (!this.port.IsOpen) throw new InvalidOperationException("Port not open.");

            // Send single color frame
            var color = new byte[] { r, g, b };
            for (var i = 0; i < this.Properties.Width * this.Properties.Height; i++) {
                this.port.Write(color, 0, 3);
            }
            this.WaitForAck();
        }

        public void Close() => this.port.Close();

        private void WaitForAck() {
            // Read ack
            var ack = this.port.ReadByte();
            if (ack != ACK_CHAR) throw new Exception($"Unexpected response received. Expected 0x{ACK_CHAR:X2}, received 0x{ack:X2}.");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!this.disposedValue) {
                if (disposing) {
                    this.port.Dispose();
                }
                this.disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Display() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
