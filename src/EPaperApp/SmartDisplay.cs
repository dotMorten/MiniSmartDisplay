using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        void DisplayImage(SKBitmap bitmap, bool partial);
        void Clear(bool white);
    }

    internal class SmartDisplay : IDisposable
    {
        private IScreen _iscreen;
        readonly SynchronizationContext uithread ;

        public SmartDisplay()
        {
            uithread = new SynchronizationContext();
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            _iscreen = new EPaperScreen();
#if WINDOWS
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _iscreen = new SimulatedScreen(296, 128);
#endif
            else
                throw new PlatformNotSupportedException("Only Linux and Windows are supported");
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
            using (SKBitmap bitmap = new SKBitmap(_iscreen.Width, _iscreen.Height, SKColorType.Gray8, SKAlphaType.Opaque))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    page.GetPage(canvas, bitmap.Info);
                }

                this._iscreen.DisplayImage(bitmap, partial: partialUpdate);
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
                UpdateScreen(force:true, partialUpdate: currentPageIndex > 0 || newPageTask?.Task.IsCompleted == true);
                // Wait until the next minute but at least 3 seconds
                int delay = (60 - DateTime.Now.Second) * 1000;
                if (delay < 3000) delay += 60000;
                newPageTask = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                var task = newPageTask.Task;
                await Task.WhenAny(Task.Delay(delay), task).ConfigureAwait(false);
                currentPageIndex++;
                if (currentPageIndex >= pages.Count)
                {
                    currentPageIndex = 0;
                }
            }
        }
        TaskCompletionSource? newPageTask;
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
