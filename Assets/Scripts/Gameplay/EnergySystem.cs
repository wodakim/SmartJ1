using EntropySyndicate.Data;
using UnityEngine;

namespace EntropySyndicate.Gameplay
{
    public class EnergySystem
    {
        private readonly GameBalanceConfig _config;
        public float Current { get; private set; }

        public EnergySystem(GameBalanceConfig config)
        {
            _config = config;
            Current = config.baseEnergy;
        }

        public void Tick(float runSeconds, float dt)
        {
            float minute = runSeconds / 60f;
            float regenMultiplier = _config.regenScalingByMinute.Evaluate(minute);
            Current += _config.baseEnergyRegenPerSecond * regenMultiplier * dt;
            if (Current > _config.maxEnergy)
            {
                Current = _config.maxEnergy;
            }
        }

        public bool Spend(float amount)
        {
            if (Current < amount)
            {
                return false;
            }

            Current -= amount;
            return true;
        }

        public void RefillInstant()
        {
            Current = _config.maxEnergy;
        }
    }
}
