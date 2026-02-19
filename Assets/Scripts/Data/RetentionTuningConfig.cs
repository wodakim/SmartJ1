using UnityEngine;

namespace EntropySyndicate.Data
{
    [CreateAssetMenu(fileName = "RetentionTuningConfig", menuName = "EntropySyndicate/Retention Tuning Config")]
    public class RetentionTuningConfig : ScriptableObject
    {
        [Header("First 60 Seconds")]
        public float tutorialPromptAtSecond = 2f;
        public float firstFeedbackDeadline = 5f;
        public float firstRewardDeadline = 15f;
        public float firstUnlockDeadline = 60f;
        public float earlyEntropyCap = 40f;

        [Header("First 3 Runs DDA")]
        public float run1DifficultyMultiplier = 0.72f;
        public float run2DifficultyMultiplier = 0.84f;
        public float run3DifficultyMultiplier = 0.93f;

        [Header("Near Failure")]
        public float nearFailureHealthThreshold = 20f;
        public float nearFailureWindowSeconds = 8f;
        public float rescueRewardScrap = 20f;

        [Header("Daily & Return")]
        public int dailyBaseScrap = 30;
        public int dailyStreakStep = 15;
        public int firstSessionBonusPrisms = 40;
        public int returnAfterDays = 2;
        public int returnRewardScrap = 120;

        [Header("Reward Pacing")]
        public float rewardDroughtCapSeconds = 20f;
        public float baselineRewardInterval = 8f;
        public float entropyRewardIntervalWeight = 4f;
    }
}
