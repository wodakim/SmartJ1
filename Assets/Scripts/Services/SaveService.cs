using System;
using System.IO;
using EntropySyndicate.Data;
using EntropySyndicate.Utils;
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

            string content = File.ReadAllText(path);
            SaveContainer container = JsonUtility.FromJson<SaveContainer>(content);
            string expected = CryptoUtils.ComputeDigest(container.payload);

            if (expected != container.digest)
            {
                Data = new PlayerSaveData();
                Data.tamperDetected = true;
                Data.leaderboardEnabled = false;
                Persist();
                return;
            }

            string decoded = CryptoUtils.Unprotect(container.payload);
            Data = JsonUtility.FromJson<PlayerSaveData>(decoded);
            if (Data == null)
            {
                Data = new PlayerSaveData();
            }
        }

        public void Persist()
        {
            string path = Path.Combine(Application.persistentDataPath, SaveFileName);
            string json = JsonUtility.ToJson(Data);
            string payload = CryptoUtils.Protect(json);
            SaveContainer container = new SaveContainer
            {
                payload = payload,
                digest = CryptoUtils.ComputeDigest(payload)
            };
            File.WriteAllText(path, JsonUtility.ToJson(container));
        }
    }
}
