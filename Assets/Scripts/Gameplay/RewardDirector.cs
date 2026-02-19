using EntropySyndicate.Data;
using UnityEngine;

namespace EntropySyndicate.Gameplay
{
    public class RewardDirector
    {
        private readonly RetentionTuningConfig _retentionConfig;
        private readonly EconomyTuningConfig _economyConfig;
        private float _lastRewardTime = -999f;

        public RewardDirector(RetentionTuningConfig retentionConfig, EconomyTuningConfig economyConfig)
        {
            _retentionConfig = retentionConfig ?? ScriptableObject.CreateInstance<RetentionTuningConfig>();
            _economyConfig = economyConfig ?? ScriptableObject.CreateInstance<EconomyTuningConfig>();
        }

        public bool ShouldGrantReward(float runSeconds, float entropyNormalized)
        {
            float dynamicInterval = _retentionConfig.baselineRewardInterval - entropyNormalized * _retentionConfig.entropyRewardIntervalWeight;
            if (dynamicInterval < 3f)
            {
                dynamicInterval = 3f;
            }

            if (runSeconds - _lastRewardTime >= dynamicInterval)
            {
                return true;
            }

            return runSeconds - _lastRewardTime >= _retentionConfig.rewardDroughtCapSeconds;
        }

        public int ComputeScrapReward(float runSeconds, float entropyNormalized)
        {
            float perMinute = _economyConfig.scrapPerMinuteCurve.Evaluate(runSeconds / 60f);
            float spikeMultiplier = 1f + entropyNormalized * 0.7f;
            return (int)(perMinute * (spikeMultiplier / 60f));
        }

        public void MarkRewardGranted(float runSeconds)
        {
            _lastRewardTime = runSeconds;
        }

        public float SecondsSinceLastReward(float runSeconds)
        {
            return runSeconds - _lastRewardTime;
        }
    }
}
