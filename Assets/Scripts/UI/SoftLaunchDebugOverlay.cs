using EntropySyndicate.Data;
using EntropySyndicate.Gameplay;
using EntropySyndicate.Services;
using UnityEngine;

namespace EntropySyndicate.UI
{
    public class SoftLaunchDebugOverlay : MonoBehaviour
    {
        [SerializeField] private RunController runController;
        private AnalyticsService _analytics;
        private BuildRuntimeConfig _runtimeConfig;
        private float _fpsSmoothed;

        public void Initialize(AnalyticsService analytics)
        {
            _analytics = analytics;
        }

        public void SetRuntimeConfig(BuildRuntimeConfig runtimeConfig)
        {
            _runtimeConfig = runtimeConfig;
        }

        private void Update()
        {
            float currentFps = Time.unscaledDeltaTime > 0f ? 1f / Time.unscaledDeltaTime : 0f;
            _fpsSmoothed = Mathf.Lerp(_fpsSmoothed <= 0f ? currentFps : _fpsSmoothed, currentFps, 0.08f);
        }

        private void OnGUI()
        {
            if (_analytics == null || !_analytics.IsDebugOverlayEnabled() || runController == null)
            {
                return;
            }

            if (_runtimeConfig != null && !_runtimeConfig.debugMode)
            {
                return;
            }

            long memoryMb = System.GC.GetTotalMemory(false) / (1024 * 1024);

            GUILayout.BeginArea(new Rect(10f, 10f, 360f, 200f), GUI.skin.box);
            GUILayout.Label("Soft Launch Overlay");
            GUILayout.Label("Run Time: " + runController.RunSeconds.ToString("F1"));
            GUILayout.Label("Score: " + runController.Score);
            GUILayout.Label("Health: " + runController.Health.ToString("F1"));
            GUILayout.Label("Entropy: " + runController.EntropyNormalized.ToString("F2"));
            GUILayout.Label("Energy: " + runController.EnergyNormalized.ToString("F2"));
            GUILayout.Label("FPS: " + _fpsSmoothed.ToString("F1"));
            GUILayout.Label("Memory MB: " + memoryMb);
            GUILayout.EndArea();
        }
    }
}
