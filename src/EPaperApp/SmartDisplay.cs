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
    internal class SmartDisplay : IDisposable
    {
        private EPaper.TouchController touch;
        private GpioController _gpioController;
        private EPaper.EPaper paper;

        private const string HomeAssistentUrl = "http://192.168.1.138:8123";
        private const string HomeAssistentAccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiI1MDU5ZDY3ZGFmNTc0NjBmOTQwNTZiNGQ3NWJjMWY4MCIsImlhdCI6MTYzMDI3NTIzOSwiZXhwIjoxOTQ1NjM1MjM5fQ.YIRKckVr5siqtkv8fR_M5WT8tfMu08MSZg1CzoZ0Mvo";
        internal static HAClient HomeClient { get; } = new HAClient(Configuration.HomeAssistentAccessToken, Configuration.HomeAssistentUrl);
        readonly SynchronizationContext uithread ;
        public SmartDisplay()
        {
            _gpioController = new GpioController();
            touch = new EPaper.TouchController(_gpioController, 27);
            paper = EPaper.EPaper.Create2in9HatDisplay(_gpioController);
            uithread = new SynchronizationContext();
            touch.Touched += Touch_Touched;
        }


        public void Initialize()
        {
            screens.Add(new UtilitiesScreen());
            screens.Add(new TemperatureScreen());
            screens.Add(new WeatherScreen());
            foreach (var screen in screens)
            {
                screen.Initialize();
            }
            Console.WriteLine("Clearing display");
            paper.Clear(true);
            Console.WriteLine("Display cleared");
            //Thread.Sleep(3000);
            foreach (var screen in screens)
            {
                screen.HasChanged += Screen_HasChanged;
            }
        }
        private ScreenBase? currentScreen;

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

        ScreenBase? lastRenderedScreen;
        private void RenderScreen(ScreenBase screen, bool partialUpdate)
        {
            using (SKBitmap bitmap = new SKBitmap(paper.Height, paper.Width, SKColorType.Gray8, SKAlphaType.Opaque))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    screen.GetScreen(canvas, bitmap.Info);
                }
                using ImageBuffer b = new ImageBuffer(paper.Height, paper.Width);
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
                    paper.DisplayImage(rotated.Buffer, partial: partialUpdate);
                }
                lastRenderedScreen = screen;
            }
        }
        bool isUpdating;
        private List<ScreenBase> screens = new List<ScreenBase>();
        int currentScreenIndex = 0;
        public async Task Run()
        {
            while (true)
            {
                //await RenderScreen(screen);
                currentScreen = screens[currentScreenIndex];
                UpdateScreen(force:true, partialUpdate: currentScreenIndex > 0 || newPageTask.Task.IsCompleted);
                // Wait until the next minute but at least 3 seconds
                int delay = (60 - DateTime.Now.Second) * 1000;
                if (delay < 3000) delay += 60000;
                newPageTask = new TaskCompletionSource();
                await Task.WhenAny(Task.Delay(delay), newPageTask.Task);
                currentScreenIndex++;
                if (currentScreenIndex >= screens.Count)
                {
                    currentScreenIndex = 0;
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
            foreach(var screen in screens.OfType<IDisposable>())
                screen.Dispose();
            paper.Sleep();
            paper.Dispose();
            touch.Dispose();
            _gpioController.Dispose();
        }
    }
}
