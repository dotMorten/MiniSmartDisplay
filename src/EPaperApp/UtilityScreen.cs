using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotMorten.HomeAssistent;
using SkiaSharp;

namespace EPaperApp
{
    internal class UtilitiesScreen : ScreenBase, IDisposable
    {
        private string current_meter_power_demand = "sensor.eagle_200_meter_power_demand";
        private string total_meter_energy_delivered = "sensor.eagle_200_total_meter_energy_delivered";

        private string flume_sensor_home_24_hours = "sensor.flume_sensor_home_24_hours"; // Past 24 hour water usage
        private string flume_sensor_home_current_day = "sensor.flume_sensor_home_current_day";// Today's water use
        private string flume_sensor_home_current = "sensor.flume_sensor_home_current";// Current water flow
        
        private string solar_current_power_production = "sensor.envoy_121502008945_current_power_production"; // Current power production
        private string solar_today_s_energy_production = "sensor.envoy_121502008945_today_s_energy_production";
        private string solar_lifetime_energy_production = "sensor.envoy_121502008945_lifetime_energy_production";

        public override void Initialize()
        {
            var powerData = HomeAssistantData.LogData(current_meter_power_demand, true, TimeSpan.FromSeconds(10));
            powerData.Updated += Data_Updated;
            var data = HomeAssistantData.LogData(total_meter_energy_delivered, true, TimeSpan.FromMinutes(10), get24delta: true, getTodayDelta: true);

            HomeAssistantData.LogData(flume_sensor_home_24_hours, true, TimeSpan.FromMinutes(10));
            HomeAssistantData.LogData(flume_sensor_home_current_day, true, TimeSpan.FromMinutes(10));
            var waterData = HomeAssistantData.LogData(flume_sensor_home_current, true, TimeSpan.FromSeconds(10));
            waterData.Updated += Data_Updated;

            HomeAssistantData.LogData(solar_current_power_production, true, TimeSpan.FromMinutes(5));
            HomeAssistantData.LogData(solar_today_s_energy_production, true, TimeSpan.FromMinutes(5));
            data = HomeAssistantData.LogData(solar_lifetime_energy_production, true, TimeSpan.FromMinutes(10), get24delta: true);
        }

        private string FormatValue(double? value, string formatString, double multiplier = 1)
        {
            if (!value.HasValue || double.IsNaN(value.Value))
                return "N/A";
            return (value.Value * multiplier).ToString(formatString);
        }

        public override void GetScreen(SKCanvas canvas, SKImageInfo info)
        {
            canvas.Clear(SKColors.White);

            int x = 100;
            int y = 40;
            int voffset = 20;
            float fontsize = 14;

            DrawText(canvas, "C", 10, 2, y + voffset - 2, centerHorizontal: SKTextAlign.Left);
            DrawText(canvas, "T", 10, 2, y + voffset*2 - 2, centerHorizontal: SKTextAlign.Left);
            DrawText(canvas, "24", 10, 2, y + voffset*3 - 2, centerHorizontal: SKTextAlign.Left);

            DrawText(canvas, "Electricity", 10, x, y, centerHorizontal: SKTextAlign.Right, bold: true);
            DrawText(canvas, FormatValue(HomeAssistantData.GetData(current_meter_power_demand)?.ValueAsNumber(), "0W", 1000), fontsize, x, y+voffset, centerHorizontal: SKTextAlign.Right);
            DrawText(canvas, FormatValue(HomeAssistantData.GetData(total_meter_energy_delivered)?.TodayDelta, "0Wh", 1000), fontsize, x, y+voffset*2, centerHorizontal: SKTextAlign.Right);
            DrawText(canvas, FormatValue(HomeAssistantData.GetData(total_meter_energy_delivered)?._24hDelta, "0Wh", 1000), fontsize, x, y+voffset*3, centerHorizontal: SKTextAlign.Right);

            x = 185;
            DrawText(canvas, "Solar", 10, x, y, centerHorizontal: SKTextAlign.Right, bold: true);
            DrawText(canvas, HomeAssistantData.GetData(solar_current_power_production)?.Value("0") ?? "N/A", fontsize, x, y+voffset, centerHorizontal: SKTextAlign.Right);
            DrawText(canvas, HomeAssistantData.GetData(solar_today_s_energy_production)?.Value("0")?.ToString() ?? "N/A", fontsize, x, y+voffset*2, centerHorizontal: SKTextAlign.Right);
            DrawText(canvas, FormatValue(HomeAssistantData.GetData(solar_lifetime_energy_production)?._24hDelta, "0Wh"), fontsize, x, y+voffset*3, centerHorizontal: SKTextAlign.Right);

            x = 275;
            DrawText(canvas, "Water", 10, x, y, centerHorizontal: SKTextAlign.Right, bold: true);
            DrawText(canvas, HomeAssistantData.GetData(flume_sensor_home_current)?.Value("0.0") ?? "N/A", fontsize, x, y+voffset, centerHorizontal: SKTextAlign.Right);
            DrawText(canvas, HomeAssistantData.GetData(flume_sensor_home_current_day)?.Value("0") ?? "N/A", fontsize, x, y+voffset*2, centerHorizontal: SKTextAlign.Right);
            DrawText(canvas, HomeAssistantData.GetData(flume_sensor_home_24_hours)?.Value("0") ?? "N/A", fontsize, x, y+voffset*3, centerHorizontal: SKTextAlign.Right);

            //DrawTime(canvas, info);
            DrawTitle("Utilities", canvas, info);
            base.GetScreen(canvas, info);
        }
        public void Dispose()
        {
            HomeAssistantData.RemoveData(current_meter_power_demand);
            HomeAssistantData.RemoveData(total_meter_energy_delivered);
            HomeAssistantData.RemoveData(flume_sensor_home_24_hours);
            HomeAssistantData.RemoveData(flume_sensor_home_current_day);
            HomeAssistantData.RemoveData(flume_sensor_home_current);
            HomeAssistantData.RemoveData(solar_current_power_production);
            HomeAssistantData.RemoveData(solar_today_s_energy_production);
            HomeAssistantData.RemoveData(solar_lifetime_energy_production);
        }

        public override bool IsReady => true;
     

        private void Data_Updated(object? sender, EventArgs e)
        {
            base.RaiseHasChanged();
        }
    }
}
