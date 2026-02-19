using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EntropySyndicate.Data;
using EntropySyndicate.Utils;
using UnityEngine;

namespace EntropySyndicate.Services
{
    public class AnalyticsService
    {
        [Serializable]
        private struct AnalyticsEvent
        {
            public string eventName;
            public float timestamp;
            public string payload;
        }

        [Serializable]
        private struct EventCollection
        {
            public List<AnalyticsEvent> events;
        }

        public struct QaRunSummary
        {
            public float firstCast;
            public float firstReward;
            public float firstDamage;
            public float runDuration;
            public string durationBucket;
            public float entropyPeak;
            public int energyDepletions;
            public string topShard1;
            public string topShard2;
            public string topShard3;
        }

        private readonly List<AnalyticsEvent> _events = new List<AnalyticsEvent>(1024);
        private readonly Dictionary<string, int> _shardUsage = new Dictionary<string, int>(32);

        private float _sessionStart;
        private float _firstShardCast = -1f;
        private float _firstDamageTaken = -1f;
        private float _firstEntropySpike = -1f;
        private float _firstReward = -1f;
        private float _entropyPeak;
        private float _lastRunDuration;
        private int _energyDepletionCount;
        private readonly List<float> _runDurations = new List<float>(64);
        private bool _debugOverlayEnabled;
        private bool _verboseLogs = true;

        public void Configure(BuildRuntimeConfig runtimeConfig)
        {
            _verboseLogs = runtimeConfig != null && runtimeConfig.debugMode && !runtimeConfig.productionMode;
        }

        public void BeginSession()
        {
            _sessionStart = Time.realtimeSinceStartup;
        }

        public void BeginRun()
        {
            _firstShardCast = -1f;
            _firstDamageTaken = -1f;
            _firstEntropySpike = -1f;
            _firstReward = -1f;
            _entropyPeak = 0f;
            _lastRunDuration = 0f;
            _energyDepletionCount = 0;
            _shardUsage.Clear();
        }

        public void SetEntropyPeak(float value)
        {
            _entropyPeak = value;
        }

        public void LogEvent(string eventName, string payload = "")
        {
            _events.Add(new AnalyticsEvent
            {
                eventName = eventName,
                timestamp = Time.realtimeSinceStartup,
                payload = payload
            });
        }

        public void MarkFirstShardCast(float runSeconds)
        {
            if (_firstShardCast >= 0f) return;
            _firstShardCast = runSeconds;
            LogEvent("time_first_shard_cast", runSeconds.ToString("F2"));
            if (_verboseLogs) AppLog.Info("[Analytics] First shard cast @ " + runSeconds.ToString("F2") + "s");
        }

        public void MarkFirstDamageTaken(float runSeconds)
        {
            if (_firstDamageTaken >= 0f) return;
            _firstDamageTaken = runSeconds;
            LogEvent("time_first_damage_taken", runSeconds.ToString("F2"));
            if (_verboseLogs) AppLog.Info("[Analytics] First damage @ " + runSeconds.ToString("F2") + "s");
        }

        public void MarkFirstEntropySpike(float runSeconds)
        {
            if (_firstEntropySpike >= 0f) return;
            _firstEntropySpike = runSeconds;
            LogEvent("time_first_entropy_spike", runSeconds.ToString("F2"));
        }

        public void MarkFirstReward(float runSeconds)
        {
            if (_firstReward >= 0f) return;
            _firstReward = runSeconds;
            LogEvent("time_first_reward", runSeconds.ToString("F2"));
            if (_verboseLogs) AppLog.Info("[Analytics] First reward @ " + runSeconds.ToString("F2") + "s");
        }

        public void MarkEnergyDepleted()
        {
            _energyDepletionCount++;
        }

        public void MarkShardUsage(string shardType)
        {
            if (_shardUsage.TryGetValue(shardType, out int count))
            {
                _shardUsage[shardType] = count + 1;
            }
            else
            {
                _shardUsage[shardType] = 1;
            }
        }

        public void EndRun(float runDuration)
        {
            _lastRunDuration = runDuration;
            _runDurations.Add(runDuration);
            LogEvent("run_duration_distribution", runDuration.ToString("F2"));
            LogEvent("energy_depletion_frequency", _energyDepletionCount.ToString());
            PrintSessionSummary();
        }

