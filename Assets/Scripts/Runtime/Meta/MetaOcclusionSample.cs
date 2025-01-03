using TMPro;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
#if METAOPENXR_2_1_OR_NEWER
using UnityEngine.XR.OpenXR.Features.Meta;
#endif // METAOPENXR_2_1_OR_NEWER

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaOcclusionSample : MonoBehaviour
    {
        const string k_EnabledMsg = "Hand removal state:\nEnabled";
        const string k_DisabledMsg = "Hand removal state\nDisabled";
        const string k_NotSupportedMsg = "Hand removal state:\nNot supported";

        [SerializeField, Tooltip("Writes whether hand removal is supported.")]
        TextMeshProUGUI m_DebugText;

#if METAOPENXR_2_1_OR_NEWER
        MetaOpenXROcclusionSubsystem m_OcclusionSubsystem;

        void OnEnable()
        {
            if (!SubsystemsUtility.TryGetLoadedSubsystem<XROcclusionSubsystem, MetaOpenXROcclusionSubsystem>(out m_OcclusionSubsystem))
            {
                Debug.LogError("Meta Occlusion subsystem not loaded.");
                enabled = false;
                return;
            }
            if (m_DebugText == null)
            {
                Debug.LogError($"{nameof(MetaOcclusionSample)} has null references. Fix in the Inspector.", this);
                enabled = false;
                return;
            }

            UpdateDebugText();
        }

        void UpdateDebugText()
        {
            if (!enabled)
                return;

            m_DebugText.text = m_OcclusionSubsystem.isHandRemovalSupported switch
            {
                Supported.Unsupported => k_NotSupportedMsg,
                Supported.Supported => m_OcclusionSubsystem.isHandRemovalEnabled ? k_EnabledMsg : k_DisabledMsg,
                _ => m_DebugText.text
            };
        }

        public void ToggleHandRemoval()
        {
            var result = m_OcclusionSubsystem.TrySetHandRemovalEnabled(!m_OcclusionSubsystem.isHandRemovalEnabled);
            if (result < 0)
                Debug.LogError($"Setting hand removal failed with error: {result.ToString()}");
            else
                UpdateDebugText();
        }
#endif // METAOPENXR_2_1_OR_NEWER
    }
}
