using UnityEngine;

namespace EntropySyndicate.UI
{
    public class JuiceController : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private ParticleSystem shardParticles;
        [SerializeField] private ParticleSystem hitParticles;
        [SerializeField] [Range(0f, 2f)] private float shakeAmplitude = 0.18f;
        [SerializeField] [Range(0f, 0.2f)] private float hitStopSeconds = 0.03f;

        private Vector3 _cameraOrigin;
        private float _intensity;
        private float _shakeTime;

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (targetCamera != null)
            {
                _cameraOrigin = targetCamera.transform.position;
            }
        }

        public void SetIntensity(float value)
        {
            _intensity = Mathf.Clamp01(value);
        }

        public void PlayShardFeedback(Vector3 worldPosition)
        {
            if (shardParticles != null)
            {
                shardParticles.transform.position = worldPosition;
                shardParticles.Play();
            }

            _shakeTime = 0.12f;
        }

        public void PlayHitFeedback(Vector3 worldPosition, float damage)
        {
            if (hitParticles != null)
            {
                hitParticles.transform.position = worldPosition;
                hitParticles.Play();
            }

            Time.timeScale = 0.01f;
            Invoke(nameof(ReleaseHitStop), hitStopSeconds);
            _shakeTime = 0.2f;
        }

        private void ReleaseHitStop()
        {
            Time.timeScale = 1f;
        }

        private void LateUpdate()
        {
            if (targetCamera == null)
            {
                return;
            }

            if (_shakeTime > 0f)
            {
                _shakeTime -= Time.unscaledDeltaTime;
                float amp = shakeAmplitude * (1f + _intensity);
                targetCamera.transform.position = _cameraOrigin + (Vector3)Random.insideUnitCircle * amp;
                return;
            }

            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, _cameraOrigin, 10f * Time.unscaledDeltaTime);
        }
    }
}
