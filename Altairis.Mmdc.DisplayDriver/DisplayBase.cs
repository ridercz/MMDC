using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Altairis.Mmdc.DisplayDriver {
    public abstract class DisplayBase : IDisplay {

        // Required (abstract) members

        public abstract int Width { get; }

        public abstract int Height { get; }

        public abstract string DisplayId { get; }

        public abstract void SendFrame(byte[] rawData);

        // Default (virtual) members

        public virtual void SendColor(IPixel color) {
            var rgbPixel = default(Rgb24);
            color.ToRgb24(ref rgbPixel);
            this.SendColor(rgbPixel.R, rgbPixel.G, rgbPixel.B);
        }

        public virtual void SendColor(byte r, byte g, byte b) {
            // Create array of single collor
            var frameBuffer = new byte[this.Width * this.Height * 3];
            for (var i = 0; i < frameBuffer.Length; i += 3) {
                frameBuffer[i + 0] = r;
                frameBuffer[i + 1] = g;
                frameBuffer[i + 2] = b;
            }

            // Send array frame
            this.SendFrame(frameBuffer);
        }

        public virtual void SendImageFrame<TPixel>(Image<TPixel> image) where TPixel : struct, IPixel<TPixel> {
            if (image == null) throw new ArgumentNullException(nameof(image));

            // Convert image to RGB array
            var frameBuffer = new byte[this.Width * this.Height * 3];
            var fbIndex = 0;

            for (var x = 0; x < Math.Min(this.Width, image.Width); x++) {
                for (var y = 0; y < Math.Min(this.Height, image.Height); y++) {
                    var pixel = image[x, y];
                    var rgbPixel = default(Rgb24);
                    pixel.ToRgb24(ref rgbPixel);
                    frameBuffer[fbIndex + 0] = rgbPixel.R;
                    frameBuffer[fbIndex + 1] = rgbPixel.G;
                    frameBuffer[fbIndex + 2] = rgbPixel.B;
                    fbIndex += 3;
                }
            }

            // Send array frame
            this.SendFrame(frameBuffer);
        }

    }
}
