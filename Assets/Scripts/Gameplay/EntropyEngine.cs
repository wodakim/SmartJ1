using EntropySyndicate.Data;
using UnityEngine;

namespace EntropySyndicate.Gameplay
{
    public class EntropyEngine
    {
        private readonly GameBalanceConfig _config;

        public float CurrentEntropy { get; private set; }
        public float PeakEntropy { get; private set; }

        public EntropyEngine(GameBalanceConfig config)
        {
            _config = config;
        }

        public void AddFromShard(float stackBonus)
        {
            CurrentEntropy += _config.entropyPerShard + stackBonus;
            if (CurrentEntropy > _config.maxEntropy)
            {
                CurrentEntropy = _config.maxEntropy;
            }

            if (CurrentEntropy > PeakEntropy)
            {
                PeakEntropy = CurrentEntropy;
            }
        }

        public void Tick(float dt)
        {
            CurrentEntropy = Mathf.Max(0f, CurrentEntropy - _config.entropyDecayPerSecond * dt);
        }

        public float EntropyNormalized()
        {
            return _config.maxEntropy <= 0f ? 0f : CurrentEntropy / _config.maxEntropy;
        }

        public float ScoreMultiplier()
        {
            return 1f + CurrentEntropy * _config.scoreMultiplierPerEntropyPoint;
        }
    }
}
