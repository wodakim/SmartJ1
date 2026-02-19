using System;
using UnityEngine;

namespace EntropySyndicate.Meta
{
    [Serializable]
    public struct SeasonalModifier
    {
        public string id;
        public string description;
        public float spawnRateMultiplier;
        public float entropyGainMultiplier;
    }

    public class SeasonalModifierService : MonoBehaviour
    {
        [SerializeField] private SeasonalModifier activeModifier;

        public SeasonalModifier Active => activeModifier;

        public float ApplySpawnRate(float baseRate)
        {
            return baseRate * Mathf.Max(0.1f, activeModifier.spawnRateMultiplier);
        }

        public float ApplyEntropyGain(float baseGain)
        {
            return baseGain * Mathf.Max(0.1f, activeModifier.entropyGainMultiplier);
        }
    }
}
