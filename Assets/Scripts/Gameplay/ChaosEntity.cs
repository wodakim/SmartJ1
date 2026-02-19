using EntropySyndicate.Utils;
using UnityEngine;

namespace EntropySyndicate.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChaosEntity : MonoBehaviour
    {
        [SerializeField] private float baseSpeed = 2.5f;
        [SerializeField] private int scrapValue = 3;
        [SerializeField] private float maxLifetimeSeconds = 8f;
        [SerializeField] private float despawnRadius = 7f;

        private Rigidbody2D _rb;
        private Vector2 _direction;
        private float _speedMultiplier = 1f;
        private float _lifetime;
        private PooledObject _pooledObject;

        public int ScrapValue => scrapValue;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _pooledObject = GetComponent<PooledObject>();
        }

        public void Spawn(Vector2 direction)
        {
            _direction = direction.normalized;
            _speedMultiplier = 1f;
            _lifetime = 0f;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _speedMultiplier = multiplier;
        }

        private void FixedUpdate()
        {
            _rb.velocity = _direction * baseSpeed * _speedMultiplier;
            _lifetime += Time.fixedDeltaTime;

            if (_lifetime >= maxLifetimeSeconds || transform.position.sqrMagnitude >= despawnRadius * despawnRadius)
            {
                _pooledObject.Release();
            }
        }
    }
}
