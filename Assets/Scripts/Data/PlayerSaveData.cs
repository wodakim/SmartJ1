using System;
using System.Collections.Generic;

namespace EntropySyndicate.Data
{
    [Serializable]
    public struct IntEntry
    {
        public string key;
        public int value;
    }

    [Serializable]
    public struct BoolEntry
    {
        public string key;
        public bool value;
    }

    [Serializable]
    public struct CurrencyEntry
    {
        public CurrencyType type;
        public int amount;
    }

    [Serializable]
    public class PlayerSaveData
    {
        public int accountLevel = 1;
        public int prestigeCount;
        public int battlePassXp;
        public bool removeAdsOwned;
        public bool tamperDetected;
        public bool leaderboardEnabled = true;

        public List<IntEntry> shardMastery = new List<IntEntry>();
        public List<IntEntry> forgeLevels = new List<IntEntry>();
        public List<IntEntry> missionProgress = new List<IntEntry>();
        public List<BoolEntry> missionClaimed = new List<BoolEntry>();
        public List<CurrencyEntry> currencies = new List<CurrencyEntry>
        {
            new CurrencyEntry { type = CurrencyType.Scrap, amount = 0 },
            new CurrencyEntry { type = CurrencyType.Sigils, amount = 0 },
            new CurrencyEntry { type = CurrencyType.Prisms, amount = 0 }
        };
    }
}
