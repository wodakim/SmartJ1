using System.Collections.Generic;
using EntropySyndicate.Core;
using EntropySyndicate.Data;
using EntropySyndicate.Services;
using EntropySyndicate.UI;
using EntropySyndicate.Utils;
using UnityEngine;

namespace EntropySyndicate.Gameplay
{
    public class RunController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private DeterministicObjectPool enemyPool;
        [SerializeField] private List<ShardDefinition> shardDefinitions = new List<ShardDefinition>(16);
        [SerializeField] private JuiceController juiceController;
        [SerializeField] private AudioIntensityController audioController;

        [Header("Run Settings")]
        [SerializeField] private float baseHealth = 100f;

        private ServiceRegistry _services;
        private GameStateMachine _stateMachine;
        private GameBalanceConfig _balance;
        private EconomyTuningConfig _economyTuning;
        private RetentionTuningConfig _retentionTuning;
        private BuildRuntimeConfig _runtimeConfig;
        private RetentionService _retentionService;
        private MonetizationTimingController _monetizationTiming;
        private AnalyticsService _analytics;
        private EntropyEngine _entropy;
        private EnergySystem _energy;
        private SpawnDirector _spawnDirector;
        private ShardExecutionPipeline _pipeline;
        private TimeModifierEngine _timeModifier;
        private RewardDirector _rewardDirector;

        private bool _running;
        private bool _firstRewardGiven;
        private bool _firstUnlockGiven;
        private bool _nearFailureTriggered;
        private bool _earlySynergyTriggered;
        private float _health;
        private float _runTime;
        private int _score;
        private int _runIndex;

        public float EnergyNormalized => _energy == null || _balance == null || _balance.maxEnergy <= 0f ? 0f : _energy.Current / _balance.maxEnergy;
        public float EntropyNormalized => _entropy == null || _balance == null || _balance.maxEntropy <= 0f ? 0f : _entropy.CurrentEntropy / _balance.maxEntropy;
        public int Score => _score;
        public float RunSeconds => _runTime;
        public float Health => _health;
        public bool IsRunning => _running;

        public void Initialize(ServiceRegistry services, GameStateMachine stateMachine)
        {
            _services = services;
            _stateMachine = stateMachine;
            _balance = services.Get<GameBalanceConfig>();
            _economyTuning = services.Get<EconomyTuningConfig>();
            _retentionTuning = services.Get<RetentionTuningConfig>();
            _runtimeConfig = services.Get<BuildRuntimeConfig>();
            _retentionService = services.Get<RetentionService>();
            _monetizationTiming = services.Get<MonetizationTimingController>();
            _analytics = services.Get<AnalyticsService>();

            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }

            if (_balance == null) _balance = ScriptableObject.CreateInstance<GameBalanceConfig>();
            if (_economyTuning == null) _economyTuning = ScriptableObject.CreateInstance<EconomyTuningConfig>();
            if (_retentionTuning == null) _retentionTuning = ScriptableObject.CreateInstance<RetentionTuningConfig>();
            if (_runtimeConfig == null) _runtimeConfig = ScriptableObject.CreateInstance<BuildRuntimeConfig>();

