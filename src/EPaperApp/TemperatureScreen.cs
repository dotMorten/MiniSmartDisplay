using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotMorten.HomeAssistent;
using SkiaSharp;

namespace EPaperApp
{
    internal class TemperatureScreen : ScreenBase, IDisposable
    {
        private string temperatureEntityId = "sensor.tempest_temperature";
        private HAData? tempData;
        private HAData? poolData;
        private HAData? insideData;

        public override void GetScreen(SKCanvas canvas, SKImageInfo info)
        {
            canvas.Clear(SKColors.White);
            using var font10 = new SKFont() { Size = 10, Subpixel = false, Edging = SKFontEdging.Alias };
            using var font12 = new SKFont() { Size = 12, Subpixel = false, Edging = SKFontEdging.Alias };
            using var font18 = new SKFont() { Size = 18, Subpixel = false, Edging = SKFontEdging.Alias };
            using var paint = new SKPaint()
            {
                Color = SKColors.Black,
                IsAntialias = false,
                HintingLevel = SKPaintHinting.NoHinting,
                IsAutohinted = false,
                SubpixelText = false,
                LcdRenderText = false
            };

            canvas.DrawText("Outside", 10, 25, font12, paint);
            canvas.DrawText(tempData?.Value("0.0") ?? "N/A", 10, 41, font18, paint);
            DrawText(canvas, "Inside", 12, info.Width / 2, 25, centerHorizontal: SKTextAlign.Center );
            DrawText(canvas, insideData?.Value("0.0") ?? "N/A", 18, info.Width / 2, 41, centerHorizontal: SKTextAlign.Center);
            DrawText(canvas, "Pool", 12, info.Width - 10, 25, centerHorizontal: SKTextAlign.Right);
            DrawText(canvas, poolData?.Value("0.0") ?? "N/A", 18, info.Width - 10, 41, centerHorizontal: SKTextAlign.Right);

            // canvas.DrawText("Pool", 230, 25, font12, paint);
            // canvas.DrawText(poolData?.Value("0.0") ?? "N/A", 230, 41, font18, paint);

            var time = DateTime.Now.ToShortTimeString();

            if (tempData?.History != null)
            {
                Queue<DatePoint> history = new Queue<DatePoint>();
                foreach (var item in tempData.History)
                {
                    if (double.TryParse(item.state, out double result))
                        history.Enqueue(new DatePoint(item.last_changed, result));
                }

                if (history.Count > 1)
                {
                    DrawGraph(canvas, new SKRect(0, 43, info.Width, info.Height - 1), history);
                }
            }
            //DrawTime(canvas, info);
            DrawTitle("Temperature", canvas, info);
            base.GetScreen(canvas, info);
        }
        public void Dispose()
        {
            HomeAssistantData.RemoveData(temperatureEntityId);
            HomeAssistantData.RemoveData("sensor.pool_temperature_temperature");
            HomeAssistantData.RemoveData("sensor.my_ecobee_temperature");
        }

        public override bool IsReady => tempData?.State != null;
        public override void Initialize()
        {
            tempData = HomeAssistantData.LogData(temperatureEntityId, true, TimeSpan.FromMinutes(1));
            tempData.Updated += TempData_Updated;

            poolData = HomeAssistantData.LogData("sensor.pool_temperature_temperature", true, TimeSpan.FromMinutes(10));
            insideData = HomeAssistantData.LogData("sensor.my_ecobee_temperature", true, TimeSpan.FromMinutes(10));
            poolData.Updated += TempData_Updated;
            insideData.Updated += TempData_Updated;
            //while(true)
            //{
            //    await Task.Delay(1000);
            //    counter++;
            //    RaiseHasChanged();
            //}
        }

        private void TempData_Updated(object? sender, EventArgs e)
        {
            base.RaiseHasChanged();
        }
    }
}
