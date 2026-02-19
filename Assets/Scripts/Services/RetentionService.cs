using System;
using EntropySyndicate.Data;
using EntropySyndicate.Utils;

namespace EntropySyndicate.Services
{
    public class RetentionService
    {
        private readonly SaveService _save;
        private readonly EconomyService _economy;
        private readonly RetentionTuningConfig _config;
        private readonly AnalyticsService _analytics;

        public RetentionService(SaveService save, EconomyService economy, RetentionTuningConfig config, AnalyticsService analytics)
        {
            _save = save;
            _economy = economy;
            _config = config ?? UnityEngine.ScriptableObject.CreateInstance<RetentionTuningConfig>();
            _analytics = analytics;
        }

        public void HandleSessionStartRewards()
        {
            DateTime now = DateTime.UtcNow;
            DateTime lastLogin = _save.Data.lastLoginTicks > 0 ? new DateTime(_save.Data.lastLoginTicks, DateTimeKind.Utc) : now;
            int daysAway = (int)(now.Date - lastLogin.Date).TotalDays;

            if (daysAway >= 1)
            {
                _save.Data.dailyStreak = daysAway == 1 ? _save.Data.dailyStreak + 1 : 1;
                int streakReward = _config.dailyBaseScrap + (_save.Data.dailyStreak - 1) * _config.dailyStreakStep;
                _economy.Add(CurrencyType.Scrap, streakReward);
                _analytics.LogEvent("daily_streak_reward", _save.Data.dailyStreak.ToString());
            }

            if (!_save.Data.firstSessionBonusClaimed)
            {
                _save.Data.firstSessionBonusClaimed = true;
                _economy.Add(CurrencyType.Prisms, _config.firstSessionBonusPrisms);
                _economy.Add(CurrencyType.Scrap, _config.dailyBaseScrap * 2);
                _analytics.LogEvent("first_session_bonus_claimed");
            }

            if (daysAway >= _config.returnAfterDays)
            {
                _economy.Add(CurrencyType.Scrap, _config.returnRewardScrap);
                _analytics.LogEvent("return_player_reward", daysAway.ToString());
            }

            _save.Data.lastLoginTicks = now.Ticks;
        }

        public int GetRunIndexForDda()
        {
            return _save.Data.totalRuns + 1;
        }

        public float GetDifficultyMultiplierForRun(int runIndex)
        {
            if (runIndex <= 1) return _config.run1DifficultyMultiplier;
            if (runIndex == 2) return _config.run2DifficultyMultiplier;
            if (runIndex == 3) return _config.run3DifficultyMultiplier;
            return 1f;
        }

        public float ClampEarlyEntropy(float entropy, float runSeconds)
        {
            if (runSeconds <= _config.firstUnlockDeadline)
            {
                return entropy > _config.earlyEntropyCap ? _config.earlyEntropyCap : entropy;
            }

            return entropy;
        }

        public void RegisterRunEnd()
        {
            _save.Data.totalRuns++;
            _save.Data.sessionRuns++;
        }

        public bool IsFirstSessionSuppressedMonetization()
        {
            return _save.Data.totalRuns <= 0;
        }
    }
}
