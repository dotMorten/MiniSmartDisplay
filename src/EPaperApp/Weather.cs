using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotMorten.HomeAssistent;
using dotMorten.WeatherGov;
using SkiaSharp;

namespace EPaperApp
{
    internal class WeatherScreen : ScreenBase, IDisposable
    {
        private bool isDisposed;
        private dotMorten.WeatherGov.Gridpoints.Root? forecast;
        private dotMorten.WeatherGov.Gridpoints.Root? forecastHourly;

        public override bool IsReady => forecast != null;

        private string temperatureEntityId = "sensor.tempest_temperature";
        private string uvEntityId = "sensor.tempest_uv_index";
        private string wind_directionEntityId = "sensor.tempest_wind_direction";
        private string precipitationEntityId = "sensor.tempest_precipitation";
        private string wind_speedEntityId = "sensor.tempest_wind_speed_average";
		
        public override void GetScreen(SKCanvas canvas, SKImageInfo info)
        {
            canvas.Clear(SKColors.White);

            float height = 0;
            if(forecast?.Properties?.Periods != null && forecast?.Properties?.Periods.Length > 0)
            {
                var next = forecast.Properties.Periods[0];
                height = DrawText(canvas, next.DetailedForecast, 11, 5, 25, maxwidth: info.Width - 10);
                var days = forecast.Properties.Periods.Skip(1).Where(p=>p.IsDaytime).Take(4).ToList();
                var itemWidth = info.Width / days.Count;
                float x = itemWidth / 2;
                canvas.DrawLine(0,88,info.Width,88, new SKPaint() { Color = SKColors.Black, StrokeWidth = 1 });
                foreach (var period in forecast.Properties.Periods.Skip(1).Where(p=>p.IsDaytime))
                {
                    Debug.WriteLine($"{period.Name} {period.Temperature} {period.ShortForecast} {period.DetailedForecast}");
                    DrawText(canvas, period.Name, 11, x, 100, centerHorizontal: SKTextAlign.Center);
                    DrawText(canvas, period.Temperature.ToString("0°"), 14, x, 114, centerHorizontal: SKTextAlign.Center);
                    DrawText(canvas, period.ShortForecast, 11, x, 125, centerHorizontal: SKTextAlign.Center, maxwidth: itemWidth - 1);
                    x+= itemWidth;
                    
                }
                //forecast.Properties.MaxTemperature.Values[0].ValidTime;
            }
            // canvas.DrawText("Pool", 230, 25, font12, paint);
            // canvas.DrawText(poolData?.Value("0.0") ?? "N/A", 230, 41, font18, paint);
            var data = HomeAssistantData.GetData(temperatureEntityId);
            height += 15;
            if(data != null)
            {
                DrawText(canvas, data.ValueAsNumber().ToString("0°"), 24, 50, height, centerHorizontal: SKTextAlign.Center);
                var now = forecastHourly?.Properties?.Periods?.FirstOrDefault();
                if (now != null)
                {
                    DrawText(canvas, now.ShortForecast, 10, 50, height + 10, centerHorizontal: SKTextAlign.Center);
                }
            }
            data = HomeAssistantData.GetData(wind_speedEntityId);
            if (data != null)
            {
                DrawText(canvas, data.ValueAsNumber().ToString("0"), 24, info.Width/2, height, centerHorizontal: SKTextAlign.Center);
                //DrawText(canvas, data.Unit, 15, 70, 70, centerHorizontal: SKTextAlign.Center);
            }
            data = HomeAssistantData.GetData(wind_directionEntityId);
            if (data != null)
            {
                DrawText(canvas, winddir(data.ValueAsNumber()), 10, info.Width/2, height + 10, centerHorizontal: SKTextAlign.Center);
            }

            data = HomeAssistantData.GetData(uvEntityId);
            if (data != null)
            {
                DrawText(canvas, data.ValueAsNumber().ToString("0.0"), 24, 230, height, centerHorizontal: SKTextAlign.Center);
                DrawText(canvas, data.Unit, 10, 230, height + 10, centerHorizontal: SKTextAlign.Center);
            }


            //DrawTime(canvas, info);
            DrawTitle("Weather", canvas, info);
            base.GetScreen(canvas, info);
        }
        public void Dispose()
        {
            disposeToken.Cancel();
            HomeAssistantData.RemoveData(temperatureEntityId);
            HomeAssistantData.RemoveData(uvEntityId);
            HomeAssistantData.RemoveData(wind_directionEntityId);
            HomeAssistantData.RemoveData(wind_speedEntityId);
            HomeAssistantData.RemoveData(precipitationEntityId);
        }

        public override void Initialize()
        {
            HomeAssistantData.LogData(uvEntityId, true, TimeSpan.FromMinutes(5));
            HomeAssistantData.LogData(wind_directionEntityId, true, TimeSpan.FromMinutes(1));
            HomeAssistantData.LogData(wind_speedEntityId, true, TimeSpan.FromMinutes(1));
            HomeAssistantData.LogData(precipitationEntityId, true, TimeSpan.FromMinutes(10));

            _ = UpdaterThread();
        }
        private static string winddir(double wd)
        {
            while (wd < 0) wd += 360;
            while (wd > 360) wd -= 360;
            switch (wd)
            {
                case <= 11.25: return "N";
                case <= 33.75: return "NNE";
                case <= 56.25: return "NE";
                case <= 78.75: return "ENE";
                case <= 101.25: return "E";
                case <= 123.75: return "ESE";
                case <= 146.25: return "SE";
                case <= 168.75: return "SSE";
                case <= 191.25: return "S";
                case <= 213.75: return "SSW";
                case <= 236.25: return "SW";
                case <= 258.75: return "WSW";
                case <= 281.25: return "W";
                case <= 303.75: return "WNW";
                case <= 326.25: return "NW";
                case <= 348.75: return "NNW";
                default: return "N";
            }
        }

        CancellationTokenSource disposeToken = new CancellationTokenSource();
        private async Task UpdaterThread()
        {
            Client weatherClient = await Client.CreateWeatherClientAsync(34.05, -117.18).ConfigureAwait(false);

            while (!disposeToken.IsCancellationRequested)
            {
                try
                {
                    forecast = await weatherClient.GetDailyForecastAsync().ConfigureAwait(false);
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine($"ForecastScreen.UpdaterThread: {ex.Message}");
                }
                if (disposeToken.IsCancellationRequested)
                    return;
                try
                {
                    forecastHourly = await weatherClient.GetHourlyForecastAsync().ConfigureAwait(false);
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine($"ForecastScreen.UpdaterThread: {ex.Message}");
                }
                if (disposeToken.IsCancellationRequested)
                    return;

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(15), disposeToken.Token).ConfigureAwait(false);
                }
                catch { }
            }
        }
    }
}
