using EntropySyndicate.Data;
using EntropySyndicate.Gameplay;
using EntropySyndicate.Services;
using UnityEngine;

namespace EntropySyndicate.UI
{
    public class SoftLaunchDebugPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;

        private SaveService _save;
        private EconomyService _economy;
        private AnalyticsService _analytics;
        private MonetizationService _monetization;
        private BuildRuntimeConfig _runtimeConfig;

        public void Initialize(SaveService save, EconomyService economy, AnalyticsService analytics, MonetizationService monetization)
        {
            _save = save;
            _economy = economy;
            _analytics = analytics;
            _monetization = monetization;
        }

        public void SetRuntimeConfig(BuildRuntimeConfig runtimeConfig)
        {
            _runtimeConfig = runtimeConfig;
        }

        public void ToggleHiddenPanel()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(!panelRoot.activeSelf);
            }
        }

        public void ToggleTestMode()
        {
            if (_runtimeConfig == null)
            {
                return;
            }

            _runtimeConfig.testModeEnabled = !_runtimeConfig.testModeEnabled;
            _analytics.SetDebugOverlayEnabled(_runtimeConfig.testModeEnabled && _runtimeConfig.analyticsOverlayOnByDefault);
            _analytics.LogEvent("debug_toggle_test_mode", _runtimeConfig.testModeEnabled ? "1" : "0");
        }

        public void ForceD1Simulation()
        {
            _save.Data.d1SimulationMode = true;
            _save.Data.totalRuns = 1;
            _save.Data.firstSessionBonusClaimed = true;
            _analytics.LogEvent("debug_force_d1_simulation");
        }

        public void ForceHighEntropy(RunController runController)
        {
            runController.ForceHighEntropyForDebug();
            _analytics.LogEvent("debug_force_high_entropy", runController.RunSeconds.ToString("F2"));
        }

        public void SimulateAdWatched()
        {
            _monetization.SimulateAdWatched(AdPlacement.EndRunDoubleReward);
        }

        public void SimulatePurchase()
        {
            _monetization.TriggerPurchase("debug_pack");
            _economy.Add(CurrencyType.Prisms, 100);
        }

        public void FastForwardSevenDays()
        {
            _save.Data.lastLoginTicks -= System.TimeSpan.FromDays(7).Ticks;
            _analytics.LogEvent("debug_fast_forward_7_days");
        }
    }
}
