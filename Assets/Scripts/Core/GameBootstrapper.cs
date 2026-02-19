using EntropySyndicate.Data;
using EntropySyndicate.Gameplay;
using EntropySyndicate.Services;
using EntropySyndicate.UI;
using UnityEngine;

namespace EntropySyndicate.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameBalanceConfig balanceConfig;
        [SerializeField] private MetaProgressionConfig metaConfig;
        [SerializeField] private RunController runController;
        [SerializeField] private UIFlowController uiFlowController;

        private ServiceRegistry _services;
        private GameStateMachine _stateMachine;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            _services = new ServiceRegistry();
            _stateMachine = new GameStateMachine();

            SaveService save = new SaveService();
            save.LoadOrCreate();

            EconomyService economy = new EconomyService(save);
            AnalyticsService analytics = new AnalyticsService();
            MonetizationService monetization = new MonetizationService(analytics);
            MissionService missionService = new MissionService(save, metaConfig, analytics);
            ProgressionService progression = new ProgressionService(save, metaConfig, economy, missionService, analytics);

            _services.Register(save);
            _services.Register(economy);
            _services.Register(analytics);
            _services.Register(monetization);
            _services.Register(missionService);
            _services.Register(progression);
            _services.Register(balanceConfig);
            _services.Register(metaConfig);

            runController.Initialize(_services, _stateMachine);
            uiFlowController.Initialize(_services, _stateMachine, runController);

            analytics.LogEvent("session_start");
            _stateMachine.SetState(GameFlowState.Home);
        }

        private void OnApplicationQuit()
        {
            AnalyticsService analytics = _services.Get<AnalyticsService>();
            analytics?.LogEvent("session_end");
            analytics?.FlushToDisk();

            SaveService save = _services.Get<SaveService>();
            save?.Persist();
        }
    }
}
