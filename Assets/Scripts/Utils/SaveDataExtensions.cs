using System.Collections.Generic;
using EntropySyndicate.Data;

namespace EntropySyndicate.Utils
{
    public static class SaveDataExtensions
    {
        public static int GetInt(this List<IntEntry> list, string key)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].key == key)
                {
                    return list[i].value;
                }
            }

            return 0;
        }

        public static void SetInt(this List<IntEntry> list, string key, int value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].key == key)
                {
                    IntEntry updated = list[i];
                    updated.value = value;
                    list[i] = updated;
                    return;
                }
            }

            list.Add(new IntEntry { key = key, value = value });
        }

        public static bool GetBool(this List<BoolEntry> list, string key)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].key == key)
                {
                    return list[i].value;
                }
            }

            return false;
        }

        public static void SetBool(this List<BoolEntry> list, string key, bool value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].key == key)
                {
                    BoolEntry updated = list[i];
                    updated.value = value;
                    list[i] = updated;
                    return;
                }
            }

            list.Add(new BoolEntry { key = key, value = value });
        }

        public static int GetCurrency(this List<CurrencyEntry> list, CurrencyType type)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].type == type)
                {
                    return list[i].amount;
                }
            }

            return 0;
        }

        public static void SetCurrency(this List<CurrencyEntry> list, CurrencyType type, int value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].type == type)
                {
                    CurrencyEntry updated = list[i];
                    updated.amount = value;
                    list[i] = updated;
                    return;
                }
            }

            list.Add(new CurrencyEntry { type = type, amount = value });
        }
    }
}
