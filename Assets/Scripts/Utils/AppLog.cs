using UnityEngine;

namespace EntropySyndicate.Utils
{
    public static class AppLog
    {
        private static bool _allowLogs = true;

        public static void Configure(bool allowLogs)
        {
            _allowLogs = allowLogs;
        }

        public static void Info(string message)
        {
            if (_allowLogs)
            {
                Debug.Log(message);
            }
        }

        public static void Warn(string message)
        {
            if (_allowLogs)
            {
                Debug.LogWarning(message);
            }
        }
    }
}
