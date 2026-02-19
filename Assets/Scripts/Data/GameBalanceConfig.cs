using UnityEngine;

namespace EntropySyndicate.Data
{
    [CreateAssetMenu(fileName = "GameBalanceConfig", menuName = "EntropySyndicate/Balance Config")]
    public class GameBalanceConfig : ScriptableObject
    {
        [Header("Run")]
        public float baseRunDifficulty = 1f;
        public float entropyDifficultyWeight = 0.35f;
        public float timeDifficultyWeight = 0.05f;
        public float shardStackDifficultyWeight = 0.15f;

        [Header("Energy")]
        public float baseEnergy = 100f;
        public float maxEnergy = 100f;
        public float baseEnergyRegenPerSecond = 7f;
        public AnimationCurve regenScalingByMinute = AnimationCurve.Linear(0f, 1f, 6f, 2f);

        [Header("Spawning")]
        public AnimationCurve spawnRateOverTime = new AnimationCurve(new Keyframe(0f, 0.8f), new Keyframe(120f, 2f), new Keyframe(300f, 4.2f));
        public AnimationCurve eliteChanceByEntropy = new AnimationCurve(new Keyframe(0f, 0.02f), new Keyframe(120f, 0.25f), new Keyframe(240f, 0.42f));
        public AnimationCurve dropRateByDifficulty = new AnimationCurve(new Keyframe(1f, 0.25f), new Keyframe(5f, 0.14f), new Keyframe(10f, 0.09f));

        [Header("Entropy")]
        public float entropyPerShard = 5f;
        public float entropyDecayPerSecond = 1f;
        public float maxEntropy = 250f;
        public float scoreMultiplierPerEntropyPoint = 0.005f;

        [Header("Reward")]
        public int baseScrapPerSecond = 3;
        public int baseSigilsAtMilestone = 5;
        public int premiumXpPerRun = 40;
    }
}
