using UnityEngine;

namespace EntropySyndicate.Data
{
    [CreateAssetMenu(fileName = "ShardDefinition", menuName = "EntropySyndicate/Shard Definition")]
    public class ShardDefinition : ScriptableObject
    {
        public ShardType shardType;
        public string displayName;
        public string description;
        public Sprite icon;
        public float energyCost = 20f;
        public float duration = 3f;
        public float radius = 2.5f;
        public float intensity = 1f;
        public int unlockAccountLevel = 1;
    }
}
