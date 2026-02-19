using EntropySyndicate.Utils;
using System;
using EntropySyndicate.Data;

namespace EntropySyndicate.Services
{
    public class EconomyService
    {
        private readonly SaveService _save;
        public event Action<CurrencyType, int> OnCurrencyChanged;

        public EconomyService(SaveService save)
        {
            _save = save;
        }

        public int Get(CurrencyType type)
        {
            return _save.Data.currencies.GetCurrency(type);
        }

        public void Add(CurrencyType type, int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            int current = Get(type);
            _save.Data.currencies.SetCurrency(type, current + amount);
            OnCurrencyChanged?.Invoke(type, _save.Data.currencies.GetCurrency(type));
        }

        public bool Spend(CurrencyType type, int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            int current = Get(type);
            if (current < amount)
            {
                return false;
            }

            _save.Data.currencies.SetCurrency(type, current - amount);
            OnCurrencyChanged?.Invoke(type, _save.Data.currencies.GetCurrency(type));
            return true;
        }
    }
}
