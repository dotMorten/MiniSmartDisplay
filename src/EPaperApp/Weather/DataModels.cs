using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotMorten.WeatherGov.Points
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Geometry
    {
        [JsonConstructor]
        public Geometry(
            [JsonProperty("type")] string type,
            [JsonProperty("coordinates")] List<double> coordinates
        )
        {
            this.Type = type;
            this.Coordinates = coordinates;
        }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("coordinates")]
        public IReadOnlyList<double> Coordinates { get; }
    }

    public class Distance
    {
        [JsonConstructor]
        public Distance(
            [JsonProperty("unitCode")] string unitCode,
            [JsonProperty("value")] double value
        )
        {
            this.UnitCode = unitCode;
            this.Value = value;
        }

        [JsonProperty("unitCode")]
        public string UnitCode { get; }

        [JsonProperty("value")]
        public double Value { get; }
    }

    public class Bearing
    {
        [JsonConstructor]
        public Bearing(
            [JsonProperty("unitCode")] string unitCode,
            [JsonProperty("value")] int value
        )
        {
            this.UnitCode = unitCode;
            this.Value = value;
        }

        [JsonProperty("unitCode")]
        public string UnitCode { get; }

        [JsonProperty("value")]
        public int Value { get; }
    }

    public class Properties
    {
        [JsonConstructor]
        public Properties(
            [JsonProperty("city")] string city,
            [JsonProperty("state")] string state,
            [JsonProperty("distance")] Distance distance,
            [JsonProperty("bearing")] Bearing bearing,
            [JsonProperty("@id")] string id,
            [JsonProperty("@type")] string type,
            [JsonProperty("cwa")] string cwa,
            [JsonProperty("forecastOffice")] string forecastOffice,
            [JsonProperty("gridId")] string gridId,
            [JsonProperty("gridX")] int gridX,
            [JsonProperty("gridY")] int gridY,
            [JsonProperty("forecast")] string forecast,
            [JsonProperty("forecastHourly")] string forecastHourly,
            [JsonProperty("forecastGridData")] string forecastGridData,
            [JsonProperty("observationStations")] string observationStations,
            [JsonProperty("relativeLocation")] RelativeLocation relativeLocation,
            [JsonProperty("forecastZone")] string forecastZone,
            [JsonProperty("county")] string county,
            [JsonProperty("fireWeatherZone")] string fireWeatherZone,
            [JsonProperty("timeZone")] string timeZone,
            [JsonProperty("radarStation")] string radarStation
        )
        {
            this.City = city;
            this.State = state;
            this.Distance = distance;
            this.Bearing = bearing;
            this.Id = id;
            this.Type = type;
            this.Cwa = cwa;
            this.ForecastOffice = forecastOffice;
            this.GridId = gridId;
            this.GridX = gridX;
            this.GridY = gridY;
            this.Forecast = forecast;
            this.ForecastHourly = forecastHourly;
            this.ForecastGridData = forecastGridData;
            this.ObservationStations = observationStations;
            this.RelativeLocation = relativeLocation;
            this.ForecastZone = forecastZone;
            this.County = county;
            this.FireWeatherZone = fireWeatherZone;
            this.TimeZone = timeZone;
            this.RadarStation = radarStation;
        }

        [JsonProperty("city")]
        public string City { get; }

        [JsonProperty("state")]
        public string State { get; }

        [JsonProperty("distance")]
        public Distance Distance { get; }

        [JsonProperty("bearing")]
        public Bearing Bearing { get; }

        [JsonProperty("@id")]
        public string Id { get; }

        [JsonProperty("@type")]
        public string Type { get; }

        [JsonProperty("cwa")]
        public string Cwa { get; }

        [JsonProperty("forecastOffice")]
        public string ForecastOffice { get; }

        [JsonProperty("gridId")]
        public string GridId { get; }

        [JsonProperty("gridX")]
        public int GridX { get; }

        [JsonProperty("gridY")]
        public int GridY { get; }

        [JsonProperty("forecast")]
        public string Forecast { get; }

        [JsonProperty("forecastHourly")]
        public string ForecastHourly { get; }

        [JsonProperty("forecastGridData")]
        public string ForecastGridData { get; }

        [JsonProperty("observationStations")]
        public string ObservationStations { get; }

        [JsonProperty("relativeLocation")]
        public RelativeLocation RelativeLocation { get; }

        [JsonProperty("forecastZone")]
        public string ForecastZone { get; }

        [JsonProperty("county")]
        public string County { get; }

        [JsonProperty("fireWeatherZone")]
        public string FireWeatherZone { get; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; }

        [JsonProperty("radarStation")]
        public string RadarStation { get; }
    }

    public class RelativeLocation
    {
        [JsonConstructor]
        public RelativeLocation(
            [JsonProperty("type")] string type,
            [JsonProperty("geometry")] Geometry geometry,
            [JsonProperty("properties")] Properties properties
        )
        {
            this.Type = type;
            this.Geometry = geometry;
            this.Properties = properties;
        }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; }

        [JsonProperty("properties")]
        public Properties Properties { get; }
    }

    public class Root
    {
        [JsonConstructor]
        public Root(
            [JsonProperty("@context")] List<object> context,
            [JsonProperty("id")] string id,
            [JsonProperty("type")] string type,
            [JsonProperty("geometry")] Geometry geometry,
            [JsonProperty("properties")] Properties properties
        )
        {
            this.Context = context;
            this.Id = id;
            this.Type = type;
            this.Geometry = geometry;
            this.Properties = properties;
        }

        [JsonProperty("@context")]
        public IReadOnlyList<object> Context { get; }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; }

        [JsonProperty("properties")]
        public Properties Properties { get; }
    }


}