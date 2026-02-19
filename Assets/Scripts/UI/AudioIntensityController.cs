using UnityEngine;

namespace EntropySyndicate.UI
{
    public class AudioIntensityController : MonoBehaviour
    {
        [SerializeField] private AudioSource ambientLayer;
        [SerializeField] private AudioSource tensionLayer;
        [SerializeField] private AudioSource chaosLayer;

        public void SetIntensity(float value)
        {
            float normalized = Mathf.Clamp01(value);
            if (ambientLayer != null) ambientLayer.volume = Mathf.Lerp(0.6f, 0.25f, normalized);
            if (tensionLayer != null) tensionLayer.volume = Mathf.Lerp(0f, 0.85f, normalized);
            if (chaosLayer != null) chaosLayer.volume = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0f, 1f, normalized));
        }
    }
}
