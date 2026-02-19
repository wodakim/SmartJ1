using System;
using System.IO;
using EntropySyndicate.Data;
using UnityEngine;

namespace EntropySyndicate.Services
{
    public class SaveService
    {
        [Serializable]
        private struct SaveContainer
        {
            public string payload;
            public string digest;
        }

        private const string SaveFileName = "entropy_syndicate_save.json";
        public PlayerSaveData Data { get; private set; }

        public void LoadOrCreate()
        {
            string path = Path.Combine(Application.persistentDataPath, SaveFileName);
            if (!File.Exists(path))
            {
                Data = new PlayerSaveData();
                Persist();
                return;
            }

            try
            {
                string content = File.ReadAllText(path);
                SaveContainer container = JsonUtility.FromJson<SaveContainer>(content);
                string expected = Utils.CryptoUtils.ComputeDigest(container.payload);

                if (expected != container.digest)
                {
                    SetTamperedFreshSave();
                    return;
                }

                string decoded = Utils.CryptoUtils.Unprotect(container.payload);
                Data = JsonUtility.FromJson<PlayerSaveData>(decoded);
                if (Data == null)
                {
                    SetTamperedFreshSave();
                    return;
                }

                NormalizeData();
            }
            catch
            {
                SetTamperedFreshSave();
            }
        }

        private void NormalizeData()
        {
            if (Data.shardMastery == null) Data.shardMastery = new System.Collections.Generic.List<IntEntry>();
            if (Data.forgeLevels == null) Data.forgeLevels = new System.Collections.Generic.List<IntEntry>();
            if (Data.missionProgress == null) Data.missionProgress = new System.Collections.Generic.List<IntEntry>();
            if (Data.missionClaimed == null) Data.missionClaimed = new System.Collections.Generic.List<BoolEntry>();
            if (Data.currencies == null || Data.currencies.Count == 0)
            {
                Data.currencies = new System.Collections.Generic.List<CurrencyEntry>
                {
                    new CurrencyEntry { type = CurrencyType.Scrap, amount = 0 },
                    new CurrencyEntry { type = CurrencyType.Sigils, amount = 0 },
                    new CurrencyEntry { type = CurrencyType.Prisms, amount = 0 }
                };
            }
        }

        private void SetTamperedFreshSave()
        {
            Data = new PlayerSaveData();
            Data.tamperDetected = true;
            Data.leaderboardEnabled = false;
            Persist();
        }

        public void Persist()
        {
            string path = Path.Combine(Application.persistentDataPath, SaveFileName);
            string json = JsonUtility.ToJson(Data);
            string payload = Utils.CryptoUtils.Protect(json);
            SaveContainer container = new SaveContainer
            {
                payload = payload,
                digest = Utils.CryptoUtils.ComputeDigest(payload)
            };
            File.WriteAllText(path, JsonUtility.ToJson(container));
        }
    }
}
