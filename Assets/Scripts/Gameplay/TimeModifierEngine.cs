using UnityEngine;

namespace EntropySyndicate.Gameplay
{
    public class TimeModifierEngine
    {
        private float _targetScale = 1f;
        private float _smoothVelocity;

        public void SetTimeScale(float scale)
        {
            _targetScale = Mathf.Clamp(scale, 0.4f, 1.3f);
        }

        public void Tick(float dt)
        {
            float newScale = Mathf.SmoothDamp(Time.timeScale, _targetScale, ref _smoothVelocity, 0.2f, 3f, dt);
            Time.timeScale = newScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}
