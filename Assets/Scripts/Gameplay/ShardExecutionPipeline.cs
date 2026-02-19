using System.Collections.Generic;
using EntropySyndicate.Data;
using EntropySyndicate.Services;
using UnityEngine;

namespace EntropySyndicate.Gameplay
{
    public class ShardExecutionPipeline
    {
        private readonly Dictionary<ShardType, ShardDefinition> _definitions;
        private readonly EnergySystem _energy;
        private readonly EntropyEngine _entropy;
        private readonly AnalyticsService _analytics;
        private int _stackCount;

        public ShardExecutionPipeline(List<ShardDefinition> definitions, EnergySystem energy, EntropyEngine entropy, AnalyticsService analytics)
        {
            _definitions = new Dictionary<ShardType, ShardDefinition>(definitions.Count);
            for (int i = 0; i < definitions.Count; i++)
            {
                _definitions[definitions[i].shardType] = definitions[i];
            }

            _energy = energy;
            _entropy = entropy;
            _analytics = analytics;
        }

        public bool TryCast(ShardType type, Vector2 position)
        {
            if (!_definitions.TryGetValue(type, out ShardDefinition definition))
            {
                return false;
            }

            if (!_energy.Spend(definition.energyCost))
            {
                return false;
            }

            ApplyEffect(type, definition, position);
            _stackCount++;
            _entropy.AddFromShard(_stackCount * 0.75f);
            _analytics.LogEvent("shard_used", type.ToString());
            return true;
        }

        private static void ApplyEffect(ShardType type, ShardDefinition definition, Vector2 position)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, definition.radius);
            float multiplier = 1f;

            switch (type)
            {
                case ShardType.GravityWarp:
                    multiplier = 1.45f;
                    break;
                case ShardType.MomentumLock:
                    multiplier = 0.2f;
                    break;
                case ShardType.TimeShear:
                    multiplier = 0.5f;
                    break;
                case ShardType.ChaosAmplifier:
                    multiplier = 1.8f;
                    break;
            }

            for (int i = 0; i < hits.Length; i++)
            {
                ChaosEntity entity = hits[i].GetComponent<ChaosEntity>();
                if (entity != null)
                {
                    entity.SetSpeedMultiplier(multiplier * definition.intensity);
                }
            }
        }
    }
}