            _entropy = new EntropyEngine(_balance);
            _energy = new EnergySystem(_balance);
            _spawnDirector = new SpawnDirector(_balance, enemyPool);
            _pipeline = new ShardExecutionPipeline(shardDefinitions, _energy, _entropy, _analytics);
            _timeModifier = new TimeModifierEngine();
            _rewardDirector = new RewardDirector(_retentionTuning, _economyTuning);
        }

        public void StartRun()
        {
            _services.Get<MonetizationService>()?.ResetRunCounters();
            _analytics?.BeginRun();
            _analytics?.LogEvent("run_start");
            _running = true;
            _firstRewardGiven = false;
            _firstUnlockGiven = false;
            _nearFailureTriggered = false;
            _earlySynergyTriggered = false;
            _health = baseHealth;
            _runTime = 0f;
            _score = 0;
            _runIndex = _retentionService != null ? _retentionService.GetRunIndexForDda() : 1;
            _stateMachine?.SetState(GameFlowState.Run);
        }

        public void EndRun()
        {
            if (!_running)
            {
                return;
            }

            _running = false;
            float progressionMult = _runtimeConfig != null && _runtimeConfig.testModeEnabled ? _runtimeConfig.progressionMultiplier : 1f;
            int scrapReward = Mathf.RoundToInt(_runTime * _balance.baseScrapPerSecond * _entropy.ScoreMultiplier() * progressionMult);
            int xpReward = Mathf.RoundToInt(_balance.premiumXpPerRun * progressionMult);

            _services.Get<ProgressionService>()?.ApplyRunRewards(scrapReward, xpReward);
            _retentionService?.RegisterRunEnd();
            _analytics?.EndRun(_runTime);
            _analytics?.LogEvent("run_end", _score.ToString());
            _analytics?.LogEvent("entropy_peak", _entropy.PeakEntropy.ToString("F1"));
            _stateMachine?.SetState(GameFlowState.GameOver);
        }

        private void Update()
        {
            if (!_running)
            {
                return;
            }

            float dt = Time.deltaTime;
            _runTime += dt;

            if (_runtimeConfig != null && _runtimeConfig.testModeEnabled && _runTime >= _runtimeConfig.forcedSessionLengthSeconds)
            {
                EndRun();
                return;
            }

            if (_runTime <= _retentionTuning.tutorialPromptAtSecond && _runTime + dt >= _retentionTuning.tutorialPromptAtSecond)
            {
                _analytics?.LogEvent("invisible_tutorial_nudge");
            }

            _entropy.Tick(dt);
            float energyScale = _economyTuning.energyRegenScalingByRunIndex.Evaluate(_runIndex);
            _energy.Tick(_runTime, dt, energyScale);

            float clampedEntropy = _retentionService != null ? _retentionService.ClampEarlyEntropy(_entropy.CurrentEntropy, _runTime) : _entropy.CurrentEntropy;
            if (!Mathf.Approximately(clampedEntropy, _entropy.CurrentEntropy))
            {
                _entropy.ForceSet(clampedEntropy);
            }

            float entropy01 = _entropy.EntropyNormalized();
            float difficulty = (_balance.baseRunDifficulty +
                               _runTime * _balance.timeDifficultyWeight +
                               _entropy.CurrentEntropy * _balance.entropyDifficultyWeight * 0.01f) *
                               (_retentionService != null ? _retentionService.GetDifficultyMultiplierForRun(_runIndex) : 1f);

            _spawnDirector.Tick(_runTime, difficulty);
            _timeModifier.Tick(dt);
            _score += Mathf.RoundToInt(dt * 8f * _entropy.ScoreMultiplier());

            if (juiceController != null) juiceController.SetIntensity(entropy01);
            if (audioController != null) audioController.SetIntensity(entropy01);

            if (_rewardDirector.ShouldGrantReward(_runTime, entropy01))
            {
                float rewardVisMult = _runtimeConfig != null && _runtimeConfig.testModeEnabled ? _runtimeConfig.rewardVisibilityMultiplier : 1f;
                int reward = Mathf.RoundToInt(_rewardDirector.ComputeScrapReward(_runTime, entropy01) * rewardVisMult);
                _services.Get<EconomyService>()?.Add(CurrencyType.Scrap, reward);
                _rewardDirector.MarkRewardGranted(_runTime);
                _monetizationTiming?.RegisterPositiveSpike(_runTime);
                if (!_firstRewardGiven)
                {
                    _firstRewardGiven = true;
                    _analytics?.MarkFirstReward(_runTime);
                }
            }

            if (!_firstUnlockGiven && _runTime >= _retentionTuning.firstUnlockDeadline)
            {
                _firstUnlockGiven = true;
                _services.Get<ProgressionService>()?.ApplyRunRewards(0, _balance.premiumXpPerRun * 2);
                _analytics?.LogEvent("first_unlock_within_60s");
            }

            if (_health <= 0f)
            {
                EndRun();
                return;
            }

            if (!_nearFailureTriggered && _health <= _retentionTuning.nearFailureHealthThreshold)
            {
                _nearFailureTriggered = true;
                _services.Get<EconomyService>()?.Add(CurrencyType.Scrap, (int)_retentionTuning.rescueRewardScrap);
                _analytics?.LogEvent("near_failure_event", _runTime.ToString("F2"));
            }

            if (Input.GetMouseButtonDown(0) && gameplayCamera != null)
            {
                Vector3 world = gameplayCamera.ScreenToWorldPoint(Input.mousePosition);
                world.z = 0f;

                bool cast = _pipeline.TryCast(ShardType.GravityWarp, world);
                if (cast)
                {
                    if (_runTime <= _retentionTuning.firstFeedbackDeadline)
                    {
                        _analytics?.LogEvent("first_5s_feedback_hit");
                    }

                    _analytics?.MarkFirstShardCast(_runTime);

                    if (!_earlySynergyTriggered && _runTime <= 20f)
                    {
                        _earlySynergyTriggered = true;
                        Vector2 synergyPos = new Vector2(world.x + 0.7f, world.y);
                        _pipeline.TryCast(ShardType.MomentumLock, synergyPos);
                        _monetizationTiming?.RegisterPositiveSpike(_runTime);
                        _analytics?.LogEvent("early_synergy_moment", _runTime.ToString("F2"));
                    }

                    if (_entropy.CurrentEntropy >= _retentionTuning.earlyEntropyCap * 0.65f)
                    {
                        _analytics?.MarkFirstEntropySpike(_runTime);
                    }

                    if (juiceController != null) juiceController.PlayShardFeedback(world);
                }
                else if (!_energy.CanAfford(1f))
                {
                    _analytics?.MarkEnergyDepleted();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_running)
            {
                return;
            }

            ChaosEntity entity = other.GetComponent<ChaosEntity>();
            if (entity == null)
            {
                return;
            }

            _health -= 4f;
            _score += entity.ScrapValue;
            _analytics?.MarkFirstDamageTaken(_runTime);
            if (juiceController != null) juiceController.PlayHitFeedback(transform.position, 6f);
        }

        public bool CanOfferRevive()
        {
            return _monetizationTiming != null && _monetizationTiming.ShouldOfferReviveAlmostHadIt(_health, _retentionTuning.nearFailureHealthThreshold);
        }

        public bool CanOfferRewardedAd(bool frustrationState)
        {
            return _monetizationTiming != null && _monetizationTiming.CanOfferRewardedAd(_runTime, frustrationState);
        }

        public void ForceHighEntropyForDebug()
        {
            _entropy.ForceSet(_balance.maxEntropy * 0.9f);
            _monetizationTiming?.RegisterPositiveSpike(_runTime);
        }

        public bool TryReviveViaAd()
        {
            if (_running || !CanOfferRevive())
            {
                return false;
            }

            _services.Get<MonetizationService>()?.ShowRewardedAd(AdPlacement.Revive);
            _health = baseHealth * 0.35f;
            _running = true;
            _stateMachine?.SetState(GameFlowState.Run);
            return true;
        }

        public bool TryRefillEnergyViaAd()
        {
            MonetizationService monetization = _services.Get<MonetizationService>();
            if (monetization == null || !monetization.CanUseEnergyRefillAd() || !CanOfferRewardedAd(false))
            {
                return false;
            }

            monetization.ShowRewardedAd(AdPlacement.InstantEnergyRefill);
            _energy.RefillInstant();
            return true;
        }
    }
}
