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

        public void DisplayImage(ReadOnlySpan<byte> buffer, bool partial) 
            => paper.DisplayImage(buffer, partial);

        public void Dispose()
        {
            paper.Sleep();
            paper.Dispose();
            touch.Dispose();
            _gpioController.Dispose();
        }
        public void Clear(bool white) => paper.Clear(white);
        public event EventHandler? Touched;
        public int Width => paper.Width;
        public int Height => paper.Height;
    }
}
