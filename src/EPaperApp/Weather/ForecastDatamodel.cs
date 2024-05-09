using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotMorten.WeatherGov.Gridpoints
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Geometry
    {
        [JsonConstructor]
        public Geometry(
            [JsonProperty("type")] string type,
            [JsonProperty("coordinates")] List<List<List<double>>> coordinates
        )
        {
            this.Type = type;
            this.Coordinates = coordinates;
        }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("coordinates")]
        public IReadOnlyList<List<List<double>>> Coordinates { get; }
    }

    public class Elevation
    {
        [JsonConstructor]
        public Elevation(
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

    public class Period
    {
        [JsonProperty("number")]
        public int Number { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
        [JsonProperty("temperatureUnit")]
        public string TemperatureUnit { get; set; }
        [JsonProperty("isDaytime")]
        public bool IsDaytime { get; set; }
        [JsonProperty("shortForecast")]
        public string ShortForecast { get; set; }
        [JsonProperty("detailedForecast")]
        public string DetailedForecast { get; set; }
    }

    public class Value
    {
        [JsonConstructor]
        public Value(
            [JsonProperty("validTime")] string validTime,
            [JsonProperty("value")] double? value
        )
        {
            this.ValidTime = validTime;
            this.Val = value;
        }

        [JsonProperty("validTime")]
        public string ValidTime { get; }

        [JsonProperty("value")]
        public double? Val { get; }
        public DateTimeOffset? Time
        {
            get
            {
                if (DateTimeOffset.TryParse(ValidTime.Substring(0, ValidTime.Length - 5), out DateTimeOffset value))
                    return value;
                return null;
            }
        }

    }

    public class Temperature
    {
        [JsonConstructor]
        public Temperature(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class Dewpoint
    {
        [JsonConstructor]
        public Dewpoint(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class MaxTemperature
    {
        [JsonConstructor]
        public MaxTemperature(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class MinTemperature
    {
        [JsonConstructor]
        public MinTemperature(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class RelativeHumidity
    {
        [JsonConstructor]
        public RelativeHumidity(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class ApparentTemperature
    {
        [JsonConstructor]
        public ApparentTemperature(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class HeatIndex
    {
        [JsonConstructor]
        public HeatIndex(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class WindChill
    {
        [JsonConstructor]
        public WindChill(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class SkyCover
    {
        [JsonConstructor]
        public SkyCover(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class WindDirection
    {
        [JsonConstructor]
        public WindDirection(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class WindSpeed
    {
        [JsonConstructor]
        public WindSpeed(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class WindGust
    {
        [JsonConstructor]
        public WindGust(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class Visibility
    {
        [JsonConstructor]
        public Visibility(
            [JsonProperty("unitCode")] string unitCode,
            [JsonProperty("value")] double? value,
            [JsonProperty("values")] List<object> values
        )
        {
            this.UnitCode = unitCode;
            this.Value = value;
            this.Values = values;
        }

        [JsonProperty("unitCode")]
        public string UnitCode { get; }

        [JsonProperty("value")]
        public double? Value { get; }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class Value14
    {
        [JsonConstructor]
        public Value14(
            [JsonProperty("coverage")] string coverage,
            [JsonProperty("weather")] string weather,
            [JsonProperty("intensity")] object intensity,
            [JsonProperty("visibility")] Visibility visibility,
            [JsonProperty("attributes")] List<object> attributes
        )
        {
            this.Coverage = coverage;
            this.Weather = weather;
            this.Intensity = intensity;
            this.Visibility = visibility;
            this.Attributes = attributes;
        }

        [JsonProperty("coverage")]
        public string Coverage { get; }

        [JsonProperty("weather")]
        public string Weather { get; }

        [JsonProperty("intensity")]
        public object Intensity { get; }

        [JsonProperty("visibility")]
        public Visibility Visibility { get; }

        [JsonProperty("attributes")]
        public IReadOnlyList<object> Attributes { get; }
    }

    public class Weather
    {
        [JsonConstructor]
        public Weather(
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class Hazards
    {
        [JsonConstructor]
        public Hazards(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class ProbabilityOfPrecipitation
    {
        [JsonConstructor]
        public ProbabilityOfPrecipitation(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class QuantitativePrecipitation
    {
        [JsonConstructor]
        public QuantitativePrecipitation(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class IceAccumulation
    {
        [JsonConstructor]
        public IceAccumulation(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class SnowfallAmount
    {
        [JsonConstructor]
        public SnowfallAmount(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class SnowLevel
    {
        [JsonConstructor]
        public SnowLevel(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class CeilingHeight
    {
        [JsonConstructor]
        public CeilingHeight(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class TransportWindSpeed
    {
        [JsonConstructor]
        public TransportWindSpeed(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class TransportWindDirection
    {
        [JsonConstructor]
        public TransportWindDirection(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class MixingHeight
    {
        [JsonConstructor]
        public MixingHeight(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class HainesIndex
    {
        [JsonConstructor]
        public HainesIndex(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class LightningActivityLevel
    {
        [JsonConstructor]
        public LightningActivityLevel(
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class TwentyFootWindSpeed
    {
        [JsonConstructor]
        public TwentyFootWindSpeed(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class TwentyFootWindDirection
    {
        [JsonConstructor]
        public TwentyFootWindDirection(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class WaveHeight
    {
        [JsonConstructor]
        public WaveHeight(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class WavePeriod
    {
        [JsonConstructor]
        public WavePeriod(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class WaveDirection
    {
        [JsonConstructor]
        public WaveDirection(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class PrimarySwellHeight
    {
        [JsonConstructor]
        public PrimarySwellHeight(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class PrimarySwellDirection
    {
        [JsonConstructor]
        public PrimarySwellDirection(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class SecondarySwellHeight
    {
        [JsonConstructor]
        public SecondarySwellHeight(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class SecondarySwellDirection
    {
        [JsonConstructor]
        public SecondarySwellDirection(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class WavePeriod2
    {
        [JsonConstructor]
        public WavePeriod2(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class WindWaveHeight
    {
        [JsonConstructor]
        public WindWaveHeight(
            [JsonProperty("uom")] string uom,
            [JsonProperty("values")] List<Value> values
        )
        {
            this.Uom = uom;
            this.Values = values;
        }

        [JsonProperty("uom")]
        public string Uom { get; }

        [JsonProperty("values")]
        public IReadOnlyList<Value> Values { get; }
    }

    public class DispersionIndex
    {
        [JsonConstructor]
        public DispersionIndex(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class Pressure
    {
        [JsonConstructor]
        public Pressure(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class ProbabilityOfTropicalStormWinds
    {
        [JsonConstructor]
        public ProbabilityOfTropicalStormWinds(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class ProbabilityOfHurricaneWinds
    {
        [JsonConstructor]
        public ProbabilityOfHurricaneWinds(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class PotentialOf15mphWinds
    {
        [JsonConstructor]
        public PotentialOf15mphWinds(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class PotentialOf25mphWinds
    {
        [JsonConstructor]
        public PotentialOf25mphWinds(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class PotentialOf35mphWinds
    {
        [JsonConstructor]
        public PotentialOf35mphWinds(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class PotentialOf45mphWinds
    {
        [JsonConstructor]
        public PotentialOf45mphWinds(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class PotentialOf20mphWindGusts
    {
        [JsonConstructor]
        public PotentialOf20mphWindGusts(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class PotentialOf30mphWindGusts
    {
        [JsonConstructor]
        public PotentialOf30mphWindGusts(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class PotentialOf40mphWindGusts
    {
        [JsonConstructor]
        public PotentialOf40mphWindGusts(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class PotentialOf50mphWindGusts
    {
        [JsonConstructor]
        public PotentialOf50mphWindGusts(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class PotentialOf60mphWindGusts
    {
        [JsonConstructor]
        public PotentialOf60mphWindGusts(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class GrasslandFireDangerIndex
    {
        [JsonConstructor]
        public GrasslandFireDangerIndex(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class ProbabilityOfThunder
    {
        [JsonConstructor]
        public ProbabilityOfThunder(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class DavisStabilityIndex
    {
        [JsonConstructor]
        public DavisStabilityIndex(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class AtmosphericDispersionIndex
    {
        [JsonConstructor]
        public AtmosphericDispersionIndex(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class LowVisibilityOccurrenceRiskIndex
    {
        [JsonConstructor]
        public LowVisibilityOccurrenceRiskIndex(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class Stability
    {
        [JsonConstructor]
        public Stability(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class RedFlagThreatIndex
    {
        [JsonConstructor]
        public RedFlagThreatIndex(
            [JsonProperty("values")] List<object> values
        )
        {
            this.Values = values;
        }

        [JsonProperty("values")]
        public IReadOnlyList<object> Values { get; }
    }

    public class Properties
    {
        [JsonConstructor]
        public Properties(
            [JsonProperty("@id")] string id,
            [JsonProperty("@type")] string type,
            [JsonProperty("updateTime")] DateTime updateTime,
            [JsonProperty("validTimes")] string validTimes,
            [JsonProperty("elevation")] Elevation elevation,
            [JsonProperty("forecastOffice")] string forecastOffice,
            [JsonProperty("gridId")] string gridId,
            [JsonProperty("gridX")] string gridX,
            [JsonProperty("gridY")] string gridY,
            [JsonProperty("temperature")] Temperature temperature,
            [JsonProperty("dewpoint")] Dewpoint dewpoint,
            [JsonProperty("maxTemperature")] MaxTemperature maxTemperature,
            [JsonProperty("minTemperature")] MinTemperature minTemperature,
            [JsonProperty("relativeHumidity")] RelativeHumidity relativeHumidity,
            [JsonProperty("apparentTemperature")] ApparentTemperature apparentTemperature,
            [JsonProperty("heatIndex")] HeatIndex heatIndex,
            [JsonProperty("windChill")] WindChill windChill,
            [JsonProperty("skyCover")] SkyCover skyCover,
            [JsonProperty("windDirection")] WindDirection windDirection,
            [JsonProperty("windSpeed")] WindSpeed windSpeed,
            [JsonProperty("windGust")] WindGust windGust,
            //[JsonProperty("weather")] Weather weather,
            [JsonProperty("hazards")] Hazards hazards,
            [JsonProperty("probabilityOfPrecipitation")] ProbabilityOfPrecipitation probabilityOfPrecipitation,
            [JsonProperty("quantitativePrecipitation")] QuantitativePrecipitation quantitativePrecipitation,
            [JsonProperty("iceAccumulation")] IceAccumulation iceAccumulation,
            [JsonProperty("snowfallAmount")] SnowfallAmount snowfallAmount,
            [JsonProperty("snowLevel")] SnowLevel snowLevel,
            [JsonProperty("ceilingHeight")] CeilingHeight ceilingHeight,
            [JsonProperty("visibility")] Visibility visibility,
            [JsonProperty("transportWindSpeed")] TransportWindSpeed transportWindSpeed,
            [JsonProperty("transportWindDirection")] TransportWindDirection transportWindDirection,
            [JsonProperty("mixingHeight")] MixingHeight mixingHeight,
            [JsonProperty("hainesIndex")] HainesIndex hainesIndex,
            [JsonProperty("lightningActivityLevel")] LightningActivityLevel lightningActivityLevel,
            [JsonProperty("twentyFootWindSpeed")] TwentyFootWindSpeed twentyFootWindSpeed,
            [JsonProperty("twentyFootWindDirection")] TwentyFootWindDirection twentyFootWindDirection,
            [JsonProperty("waveHeight")] WaveHeight waveHeight,
            [JsonProperty("wavePeriod")] WavePeriod wavePeriod,
            [JsonProperty("waveDirection")] WaveDirection waveDirection,
            [JsonProperty("primarySwellHeight")] PrimarySwellHeight primarySwellHeight,
            [JsonProperty("primarySwellDirection")] PrimarySwellDirection primarySwellDirection,
            [JsonProperty("secondarySwellHeight")] SecondarySwellHeight secondarySwellHeight,
            [JsonProperty("secondarySwellDirection")] SecondarySwellDirection secondarySwellDirection,
            [JsonProperty("wavePeriod2")] WavePeriod2 wavePeriod2,
            [JsonProperty("windWaveHeight")] WindWaveHeight windWaveHeight,
            [JsonProperty("dispersionIndex")] DispersionIndex dispersionIndex,
            [JsonProperty("pressure")] Pressure pressure,
            [JsonProperty("probabilityOfTropicalStormWinds")] ProbabilityOfTropicalStormWinds probabilityOfTropicalStormWinds,
            [JsonProperty("probabilityOfHurricaneWinds")] ProbabilityOfHurricaneWinds probabilityOfHurricaneWinds,
            [JsonProperty("potentialOf15mphWinds")] PotentialOf15mphWinds potentialOf15mphWinds,
            [JsonProperty("potentialOf25mphWinds")] PotentialOf25mphWinds potentialOf25mphWinds,
            [JsonProperty("potentialOf35mphWinds")] PotentialOf35mphWinds potentialOf35mphWinds,
            [JsonProperty("potentialOf45mphWinds")] PotentialOf45mphWinds potentialOf45mphWinds,
            [JsonProperty("potentialOf20mphWindGusts")] PotentialOf20mphWindGusts potentialOf20mphWindGusts,
            [JsonProperty("potentialOf30mphWindGusts")] PotentialOf30mphWindGusts potentialOf30mphWindGusts,
            [JsonProperty("potentialOf40mphWindGusts")] PotentialOf40mphWindGusts potentialOf40mphWindGusts,
            [JsonProperty("potentialOf50mphWindGusts")] PotentialOf50mphWindGusts potentialOf50mphWindGusts,
            [JsonProperty("potentialOf60mphWindGusts")] PotentialOf60mphWindGusts potentialOf60mphWindGusts,
            [JsonProperty("grasslandFireDangerIndex")] GrasslandFireDangerIndex grasslandFireDangerIndex,
            [JsonProperty("probabilityOfThunder")] ProbabilityOfThunder probabilityOfThunder,
            [JsonProperty("davisStabilityIndex")] DavisStabilityIndex davisStabilityIndex,
            [JsonProperty("atmosphericDispersionIndex")] AtmosphericDispersionIndex atmosphericDispersionIndex,
            [JsonProperty("lowVisibilityOccurrenceRiskIndex")] LowVisibilityOccurrenceRiskIndex lowVisibilityOccurrenceRiskIndex,
            [JsonProperty("stability")] Stability stability,
            [JsonProperty("redFlagThreatIndex")] RedFlagThreatIndex redFlagThreatIndex,
            [JsonProperty("periods")] Period[] periods
        )
        {
            this.Id = id;
            this.Type = type;
            this.UpdateTime = updateTime;
            this.ValidTimes = validTimes;
            this.Elevation = elevation;
            this.ForecastOffice = forecastOffice;
            this.GridId = gridId;
            this.GridX = gridX;
            this.GridY = gridY;
            this.Temperature = temperature;
            this.Dewpoint = dewpoint;
            this.MaxTemperature = maxTemperature;
            this.MinTemperature = minTemperature;
            this.RelativeHumidity = relativeHumidity;
            this.ApparentTemperature = apparentTemperature;
            this.HeatIndex = heatIndex;
            this.WindChill = windChill;
            this.SkyCover = skyCover;
            this.WindDirection = windDirection;
            this.WindSpeed = windSpeed;
            this.WindGust = windGust;
            //this.Weather = weather;
            this.Hazards = hazards;
            this.ProbabilityOfPrecipitation = probabilityOfPrecipitation;
            this.QuantitativePrecipitation = quantitativePrecipitation;
            this.IceAccumulation = iceAccumulation;
            this.SnowfallAmount = snowfallAmount;
            this.SnowLevel = snowLevel;
            this.CeilingHeight = ceilingHeight;
            this.Visibility = visibility;
            this.TransportWindSpeed = transportWindSpeed;
            this.TransportWindDirection = transportWindDirection;
            this.MixingHeight = mixingHeight;
            this.HainesIndex = hainesIndex;
            this.LightningActivityLevel = lightningActivityLevel;
            this.TwentyFootWindSpeed = twentyFootWindSpeed;
            this.TwentyFootWindDirection = twentyFootWindDirection;
            this.WaveHeight = waveHeight;
            this.WavePeriod = wavePeriod;
            this.WaveDirection = waveDirection;
            this.PrimarySwellHeight = primarySwellHeight;
            this.PrimarySwellDirection = primarySwellDirection;
            this.SecondarySwellHeight = secondarySwellHeight;
            this.SecondarySwellDirection = secondarySwellDirection;
            this.WavePeriod2 = wavePeriod2;
            this.WindWaveHeight = windWaveHeight;
            this.DispersionIndex = dispersionIndex;
            this.Pressure = pressure;
            this.ProbabilityOfTropicalStormWinds = probabilityOfTropicalStormWinds;
            this.ProbabilityOfHurricaneWinds = probabilityOfHurricaneWinds;
            this.PotentialOf15mphWinds = potentialOf15mphWinds;
            this.PotentialOf25mphWinds = potentialOf25mphWinds;
            this.PotentialOf35mphWinds = potentialOf35mphWinds;
            this.PotentialOf45mphWinds = potentialOf45mphWinds;
            this.PotentialOf20mphWindGusts = potentialOf20mphWindGusts;
            this.PotentialOf30mphWindGusts = potentialOf30mphWindGusts;
            this.PotentialOf40mphWindGusts = potentialOf40mphWindGusts;
            this.PotentialOf50mphWindGusts = potentialOf50mphWindGusts;
            this.PotentialOf60mphWindGusts = potentialOf60mphWindGusts;
            this.GrasslandFireDangerIndex = grasslandFireDangerIndex;
            this.ProbabilityOfThunder = probabilityOfThunder;
            this.DavisStabilityIndex = davisStabilityIndex;
            this.AtmosphericDispersionIndex = atmosphericDispersionIndex;
            this.LowVisibilityOccurrenceRiskIndex = lowVisibilityOccurrenceRiskIndex;
            this.Stability = stability;
            this.RedFlagThreatIndex = redFlagThreatIndex;
            this.Periods = periods;
        }

        [JsonProperty("@id")]
        public string Id { get; }

        [JsonProperty("@type")]
        public string Type { get; }

        [JsonProperty("updateTime")]
        public DateTime UpdateTime { get; }

        [JsonProperty("validTimes")]
        public string ValidTimes { get; }

        [JsonProperty("elevation")]
        public Elevation Elevation { get; }

        [JsonProperty("forecastOffice")]
        public string ForecastOffice { get; }

        [JsonProperty("gridId")]
        public string GridId { get; }

        [JsonProperty("gridX")]
        public string GridX { get; }

        [JsonProperty("gridY")]
        public string GridY { get; }

        [JsonProperty("temperature")]
        public Temperature Temperature { get; }

        [JsonProperty("dewpoint")]
        public Dewpoint Dewpoint { get; }

        [JsonProperty("maxTemperature")]
        public MaxTemperature MaxTemperature { get; }

        [JsonProperty("minTemperature")]
        public MinTemperature MinTemperature { get; }

        [JsonProperty("relativeHumidity")]
        public RelativeHumidity RelativeHumidity { get; }

        [JsonProperty("apparentTemperature")]
        public ApparentTemperature ApparentTemperature { get; }

        [JsonProperty("heatIndex")]
        public HeatIndex HeatIndex { get; }

        [JsonProperty("windChill")]
        public WindChill WindChill { get; }

        [JsonProperty("skyCover")]
        public SkyCover SkyCover { get; }

        [JsonProperty("windDirection")]
        public WindDirection WindDirection { get; }

        [JsonProperty("windSpeed")]
        public WindSpeed WindSpeed { get; }

        [JsonProperty("windGust")]
        public WindGust WindGust { get; }

        //[JsonProperty("weather")]
        //public Weather Weather { get; }

        [JsonProperty("hazards")]
        public Hazards Hazards { get; }

        [JsonProperty("probabilityOfPrecipitation")]
        public ProbabilityOfPrecipitation ProbabilityOfPrecipitation { get; }

        [JsonProperty("quantitativePrecipitation")]
        public QuantitativePrecipitation QuantitativePrecipitation { get; }

        [JsonProperty("iceAccumulation")]
        public IceAccumulation IceAccumulation { get; }

        [JsonProperty("snowfallAmount")]
        public SnowfallAmount SnowfallAmount { get; }

        [JsonProperty("snowLevel")]
        public SnowLevel SnowLevel { get; }

        [JsonProperty("ceilingHeight")]
        public CeilingHeight CeilingHeight { get; }

        [JsonProperty("visibility")]
        public Visibility Visibility { get; }

        [JsonProperty("transportWindSpeed")]
        public TransportWindSpeed TransportWindSpeed { get; }

        [JsonProperty("transportWindDirection")]
        public TransportWindDirection TransportWindDirection { get; }

        [JsonProperty("mixingHeight")]
        public MixingHeight MixingHeight { get; }

        [JsonProperty("hainesIndex")]
        public HainesIndex HainesIndex { get; }

        [JsonProperty("lightningActivityLevel")]
        public LightningActivityLevel LightningActivityLevel { get; }

        [JsonProperty("twentyFootWindSpeed")]
        public TwentyFootWindSpeed TwentyFootWindSpeed { get; }

        [JsonProperty("twentyFootWindDirection")]
        public TwentyFootWindDirection TwentyFootWindDirection { get; }

        [JsonProperty("waveHeight")]
        public WaveHeight WaveHeight { get; }

        [JsonProperty("wavePeriod")]
        public WavePeriod WavePeriod { get; }

        [JsonProperty("waveDirection")]
        public WaveDirection WaveDirection { get; }

        [JsonProperty("primarySwellHeight")]
        public PrimarySwellHeight PrimarySwellHeight { get; }

        [JsonProperty("primarySwellDirection")]
        public PrimarySwellDirection PrimarySwellDirection { get; }

        [JsonProperty("secondarySwellHeight")]
        public SecondarySwellHeight SecondarySwellHeight { get; }

        [JsonProperty("secondarySwellDirection")]
        public SecondarySwellDirection SecondarySwellDirection { get; }

        [JsonProperty("wavePeriod2")]
        public WavePeriod2 WavePeriod2 { get; }

        [JsonProperty("windWaveHeight")]
        public WindWaveHeight WindWaveHeight { get; }

        [JsonProperty("dispersionIndex")]
        public DispersionIndex DispersionIndex { get; }

        [JsonProperty("pressure")]
        public Pressure Pressure { get; }

        [JsonProperty("probabilityOfTropicalStormWinds")]
        public ProbabilityOfTropicalStormWinds ProbabilityOfTropicalStormWinds { get; }

        [JsonProperty("probabilityOfHurricaneWinds")]
        public ProbabilityOfHurricaneWinds ProbabilityOfHurricaneWinds { get; }

        [JsonProperty("potentialOf15mphWinds")]
        public PotentialOf15mphWinds PotentialOf15mphWinds { get; }

        [JsonProperty("potentialOf25mphWinds")]
        public PotentialOf25mphWinds PotentialOf25mphWinds { get; }

        [JsonProperty("potentialOf35mphWinds")]
        public PotentialOf35mphWinds PotentialOf35mphWinds { get; }

        [JsonProperty("potentialOf45mphWinds")]
        public PotentialOf45mphWinds PotentialOf45mphWinds { get; }

        [JsonProperty("potentialOf20mphWindGusts")]
        public PotentialOf20mphWindGusts PotentialOf20mphWindGusts { get; }

        [JsonProperty("potentialOf30mphWindGusts")]
        public PotentialOf30mphWindGusts PotentialOf30mphWindGusts { get; }

        [JsonProperty("potentialOf40mphWindGusts")]
        public PotentialOf40mphWindGusts PotentialOf40mphWindGusts { get; }

        [JsonProperty("potentialOf50mphWindGusts")]
        public PotentialOf50mphWindGusts PotentialOf50mphWindGusts { get; }

        [JsonProperty("potentialOf60mphWindGusts")]
        public PotentialOf60mphWindGusts PotentialOf60mphWindGusts { get; }

        [JsonProperty("grasslandFireDangerIndex")]
        public GrasslandFireDangerIndex GrasslandFireDangerIndex { get; }

        [JsonProperty("probabilityOfThunder")]
        public ProbabilityOfThunder ProbabilityOfThunder { get; }

        [JsonProperty("davisStabilityIndex")]
        public DavisStabilityIndex DavisStabilityIndex { get; }

        [JsonProperty("atmosphericDispersionIndex")]
        public AtmosphericDispersionIndex AtmosphericDispersionIndex { get; }

        [JsonProperty("lowVisibilityOccurrenceRiskIndex")]
        public LowVisibilityOccurrenceRiskIndex LowVisibilityOccurrenceRiskIndex { get; }

        [JsonProperty("stability")]
        public Stability Stability { get; }

        [JsonProperty("redFlagThreatIndex")]
        public RedFlagThreatIndex RedFlagThreatIndex { get; }
        [JsonProperty("periods")]
        public Period[] Periods { get; }
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