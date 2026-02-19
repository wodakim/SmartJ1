using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly List<AnalyticsEvent> _events = new List<AnalyticsEvent>(512);

        public void LogEvent(string eventName, string payload = "")
        {
            _events.Add(new AnalyticsEvent
            {
                eventName = eventName,
                timestamp = Time.realtimeSinceStartup,
                payload = payload
            });
        }

        public void FlushToDisk()
        {
            string path = Path.Combine(Application.persistentDataPath, "analytics_events.json");
            EventCollection collection = new EventCollection { events = _events };
            File.WriteAllText(path, JsonUtility.ToJson(collection));
        }

        public string Export()
        {
            EventCollection collection = new EventCollection { events = _events };
            return JsonUtility.ToJson(collection, true);
        }
    }
}
