using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntropySyndicate.Data
{
    [CreateAssetMenu(fileName = "MetaProgressionConfig", menuName = "EntropySyndicate/Meta Progression Config")]
    public class MetaProgressionConfig : ScriptableObject
    {
        [Serializable]
        public struct ForgeNodeData
        {
            public string id;
            public string title;
            public string description;
            public CurrencyType costCurrency;
            public int costAmount;
            public int maxLevel;
        }

        [Serializable]
        public struct MissionData
        {
            public string id;
            public MissionCadence cadence;
            public string description;
            public int target;
            public int scrapReward;
            public int passXpReward;
        }

        [Serializable]
        public struct BattlePassTier
        {
            public int tier;
            public int requiredXp;
            public int freeScrap;
            public int premiumPrisms;
        }

        public List<ForgeNodeData> forgeNodes = new List<ForgeNodeData>();
        public List<MissionData> missions = new List<MissionData>();
        public List<BattlePassTier> battlePassTiers = new List<BattlePassTier>();
    }
}
