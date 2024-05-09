using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotMorten.HomeAssistent;

namespace EPaperApp
{
    public static class HomeAssistantData
    {
        private const string HomeAssistentUrl = "http://homeassistant.local:8123";
        private const string HomeAssistentAccessToken = "HOMEASSISTANT_ACCESS_TOKEN_HERE";
        internal static HAClient HomeClient { get; } = new HAClient(Configuration.HomeAssistentAccessToken, Configuration.HomeAssistentUrl);

        private static ConcurrentDictionary<string, HAData> data = new ConcurrentDictionary<string, HAData>();

        public static HAData LogData(string entityId, bool getHistory, TimeSpan updateFrequency, bool get24delta = false, bool getTodayDelta = false)
        {
            var entry = new HAData(entityId, updateFrequency, getHistory, get24delta, getTodayDelta);
            if (!data.TryAdd(entityId, entry))
                throw new InvalidOperationException($"Entity {entityId} already added");
            return entry;
        }

        public static HAData? GetData(string entityId)
        {
            if (data.TryGetValue(entityId, out var sensorData))
            {
                return sensorData;
            }
            return null;
        }
        public static void RemoveData(string entityId)
        {
            if (data.TryRemove(entityId, out var entry))
                entry.Dispose();
        }
    }

    public class HAData : IDisposable
    {
        public TimeSpan UpdateFrequency { get; set; } = TimeSpan.FromMinutes(10);
        public string EntityId { get; }
        public SensorState? State { get; set; }
        public string? Value(string formatString)
        {
            if (State is null)
                return null;
            string unit = Unit;
            var value = ValueAsNumber();
            if (!double.IsNaN(value))
            {
                return value.ToString(formatString) + unit;
            }
            return State.state + unit;
        }
        public double ValueAsNumber()
        {
            if (State is null)
                return double.NaN;
            if (double.TryParse(State.state, out double result))
                return result;
            return double.NaN;
        }
        public string Unit =>
             State != null && State.attributes.ContainsKey("unit_of_measurement") ? State.attributes["unit_of_measurement"]?.ToString() ?? "" : "";

        public bool GetHistory { get; set; }
        public bool Get24Delta { get; set; }
        public bool GetTodayDelta { get; set; }
        public List<SensorState> History { get; private set; } = new List<SensorState>();
        public DateTime LastUpdate { get; private set; }
        public EventHandler? Updated;
        
        public HAData(string entityId, TimeSpan updateFrequency, bool getHistory, bool get24delta, bool getTodayDelta)
        {
            EntityId = entityId;
            UpdateFrequency = updateFrequency;
            GetHistory = getHistory;
            Get24Delta = get24delta;
            GetTodayDelta = getTodayDelta;
            UpdaterThread();
        }

        public double _24hDelta { get; private set; } = double.NaN;

        public double TodayDelta { get; private set; } = double.NaN;

        public async Task Update(CancellationToken cancellationToken)
        {
            bool stateChanged = false;
            Task<bool> stateTask = GetStateAsync(stateChanged, cancellationToken);
            List<Task> tasks = [stateTask];

            if (Get24Delta)
                tasks.Add(Get24HourDeltaAsync(cancellationToken));
            if (GetTodayDelta)
                tasks.Add(GetTodayDeltaAsync(cancellationToken));
            if (GetHistory)
                tasks.Add(LoadHistoryAsync(cancellationToken));
            await Task.WhenAll(tasks);
            if (cancellationToken.IsCancellationRequested) return;
            stateChanged = stateTask.Result;
            LastUpdate = DateTime.Now;
            if (stateChanged)
                Updated?.Invoke(this, EventArgs.Empty);
        }

        private async Task<bool> GetStateAsync(bool stateChanged, CancellationToken cancellationToken)
        {
            try
            {
                var newState = await HomeAssistantData.HomeClient.GetState(EntityId, cancellationToken).ConfigureAwait(false);
                if (newState.state != State?.state || newState.last_changed != State?.last_changed)
                {
                    stateChanged = true;
                }
                State = newState;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Failed to get data for {EntityId}: ({ex.Message})");
                State = null;
            }

            return stateChanged;
        }

        private async Task Get24HourDeltaAsync(CancellationToken cancellationToken)
        {
            try
            {
                var _24hState = await HomeAssistantData.HomeClient.GetHistory(EntityId, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now, minimal_response: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                var sensorHistory = Newtonsoft.Json.JsonConvert.DeserializeObject<SensorState[]>(_24hState.Substring(1, _24hState.Length - 2));
                if (sensorHistory != null)
                {
                    if (sensorHistory.Length >= 2)
                    {
                        var first = sensorHistory.OrderBy(s => s.last_changed).First();
                        var last = sensorHistory.OrderBy(s => s.last_changed).Last();
                        if (double.TryParse(first.state, out double today) && double.TryParse(last.state, out double now))
                        {
                            _24hDelta = now - today;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Failed to get 24H Delta data for {EntityId}: ({ex.Message})");
            }
        }

        private async Task GetTodayDeltaAsync(CancellationToken cancellationToken)
        {
            try
            {
                var todayState = await HomeAssistantData.HomeClient.GetHistory(EntityId, DateTimeOffset.Now.Date, DateTimeOffset.Now, minimal_response: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                var sensorHistory = Newtonsoft.Json.JsonConvert.DeserializeObject<SensorState[]>(todayState.Substring(1, todayState.Length - 2));
                if (sensorHistory != null)
                {
                    if (sensorHistory.Length >= 2)
                    {
                        var first = sensorHistory.OrderBy(s => s.last_changed).First();
                        var last = sensorHistory.OrderBy(s => s.last_changed).Last();
                        if (double.TryParse(first.state, out double today) && double.TryParse(last.state, out double now))
                        {
                            TodayDelta = now - today;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Failed to get Today Delta data for {EntityId}: ({ex.Message})");
            }
        }

        private async Task LoadHistoryAsync(CancellationToken cancellationToken)
        {
            try
            {
                var json = await HomeAssistantData.HomeClient.GetHistory(EntityId, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now, cancellationToken: cancellationToken).ConfigureAwait(false);
                var sensorHistory = Newtonsoft.Json.JsonConvert.DeserializeObject<SensorState[]>(json.Substring(1, json.Length - 2));
                if (sensorHistory != null)
                {
                    History = sensorHistory.OrderBy(s => s.last_changed).ToList();
                }
            }
            catch (System.Exception ex)
            {
                History.Clear();
                Debug.WriteLine($"Failed to get history data for {EntityId}: ({ex.Message})");
            }
        }

        public CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private async void UpdaterThread()
        {
            try
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    await Update(cancellationTokenSource.Token).ConfigureAwait(false);
                    await Task.Delay(UpdateFrequency, cancellationTokenSource.Token).ConfigureAwait(false);
                }
            }
            catch(OperationCanceledException)
            {
            }
        }
        
        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
