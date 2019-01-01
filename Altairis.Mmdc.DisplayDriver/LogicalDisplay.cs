using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Altairis.Mmdc.DisplayDriver {
    public class LogicalDisplay : DisplayBase {
        private readonly DisplayMap map;
        private readonly List<PhysicalDisplay> displays = new List<PhysicalDisplay>();
        private bool isOpen = false;

        public LogicalDisplay(DisplayMap map) {
            this.map = map;
        }

        public override int Width => this.map.Width;

        public override int Height => this.map.Height;

        public void Open() {
            this.displays.Clear();
            foreach (var item in this.map.Items) {
                var newDisplay = new PhysicalDisplay(item.Display.PortName);
                newDisplay.Open();
                this.displays.Add(newDisplay);
            }
            this.isOpen = true;
        }

        public void Close() {
            foreach (var item in this.displays) {
                item.Close();
            }
            this.displays.Clear();
            this.isOpen = false;
        }

        public override void SendFrame(byte[] rawData) {
            if (rawData == null) throw new ArgumentNullException(nameof(rawData));
            if (!this.isOpen) throw new InvalidOperationException("Display not open.");

            // Compute and compare expected frame length
            var frameLength = this.Width * this.Height * 3;
            if (rawData.Length != frameLength) throw new ArgumentException($"Invalid data length. Expected {frameLength} bytes, got {rawData.Length} bytes.", nameof(rawData));

            // Load raw data as image
            var img = Image.LoadPixelData<Rgb24>(rawData, this.Width, this.Height);

            // Send image
            this.SendFrame(img);
        }

        public override void SendFrame<TPixel>(Image<TPixel> image) {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (!this.isOpen) throw new InvalidOperationException("Display not open.");

            foreach (var display in this.displays) {
                // Get display rectangle
                var mapItem = this.map.Items.FirstOrDefault(x => x.Display.SerialNumber == display.Properties.SerialNumber);
                var displayRectangle = new Rectangle {
                    X = mapItem.Position.Y,
                    Y = mapItem.Position.Y,
                    Width = mapItem.Display.Width,
                    Height = mapItem.Display.Height
                };

                // Get cropped image
                var croppedImage = image.Clone(x => x.Crop(displayRectangle));

                // Send cropped image
                display.SendFrame(croppedImage);
            }

        }


    }
}
