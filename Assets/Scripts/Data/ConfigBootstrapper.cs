using UnityEngine;

namespace EntropySyndicate.Data
{
    public class ConfigBootstrapper : MonoBehaviour
    {
        [SerializeField] private MetaProgressionConfig meta;

        [ContextMenu("Populate Default Meta Content")]
        public void Populate()
        {
            meta.forgeNodes.Clear();
            for (int i = 0; i < 20; i++)
            {
                MetaProgressionConfig.ForgeNodeData node = new MetaProgressionConfig.ForgeNodeData
                {
                    id = "forge_" + i,
                    title = "Protocol " + (i + 1),
                    description = "Permanent systems refinement " + (i + 1),
                    costCurrency = i % 5 == 0 ? CurrencyType.Sigils : CurrencyType.Scrap,
                    costAmount = 50 + i * 25,
                    maxLevel = 5
                };
                meta.forgeNodes.Add(node);
            }

            meta.missions.Clear();
            for (int i = 0; i < 10; i++)
            {
                MetaProgressionConfig.MissionData daily = new MetaProgressionConfig.MissionData
                {
                    id = i == 0 ? "daily_run_1" : "daily_" + i,
                    cadence = MissionCadence.Daily,
                    description = "Daily objective " + (i + 1),
                    target = 1 + i,
                    scrapReward = 20 + i * 8,
                    passXpReward = 30 + i * 4
                };
                meta.missions.Add(daily);
            }

            for (int i = 0; i < 6; i++)
            {
                MetaProgressionConfig.MissionData weekly = new MetaProgressionConfig.MissionData
                {
                    id = i == 0 ? "weekly_runs_1" : "weekly_" + i,
                    cadence = MissionCadence.Weekly,
                    description = "Weekly protocol " + (i + 1),
                    target = 4 + i * 2,
                    scrapReward = 120 + i * 20,
                    passXpReward = 120 + i * 20
                };
                meta.missions.Add(weekly);
            }

            meta.battlePassTiers.Clear();
            for (int i = 1; i <= 50; i++)
            {
                MetaProgressionConfig.BattlePassTier tier = new MetaProgressionConfig.BattlePassTier
                {
                    tier = i,
                    requiredXp = i * 100,
                    freeScrap = 60 + i * 5,
                    premiumPrisms = i % 5 == 0 ? 20 : 0
                };
                meta.battlePassTiers.Add(tier);
            }
        }
    }
}
