using EntropySyndicate.Data;

namespace EntropySyndicate.Services
{
    public class MonetizationService
    {
        private readonly AnalyticsService _analytics;
        private int _energyRefillsUsedThisRun;

        public MonetizationService(AnalyticsService analytics)
        {
            _analytics = analytics;
        }

        public void ResetRunCounters()
        {
            _energyRefillsUsedThisRun = 0;
        }

        public bool CanUseEnergyRefillAd()
        {
            return _energyRefillsUsedThisRun < 2;
        }

        public void ShowRewardedAd(AdPlacement placement)
        {
            _analytics.LogEvent("ad_shown", placement.ToString());
            _analytics.LogEvent("ad_watched", placement.ToString());
            if (placement == AdPlacement.InstantEnergyRefill)
            {
                _energyRefillsUsedThisRun++;
            }
        }

        public void SimulateAdWatched(AdPlacement placement)
        {
            ShowRewardedAd(placement);
        }

        public void TriggerPurchase(string productId)
        {
            _analytics.LogEvent("purchase_triggered", productId);
        }
    }
}
