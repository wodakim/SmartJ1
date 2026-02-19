using EntropySyndicate.Core;
using EntropySyndicate.Data;
using EntropySyndicate.Gameplay;
using EntropySyndicate.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EntropySyndicate.UI
{
    public class UIFlowController : MonoBehaviour
    {
        [SerializeField] private GameObject homeScreen;
        [SerializeField] private GameObject shardsScreen;
        [SerializeField] private GameObject forgeScreen;
        [SerializeField] private GameObject missionsScreen;
        [SerializeField] private GameObject shopScreen;
        [SerializeField] private GameObject settingsScreen;
        [SerializeField] private GameObject runHud;
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private GameObject debugScreen;

        [Header("HUD")]
        [SerializeField] private Slider energyMeter;
        [SerializeField] private Slider entropyMeter;
        [SerializeField] private TextMeshProUGUI energyLabel;
        [SerializeField] private TextMeshProUGUI entropyLabel;
        [SerializeField] private TextMeshProUGUI scoreLabel;

        [Header("Game Over")]
        [SerializeField] private TextMeshProUGUI gameOverSummaryLabel;

        private ServiceRegistry _services;
        private GameStateMachine _stateMachine;
        private RunController _run;
        private BuildRuntimeConfig _runtimeConfig;
        private int _lastScore = -1;
        private float _lastEnergy = -1f;
        private float _lastEntropy = -1f;

        public void Initialize(ServiceRegistry services, GameStateMachine stateMachine, RunController run)
        {
            _services = services;
            _stateMachine = stateMachine;
            _run = run;
            _runtimeConfig = services.Get<BuildRuntimeConfig>();
            _stateMachine.OnStateChanged += HandleState;
            HandleState(GameFlowState.Home);

            if (debugScreen != null && (_runtimeConfig == null || !_runtimeConfig.debugMode))
            {
                debugScreen.SetActive(false);
            }
        }

        private void HandleState(GameFlowState state)
        {
            SetScreenActive(homeScreen, state == GameFlowState.Home);
            SetScreenActive(shardsScreen, state == GameFlowState.Shards);
            SetScreenActive(forgeScreen, state == GameFlowState.Forge);
            SetScreenActive(missionsScreen, state == GameFlowState.Missions);
            SetScreenActive(shopScreen, state == GameFlowState.Shop);
            SetScreenActive(settingsScreen, state == GameFlowState.Settings);
            SetScreenActive(runHud, state == GameFlowState.Run);
            SetScreenActive(gameOverScreen, state == GameFlowState.GameOver);

            bool showDebug = state == GameFlowState.Debug && _runtimeConfig != null && _runtimeConfig.debugMode;
            SetScreenActive(debugScreen, showDebug);

            if (state == GameFlowState.GameOver)
            {
                UpdateGameOverSummary();
            }
        }

        private static void SetScreenActive(GameObject screen, bool active)
        {
            if (screen != null)
            {
                screen.SetActive(active);
            }
        }

        private void Update()
        {
            if (runHud == null || !runHud.activeSelf || _run == null)
            {
                return;
            }

            float energy = _run.EnergyNormalized;
            if (energyMeter != null && !Mathf.Approximately(energy, _lastEnergy))
            {
                _lastEnergy = energy;
                energyMeter.value = energy;
                if (energyLabel != null)
                {
                    energyLabel.SetText("Energy {0:0}%", energy * 100f);
                }
            }

            float entropy = _run.EntropyNormalized;
            if (entropyMeter != null && !Mathf.Approximately(entropy, _lastEntropy))
            {
                _lastEntropy = entropy;
                entropyMeter.value = entropy;
                if (entropyLabel != null)
                {
                    entropyLabel.SetText("Entropy {0:0}%", entropy * 100f);
                }
            }

            int score = _run.Score;
            if (scoreLabel != null && score != _lastScore)
            {
                _lastScore = score;
                scoreLabel.SetText("{0}", score);
            }
        }

        private void UpdateGameOverSummary()
        {
            if (gameOverSummaryLabel == null || _run == null)
            {
                return;
            }

            gameOverSummaryLabel.SetText("Run {0:0}s\nScore {1}\nEntropy {2:0}%", _run.RunSeconds, _run.Score, _run.EntropyNormalized * 100f);
        }

        public void OnTapStartRun() => _run.StartRun();
        public void OnTapHome() => _stateMachine.SetState(GameFlowState.Home);
        public void OnTapShards() => _stateMachine.SetState(GameFlowState.Shards);
        public void OnTapForge() => _stateMachine.SetState(GameFlowState.Forge);
        public void OnTapMissions() => _stateMachine.SetState(GameFlowState.Missions);
        public void OnTapShop() => _stateMachine.SetState(GameFlowState.Shop);
        public void OnTapSettings() => _stateMachine.SetState(GameFlowState.Settings);

        public void OnTapDoubleRewardAd()
        {
            if (!_run.CanOfferRewardedAd(false))
            {
                return;
            }

            _services.Get<MonetizationService>()?.ShowRewardedAd(AdPlacement.EndRunDoubleReward);
            _services.Get<EconomyService>()?.Add(CurrencyType.Scrap, Mathf.RoundToInt(_run.RunSeconds * 3f));
        }

        public void OnTapRevive() => _run.TryReviveViaAd();
        public void OnTapEnergyRefill() => _run.TryRefillEnergyViaAd();

        public void OnTapExportAnalytics()
        {
            string export = _services.Get<AnalyticsService>()?.Export();
            if (!string.IsNullOrEmpty(export))
            {
                Debug.Log(export);
            }
        }

        public void OnTapExportAnalyticsCsv()
        {
            AnalyticsService analytics = _services.Get<AnalyticsService>();
            analytics?.FlushCsvToDisk();
            string csv = analytics?.ExportCsv();
            if (!string.IsNullOrEmpty(csv))
            {
                Debug.Log(csv);
            }
        }

        public void OnTapToggleDebugOverlay()
        {
            AnalyticsService analytics = _services.Get<AnalyticsService>();
            if (analytics != null)
            {
                analytics.SetDebugOverlayEnabled(!analytics.IsDebugOverlayEnabled());
            }
        }
    }
}
