using EntropySyndicate.Data;
using EntropySyndicate.Utils;
using UnityEngine;

namespace EntropySyndicate.Gameplay
{
    public class SpawnDirector
    {
        private readonly GameBalanceConfig _config;
        private readonly DeterministicObjectPool _pool;
        private float _spawnTimer;

        public SpawnDirector(GameBalanceConfig config, DeterministicObjectPool pool)
        {
            _config = config;
            _pool = pool;
        }

        public void Tick(float runSeconds, float difficultyScale)
        {
            float spawnRate = _config.spawnRateOverTime.Evaluate(runSeconds) * difficultyScale;
            _spawnTimer += Time.deltaTime;

            float interval = 1f / Mathf.Max(0.1f, spawnRate);
            while (_spawnTimer >= interval)
            {
                _spawnTimer -= interval;
                SpawnEntity();
            }
        }

        private void SpawnEntity()
        {
            Vector2 pos = Random.insideUnitCircle.normalized * 4.8f;
            GameObject entity = _pool.Get(pos);
            ChaosEntity chaos = entity.GetComponent<ChaosEntity>();
            Vector2 dir = -pos.normalized;
            chaos.Spawn(dir);
        }
    }
}
