using UnityEngine;

namespace EntropySyndicate.UI
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private JuiceController juice;
        [SerializeField] private AudioIntensityController audioController;

        public void SetMasterVolume(float value)
        {
            AudioListener.volume = Mathf.Clamp01(value);
        }

        public void SetTargetFrameRate(int value)
        {
            Application.targetFrameRate = value;
        }

        public void SetEffectIntensity(float value)
        {
            juice.SetIntensity(value);
            audioController.SetIntensity(value);
        }
    }
}
