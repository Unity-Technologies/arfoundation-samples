using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [DisallowMultipleComponent]
    public class ScreenOrientationLock : MonoBehaviour
    {
#pragma warning disable 0414
        [SerializeField, Tooltip("The orientation to lock the screen while this component is enabled.")]
        ScreenOrientation m_LockOrientation = ScreenOrientation.Portrait;
#pragma warning restore 0414

#if !UNITY_EDITOR
        void OnEnable()
        {
            if (!MenuLoader.IsHmdDevice())
                Screen.orientation = m_LockOrientation;
        }

        void OnDisable()
        {
            if (!MenuLoader.IsHmdDevice())
                Screen.orientation = ScreenOrientation.AutoRotation;
        }
#endif
    }
}
