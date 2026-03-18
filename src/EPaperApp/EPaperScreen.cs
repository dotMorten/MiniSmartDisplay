using SkiaSharp;
using System.Device.Gpio;

namespace EPaperApp
{
    internal class EPaperScreen : IScreen
    {
         private EPaper.TouchController touch;
        private GpioController _gpioController;
        private EPaper.EPaper paper;
 
        public EPaperScreen()
        {
            _gpioController = new GpioController();
            touch = new EPaper.TouchController(_gpioController, 27);
            paper = EPaper.EPaper.Create2in9HatDisplay(_gpioController);
            touch.Touched += Touch_Touched;

        }

        private void Touch_Touched(object? sender, EventArgs e)
        {
            Touched?.Invoke(this, EventArgs.Empty);
        }

        public void DisplayImage(SKBitmap bitmap, bool partial)
        {
            using ImageBuffer b = new ImageBuffer(Height, Width);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    bitmap.GetPixel(x, y).ToHsl(out float h, out float s, out float l);
                    b.SetPixel(x, y, l > 0.75);
                }
            }
            using var rotated = b.Rotate();
            {
                paper.DisplayImage(rotated.Buffer, partial: partial);
            }
        }

        public void Dispose()
        {
            paper.Sleep();
            paper.Dispose();
            touch.Dispose();
            _gpioController.Dispose();
        }
        public void Clear(bool white) => paper.Clear(white);
        public event EventHandler? Touched;
        public int Width => paper.Height;
        public int Height => paper.Width;
    }
}
