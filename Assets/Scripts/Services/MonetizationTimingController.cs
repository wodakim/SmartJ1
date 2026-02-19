using EntropySyndicate.Data;

namespace EntropySyndicate.Services
{
    public class MonetizationTimingController
    {
        private readonly RetentionTuningConfig _retentionConfig;
        private readonly RetentionService _retentionService;
        private bool _recentPositiveSpike;
        private float _spikeTimestamp = -999f;

        public MonetizationTimingController(RetentionTuningConfig retentionConfig, RetentionService retentionService)
        {
            _retentionConfig = retentionConfig ?? UnityEngine.ScriptableObject.CreateInstance<RetentionTuningConfig>();
            _retentionService = retentionService;
        }

        public void RegisterPositiveSpike(float runSeconds)
        {
            _recentPositiveSpike = true;
            _spikeTimestamp = runSeconds;
        }

        public bool CanOfferRewardedAd(float runSeconds, bool frustrationState)
        {
            if (_retentionService.IsFirstSessionSuppressedMonetization())
            {
                return false;
            }

            if (frustrationState)
            {
                return false;
            }

            if (!_recentPositiveSpike)
            {
                return false;
            }

            if (runSeconds - _spikeTimestamp > _retentionConfig.nearFailureWindowSeconds)
            {
                _recentPositiveSpike = false;
                return false;
            }

            return true;
        }

        public bool ShouldOfferReviveAlmostHadIt(float health, float threshold)
        {
            return health > 0f && health <= threshold;
        }
    }
}
