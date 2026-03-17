using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotMorten.HomeAssistent;
using Iot.Device.Media;
using SkiaSharp;

namespace EPaperApp
{
    public interface IScreen : IDisposable
    {
        int Width { get; }
        int Height { get; }
        event EventHandler Touched;

        void DisplayImage(ReadOnlySpan<byte> buffer, bool partial);
        void Clear(bool white);
    }

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

    internal class SmartDisplay : IDisposable
    {
        private IScreen _iscreen;
        readonly SynchronizationContext uithread ;

        public SmartDisplay()
        {
            uithread = new SynchronizationContext();
            _iscreen = new EPaperScreen();
            _iscreen.Touched += Touch_Touched;
        }

        public void Initialize()
        {
            pages.Add(new UtilitiesScreen());
            pages.Add(new TemperaturePage());
            pages.Add(new WeatherPage());
            foreach (var screen in pages)
            {
                screen.Initialize();
            }
            Console.WriteLine("Clearing display");
            this._iscreen.Clear(true);
            Console.WriteLine("Display cleared");
            //Thread.Sleep(3000);
            foreach (var screen in pages)
            {
                screen.HasChanged += Screen_HasChanged;
            }
        }
        private PageBase? currentScreen;

        private void Screen_HasChanged(object? sender, EventArgs e)
        {
            uithread.Post((context) =>
            {
                if(sender == currentScreen)
                    UpdateScreen();
            }, sender);
        }
        public async void UpdateScreen(bool force = false, bool partialUpdate = true)
        {
            if (currentScreen != null)
            {
                while (isUpdating)
                {
                    await Task.Delay(100);
                }
                if (currentScreen.IsDirty || force)
                {
                    isUpdating = true;
                    RenderScreen(currentScreen, partialUpdate);
                    isUpdating = false;
                }
            }
        }

        PageBase? lastRenderedPage;
        private void RenderScreen(PageBase page, bool partialUpdate)
        {
            using (SKBitmap bitmap = new SKBitmap(_iscreen.Height, _iscreen.Width, SKColorType.Gray8, SKAlphaType.Opaque))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    page.GetPage(canvas, bitmap.Info);
                }
                using ImageBuffer b = new ImageBuffer(_iscreen.Height, _iscreen.Width);
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
                    this._iscreen.DisplayImage(rotated.Buffer, partial: partialUpdate);
                }
                lastRenderedPage = page;
            }
        }
        bool isUpdating;
        private List<PageBase> pages = new List<PageBase>();
        int currentPageIndex = 0;
        public async Task Run()
        {
            while (true)
            {
                //await RenderScreen(screen);
                currentScreen = pages[currentPageIndex];
                UpdateScreen(force:true, partialUpdate: currentPageIndex > 0 || newPageTask.Task.IsCompleted);
                // Wait until the next minute but at least 3 seconds
                int delay = (60 - DateTime.Now.Second) * 1000;
                if (delay < 3000) delay += 60000;
                newPageTask = new TaskCompletionSource();
                await Task.WhenAny(Task.Delay(delay), newPageTask.Task);
                currentPageIndex++;
                if (currentPageIndex >= pages.Count)
                {
                    currentPageIndex = 0;
                }
            }
        }
        TaskCompletionSource newPageTask = new TaskCompletionSource();
        private void Touch_Touched(object? sender, EventArgs e)
        {
            newPageTask?.TrySetResult();
        }

        public void Dispose()
        {
            foreach(var page in pages.OfType<IDisposable>())
                page.Dispose();
           _iscreen.Dispose();
        }
    }
}
