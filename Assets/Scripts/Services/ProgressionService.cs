using EntropySyndicate.Utils;
using EntropySyndicate.Data;

namespace EntropySyndicate.Services
{
    public class ProgressionService
    {
        private readonly SaveService _save;
        private readonly MetaProgressionConfig _meta;
        private readonly EconomyService _economy;
        private readonly MissionService _mission;
        private readonly AnalyticsService _analytics;
        private readonly EconomyTuningConfig _economyTuning;

        public ProgressionService(SaveService save, MetaProgressionConfig meta, EconomyService economy, MissionService mission, AnalyticsService analytics, EconomyTuningConfig economyTuning)
        {
            _save = save;
            _meta = meta;
            _economy = economy;
            _mission = mission;
            _analytics = analytics;
            _economyTuning = economyTuning ?? UnityEngine.ScriptableObject.CreateInstance<EconomyTuningConfig>();
            EnsureForgeNodes();
        }

        private void EnsureForgeNodes()
        {
            for (int i = 0; i < _meta.forgeNodes.Count; i++)
            {
                string id = _meta.forgeNodes[i].id;
                if (_save.Data.forgeLevels.GetInt(id) == 0)
                {
                    _save.Data.forgeLevels.SetInt(id, 0);
                }
            }
        }

        public bool UpgradeForgeNode(string nodeId)
        {
            for (int i = 0; i < _meta.forgeNodes.Count; i++)
            {
                MetaProgressionConfig.ForgeNodeData node = _meta.forgeNodes[i];
                if (node.id != nodeId)
                {
                    continue;
                }

                int current = _save.Data.forgeLevels.GetInt(node.id);
                if (current >= node.maxLevel)
                {
                    return false;
                }

                int dynamicCost = _economyTuning.forgeBaseCost + (int)(current * current * _economyTuning.forgeCostPower) + _economyTuning.forgeTierStep * i;
                if (!_economy.Spend(node.costCurrency, dynamicCost))
                {
                    return false;
                }

                _save.Data.forgeLevels.SetInt(node.id, current + 1);
                return true;
            }

            return false;
        }

        public void ApplyRunRewards(int scrap, int passXp)
        {
            _economy.Add(CurrencyType.Scrap, scrap);
            _save.Data.battlePassXp += passXp;
            _mission.AddProgress("daily_run_1", 1);
            _mission.AddProgress("weekly_runs_1", 1);

            if (_save.Data.battlePassXp >= _save.Data.accountLevel * 120)
            {
                _save.Data.accountLevel++;
            }
        }

        public void PrestigeRecalibration()
        {
            int threshold = _economyTuning.prestigeBaseThreshold + (int)(_save.Data.prestigeCount * _economyTuning.prestigeThresholdScaling);
            if (_save.Data.accountLevel < threshold)
            {
                return;
            }

            int sigilsGained = _save.Data.accountLevel / 2;
            _save.Data.prestigeCount++;
            _save.Data.accountLevel = 1;
            _save.Data.battlePassXp = 0;
            _economy.Add(CurrencyType.Sigils, sigilsGained);
            _analytics.LogEvent("prestige_used", sigilsGained.ToString());
        }
    }
}
