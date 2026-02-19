using UnityEngine;

namespace EntropySyndicate.UI
{
    [RequireComponent(typeof(Camera))]
    public class ScreenDistortionController : MonoBehaviour
    {
        [SerializeField] private Material distortionMaterial;
        [SerializeField] [Range(0f, 1f)] private float strength = 0.1f;

        public void SetStrength(float value)
        {
            strength = Mathf.Clamp01(value);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (distortionMaterial == null)
            {
                Graphics.Blit(src, dest);
                return;
            }

            distortionMaterial.SetFloat("_DistortionStrength", strength);
            Graphics.Blit(src, dest, distortionMaterial);
        }
    }
}
