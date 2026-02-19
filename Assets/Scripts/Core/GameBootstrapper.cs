using EntropySyndicate.Data;
using EntropySyndicate.Gameplay;
using EntropySyndicate.Services;
using EntropySyndicate.UI;
using EntropySyndicate.Utils;
using UnityEngine;

namespace EntropySyndicate.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameBalanceConfig balanceConfig;
        [SerializeField] private MetaProgressionConfig metaConfig;
        [SerializeField] private EconomyTuningConfig economyTuningConfig;
        [SerializeField] private RetentionTuningConfig retentionTuningConfig;
        [SerializeField] private BuildRuntimeConfig buildRuntimeConfig;
        [SerializeField] private RunController runController;
        [SerializeField] private UIFlowController uiFlowController;
        [SerializeField] private SoftLaunchDebugPanel softLaunchDebugPanel;
        [SerializeField] private SoftLaunchDebugOverlay softLaunchDebugOverlay;

        private ServiceRegistry _services;
        private GameStateMachine _stateMachine;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            EnsureConfigFallbacks();
            AppLog.Configure(!buildRuntimeConfig.productionMode && buildRuntimeConfig.debugMode);

            _services = new ServiceRegistry();
            _stateMachine = new GameStateMachine();

            SaveService save = new SaveService();
            save.LoadOrCreate();

            EconomyService economy = new EconomyService(save);
            AnalyticsService analytics = new AnalyticsService();
            analytics.BeginSession();
            analytics.Configure(buildRuntimeConfig);
            MonetizationService monetization = new MonetizationService(analytics);
            MissionService missionService = new MissionService(save, metaConfig, analytics);
            ProgressionService progression = new ProgressionService(save, metaConfig, economy, missionService, analytics, economyTuningConfig);
            RetentionService retention = new RetentionService(save, economy, retentionTuningConfig, analytics);
            MonetizationTimingController monetizationTiming = new MonetizationTimingController(retentionTuningConfig, retention);

            retention.HandleSessionStartRewards();

            _services.Register(save);
            _services.Register(economy);
            _services.Register(analytics);
            _services.Register(monetization);
            _services.Register(missionService);
            _services.Register(progression);
            _services.Register(retention);
            _services.Register(monetizationTiming);
            _services.Register(balanceConfig);
            _services.Register(metaConfig);
            _services.Register(economyTuningConfig);
            _services.Register(retentionTuningConfig);
            _services.Register(buildRuntimeConfig);

            if (runController != null)
            {
                runController.Initialize(_services, _stateMachine);
            }

            if (uiFlowController != null && runController != null)
            {
                uiFlowController.Initialize(_services, _stateMachine, runController);
            }

            if (softLaunchDebugPanel != null)
            {
                softLaunchDebugPanel.Initialize(save, economy, analytics, monetization);
                softLaunchDebugPanel.SetRuntimeConfig(buildRuntimeConfig);
            }

            if (softLaunchDebugOverlay != null)
            {
                softLaunchDebugOverlay.Initialize(analytics);
                softLaunchDebugOverlay.SetRuntimeConfig(buildRuntimeConfig);
            }

            if (buildRuntimeConfig.testModeEnabled && buildRuntimeConfig.analyticsOverlayOnByDefault)
            {
                analytics.SetDebugOverlayEnabled(true);
            }

            analytics.LogEvent("session_start");
            _stateMachine.SetState(GameFlowState.Home);
        }

        private void EnsureConfigFallbacks()
        {
            if (balanceConfig == null) balanceConfig = ScriptableObject.CreateInstance<GameBalanceConfig>();
            if (metaConfig == null) metaConfig = ScriptableObject.CreateInstance<MetaProgressionConfig>();
            if (economyTuningConfig == null) economyTuningConfig = ScriptableObject.CreateInstance<EconomyTuningConfig>();
            if (retentionTuningConfig == null) retentionTuningConfig = ScriptableObject.CreateInstance<RetentionTuningConfig>();
            if (buildRuntimeConfig == null) buildRuntimeConfig = ScriptableObject.CreateInstance<BuildRuntimeConfig>();
        }

        private void OnApplicationQuit()
        {
            AnalyticsService analytics = _services.Get<AnalyticsService>();
            analytics?.TrackAbandonmentTime(runController != null ? runController.RunSeconds : 0f);
            analytics?.PrintSessionSummary();
            analytics?.LogEvent("session_end");
            analytics?.FlushToDisk();
            analytics?.FlushCsvToDisk();

            SaveService save = _services.Get<SaveService>();
            save?.Persist();
        }
    }
}
