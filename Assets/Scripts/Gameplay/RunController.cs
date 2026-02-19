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
        [SerializeField] private DeterministicObjectPool enemyPool;
        [SerializeField] private List<ShardDefinition> shardDefinitions = new List<ShardDefinition>(16);
        [SerializeField] private JuiceController juiceController;
        [SerializeField] private AudioIntensityController audioController;

        [Header("Run Settings")]
        [SerializeField] private float baseHealth = 100f;

        private ServiceRegistry _services;
        private GameStateMachine _stateMachine;
        private GameBalanceConfig _balance;
        private EntropyEngine _entropy;
        private EnergySystem _energy;
        private SpawnDirector _spawnDirector;
        private ShardExecutionPipeline _pipeline;
        private TimeModifierEngine _timeModifier;

        private bool _running;
        private float _health;
        private float _runTime;
        private int _score;

        public float EnergyNormalized => _energy == null ? 0f : _energy.Current / _balance.maxEnergy;
        public float EntropyNormalized => _entropy == null ? 0f : _entropy.CurrentEntropy / _balance.maxEntropy;
        public int Score => _score;
        public float RunSeconds => _runTime;

        public void Initialize(ServiceRegistry services, GameStateMachine stateMachine)
        {
            _services = services;
            _stateMachine = stateMachine;
            _balance = services.Get<GameBalanceConfig>();

            _entropy = new EntropyEngine(_balance);
            _energy = new EnergySystem(_balance);
            _spawnDirector = new SpawnDirector(_balance, enemyPool);
            _pipeline = new ShardExecutionPipeline(shardDefinitions, _energy, _entropy, services.Get<AnalyticsService>());
            _timeModifier = new TimeModifierEngine();
        }

        public void StartRun()
        {
            _services.Get<MonetizationService>().ResetRunCounters();
            _services.Get<AnalyticsService>().LogEvent("run_start");
            _running = true;
            _health = baseHealth;
            _runTime = 0f;
            _score = 0;
            _stateMachine.SetState(GameFlowState.Run);
        }

        public void EndRun()
        {
            if (!_running)
            {
                return;
            }

            _running = false;
            int scrapReward = Mathf.RoundToInt(_runTime * _balance.baseScrapPerSecond * _entropy.ScoreMultiplier());
            _services.Get<ProgressionService>().ApplyRunRewards(scrapReward, _balance.premiumXpPerRun);
            _services.Get<AnalyticsService>().LogEvent("run_end", _score.ToString());
            _services.Get<AnalyticsService>().LogEvent("entropy_peak", _entropy.PeakEntropy.ToString("F1"));
            _stateMachine.SetState(GameFlowState.GameOver);
        }

        private void Update()
        {
            if (!_running)
            {
                return;
            }

            float dt = Time.deltaTime;
            _runTime += dt;

            _entropy.Tick(dt);
            _energy.Tick(_runTime, dt);

            float difficulty = _balance.baseRunDifficulty +
                               _runTime * _balance.timeDifficultyWeight +
                               _entropy.CurrentEntropy * _balance.entropyDifficultyWeight * 0.01f;

            _spawnDirector.Tick(_runTime, difficulty);
            _timeModifier.Tick(dt);
            _score += Mathf.RoundToInt(dt * 8f * _entropy.ScoreMultiplier());

            juiceController.SetIntensity(_entropy.EntropyNormalized());
            audioController.SetIntensity(_entropy.EntropyNormalized());

            if (_health <= 0f)
            {
                EndRun();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                bool cast = _pipeline.TryCast(ShardType.GravityWarp, world);
                if (cast)
                {
                    juiceController.PlayShardFeedback(world);
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
            juiceController.PlayHitFeedback(transform.position, 6f);
        }

        public bool TryReviveViaAd()
        {
            if (_running)
            {
                return false;
            }

            _services.Get<MonetizationService>().ShowRewardedAd(AdPlacement.Revive);
            _health = baseHealth * 0.35f;
            _running = true;
            _stateMachine.SetState(GameFlowState.Run);
            return true;
        }

        public bool TryRefillEnergyViaAd()
        {
            MonetizationService monetization = _services.Get<MonetizationService>();
            if (!monetization.CanUseEnergyRefillAd())
            {
                return false;
            }

            monetization.ShowRewardedAd(AdPlacement.InstantEnergyRefill);
            _energy.RefillInstant();
            return true;
        }
    }
}
