using UnityEngine;

namespace EntropySyndicate.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChaosEntity : MonoBehaviour
    {
        [SerializeField] private float baseSpeed = 2.5f;
        [SerializeField] private int scrapValue = 3;

        private Rigidbody2D _rb;
        private Vector2 _direction;
        private float _speedMultiplier = 1f;

        public int ScrapValue => scrapValue;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Spawn(Vector2 direction)
        {
            _direction = direction.normalized;
            _speedMultiplier = 1f;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _speedMultiplier = multiplier;
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _direction * baseSpeed * _speedMultiplier;
        }
    }
}
