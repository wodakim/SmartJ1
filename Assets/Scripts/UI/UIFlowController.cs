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
        [SerializeField] private TextMeshProUGUI scoreLabel;

        private ServiceRegistry _services;
        private GameStateMachine _stateMachine;
        private RunController _run;

        public void Initialize(ServiceRegistry services, GameStateMachine stateMachine, RunController run)
        {
            _services = services;
            _stateMachine = stateMachine;
            _run = run;
            _stateMachine.OnStateChanged += HandleState;
            HandleState(GameFlowState.Home);
        }

        private void HandleState(GameFlowState state)
        {
            homeScreen.SetActive(state == GameFlowState.Home);
            shardsScreen.SetActive(state == GameFlowState.Shards);
            forgeScreen.SetActive(state == GameFlowState.Forge);
            missionsScreen.SetActive(state == GameFlowState.Missions);
            shopScreen.SetActive(state == GameFlowState.Shop);
            settingsScreen.SetActive(state == GameFlowState.Settings);
            runHud.SetActive(state == GameFlowState.Run);
            gameOverScreen.SetActive(state == GameFlowState.GameOver);
            debugScreen.SetActive(state == GameFlowState.Debug);
        }

        private void Update()
        {
            if (runHud.activeSelf)
            {
                energyMeter.value = _run.EnergyNormalized;
                entropyMeter.value = _run.EntropyNormalized;
                scoreLabel.text = _run.Score.ToString();
            }
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
            _services.Get<MonetizationService>().ShowRewardedAd(AdPlacement.EndRunDoubleReward);
            _services.Get<EconomyService>().Add(CurrencyType.Scrap, Mathf.RoundToInt(_run.RunSeconds * 3f));
        }

        public void OnTapRevive() => _run.TryReviveViaAd();
        public void OnTapEnergyRefill() => _run.TryRefillEnergyViaAd();

        public void OnTapExportAnalytics()
        {
            string export = _services.Get<AnalyticsService>().Export();
            Debug.Log(export);
        }
    }
}
