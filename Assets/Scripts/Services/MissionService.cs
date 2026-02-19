using EntropySyndicate.Utils;
using EntropySyndicate.Data;

namespace EntropySyndicate.Services
{
    public class MissionService
    {
        private readonly SaveService _save;
        private readonly MetaProgressionConfig _meta;
        private readonly AnalyticsService _analytics;

        public MissionService(SaveService save, MetaProgressionConfig meta, AnalyticsService analytics)
        {
            _save = save;
            _meta = meta;
            _analytics = analytics;
            EnsureMissionKeys();
        }

        private void EnsureMissionKeys()
        {
            for (int i = 0; i < _meta.missions.Count; i++)
            {
                MetaProgressionConfig.MissionData mission = _meta.missions[i];
                if (_save.Data.missionProgress.GetInt(mission.id) == 0 && !_save.Data.missionClaimed.GetBool(mission.id))
                {
                    _save.Data.missionProgress.SetInt(mission.id, 0);
                    _save.Data.missionClaimed.SetBool(mission.id, false);
                }
            }
        }

        public void AddProgress(string missionId, int amount)
        {
            int current = _save.Data.missionProgress.GetInt(missionId);
            _save.Data.missionProgress.SetInt(missionId, current + amount);
        }

        public bool Claim(string missionId, EconomyService economy)
        {
            if (_save.Data.missionClaimed.GetBool(missionId))
            {
                return false;
            }

            for (int i = 0; i < _meta.missions.Count; i++)
            {
                MetaProgressionConfig.MissionData mission = _meta.missions[i];
                if (mission.id != missionId)
                {
                    continue;
                }

                if (_save.Data.missionProgress.GetInt(missionId) < mission.target)
                {
                    return false;
                }

                _save.Data.missionClaimed.SetBool(missionId, true);
                economy.Add(CurrencyType.Scrap, mission.scrapReward);
                _save.Data.battlePassXp += mission.passXpReward;
                _analytics.LogEvent("mission_claimed", missionId);
                return true;
            }

            return false;
        }
    }
}
