#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
    [CreateAssetMenu(menuName = "XR/AR Foundation/Events/Meta Environment Raycast Hit Event Asset", fileName = "Meta Environment Raycast Hit Event")]
    public class EnvironmentRaycastHitEventAsset : EventAsset<EnvironmentRaycastHit>
    {
    }
#else
    public class EnvironmentRaycastHitEventAsset : ScriptableObject
    {
    }
#endif
}