        public void TrackAbandonmentTime(float runSeconds)
        {
            LogEvent("abandonment_time", runSeconds.ToString("F2"));
        }

        public void SetDebugOverlayEnabled(bool enabled)
        {
            _debugOverlayEnabled = enabled;
            LogEvent("debug_overlay_toggled", enabled ? "1" : "0");
        }

        public bool IsDebugOverlayEnabled()
        {
            return _debugOverlayEnabled;
        }

        public QaRunSummary GetQaRunSummary()
        {
            QaRunSummary summary = new QaRunSummary
            {
                firstCast = _firstShardCast,
                firstReward = _firstReward,
                firstDamage = _firstDamageTaken,
                runDuration = _lastRunDuration,
                durationBucket = GetDurationBucket(_lastRunDuration),
                entropyPeak = _entropyPeak,
                energyDepletions = _energyDepletionCount,
                topShard1 = "-",
                topShard2 = "-",
                topShard3 = "-"
            };

            string best1 = "-";
            string best2 = "-";
            string best3 = "-";
            int c1 = -1;
            int c2 = -1;
            int c3 = -1;

            foreach (KeyValuePair<string, int> kv in _shardUsage)
            {
                if (kv.Value > c1)
                {
                    c3 = c2; best3 = best2;
                    c2 = c1; best2 = best1;
                    c1 = kv.Value; best1 = kv.Key + "(" + kv.Value + ")";
                }
                else if (kv.Value > c2)
                {
                    c3 = c2; best3 = best2;
                    c2 = kv.Value; best2 = kv.Key + "(" + kv.Value + ")";
                }
                else if (kv.Value > c3)
                {
                    c3 = kv.Value; best3 = kv.Key + "(" + kv.Value + ")";
                }
            }

            summary.topShard1 = best1;
            summary.topShard2 = best2;
            summary.topShard3 = best3;
            return summary;
        }

        private static string GetDurationBucket(float seconds)
        {
            if (seconds < 60f) return "<60s";
            if (seconds < 180f) return "60-180s";
            if (seconds < 300f) return "180-300s";
            return ">=300s";
        }

        public void PrintSessionSummary()
        {
            float sessionDuration = Time.realtimeSinceStartup - _sessionStart;
            AppLog.Info("[Analytics Summary] Session=" + sessionDuration.ToString("F1") + "s Runs=" + _runDurations.Count + " FirstShard=" + _firstShardCast.ToString("F2") + " FirstReward=" + _firstReward.ToString("F2") + " FirstDamage=" + _firstDamageTaken.ToString("F2"));
        }

        public void FlushToDisk()
        {
            string path = Path.Combine(Application.persistentDataPath, "analytics_events.json");
            EventCollection collection = new EventCollection { events = _events };
            File.WriteAllText(path, JsonUtility.ToJson(collection));
        }

        public void FlushCsvToDisk()
        {
            string path = Path.Combine(Application.persistentDataPath, "analytics_events.csv");
            File.WriteAllText(path, ExportCsv());
        }

        public string Export()
        {
            EventCollection collection = new EventCollection { events = _events };
            return JsonUtility.ToJson(collection, true);
        }

        public string ExportCsv()
        {
            StringBuilder builder = new StringBuilder(4096);
            builder.AppendLine("event_name,timestamp,payload");
            for (int i = 0; i < _events.Count; i++)
            {
                AnalyticsEvent evt = _events[i];
                builder.Append(evt.eventName).Append(',').Append(evt.timestamp.ToString("F3")).Append(',').Append(evt.payload).AppendLine();
            }

            builder.AppendLine("metric,value");
            builder.Append("run_count,").Append(_runDurations.Count).AppendLine();
            for (int i = 0; i < _runDurations.Count; i++)
            {
                builder.Append("run_duration_").Append(i).Append(',').Append(_runDurations[i].ToString("F2")).AppendLine();
            }

            foreach (KeyValuePair<string, int> kv in _shardUsage)
            {
                builder.Append("shard_").Append(kv.Key).Append(',').Append(kv.Value).AppendLine();
            }

            return builder.ToString();
        }
    }
}
