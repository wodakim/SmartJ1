using UnityEngine;

namespace EntropySyndicate.Data
{
    [CreateAssetMenu(fileName = "BuildRuntimeConfig", menuName = "EntropySyndicate/Build Runtime Config")]
    public class BuildRuntimeConfig : ScriptableObject
    {
        [Header("Build Flags")]
        public bool productionMode = false;
        public bool debugMode = true;

        [Header("Test Session Mode")]
        public bool testModeEnabled = false;
        public float forcedSessionLengthSeconds = 180f;
        public float progressionMultiplier = 2f;
        public float rewardVisibilityMultiplier = 1.6f;
        public bool analyticsOverlayOnByDefault = true;
    }
}
