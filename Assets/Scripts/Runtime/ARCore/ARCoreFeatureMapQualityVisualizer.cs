#if UNITY_ANDROID && ARCORE_4_2_OR_NEWER && !UNITY_EDITOR
using UnityEngine.XR.ARCore;
#endif
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARAnchorDebugVisualizer))]
    public class ARCoreFeatureMapQualityVisualizer : MonoBehaviour
    {
#if UNITY_ANDROID && ARCORE_4_2_OR_NEWER && !UNITY_EDITOR
        ARAnchorDebugVisualizer m_AnchorDebugVisualizer;

        ArFeatureMapQuality m_CurrentFeatureMapQuality;

        void Awake()
        {
            if (m_AnchorDebugVisualizer == null)
                m_AnchorDebugVisualizer = GetComponent<ARAnchorDebugVisualizer>();
        }

        void OnEnable()
        {
            m_AnchorDebugVisualizer.debugInfoChanged += HandleDebugInfoChanged;
            m_AnchorDebugVisualizer.hasDebugInfoChanged += HasFeatureMapQualityChanged;

            m_CurrentFeatureMapQuality = ArFeatureMapQuality.AR_FEATURE_MAP_QUALITY_INSUFFICIENT;
        }

        void OnDisable()
        {
            m_AnchorDebugVisualizer.debugInfoChanged -= HandleDebugInfoChanged;
            m_AnchorDebugVisualizer.hasDebugInfoChanged -= HasFeatureMapQualityChanged;

            m_CurrentFeatureMapQuality = ArFeatureMapQuality.AR_FEATURE_MAP_QUALITY_INSUFFICIENT;
        }

        void HandleDebugInfoChanged(ARAnchorManager anchorManager, DebugInfoDisplayController debugInfoDisplayController)
        {
            // note: in event that the code still compiles, we do a runtime check against the 
            // current anchor subsystem and only show the feature map quality if we are actually
            // running the arcore anchor subsystem.
            if (anchorManager.subsystem is ARCoreAnchorSubsystem)
                debugInfoDisplayController.AppendDebugEntry("Quality:", m_CurrentFeatureMapQuality.ToString());
        }

        bool HasFeatureMapQualityChanged(TrackableId trackableId, ARAnchorManager anchorManager)
        {
            var hasChanged = false;
            ArFeatureMapQuality quality = default;
            if (anchorManager.subsystem is ARCoreAnchorSubsystem arCoreAnchorSubsystem)
            {
                var resultStatus = arCoreAnchorSubsystem.EstimateFeatureMapQualityForHosting(trackableId, ref quality);
                if (resultStatus.IsSuccess())
                {
                    hasChanged = quality != m_CurrentFeatureMapQuality;
                    m_CurrentFeatureMapQuality = quality;
                }
            }
            return hasChanged;
        }
#endif
    }
}
