using UnityEngine;

namespace EntropySyndicate.Data
{
    [CreateAssetMenu(fileName = "EconomyTuningConfig", menuName = "EntropySyndicate/Economy Tuning Config")]
    public class EconomyTuningConfig : ScriptableObject
    {
        [Header("Scrap")]
        public AnimationCurve scrapPerMinuteCurve = new AnimationCurve(
            new Keyframe(0f, 60f),
            new Keyframe(5f, 140f),
            new Keyframe(12f, 280f));

        [Header("Upgrade Formula")]
        public int forgeBaseCost = 50;
        public float forgeCostPower = 1.24f;
        public int forgeTierStep = 15;

        [Header("Prestige")]
        public int prestigeBaseThreshold = 6;
        public float prestigeThresholdScaling = 1.35f;

        [Header("Energy")]
        public AnimationCurve energyRegenScalingByRunIndex = new AnimationCurve(
            new Keyframe(1f, 1.4f),
            new Keyframe(3f, 1.15f),
            new Keyframe(6f, 1f));
    }
}
