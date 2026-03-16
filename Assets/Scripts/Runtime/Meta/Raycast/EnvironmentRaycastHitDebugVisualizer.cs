using System;
using UnityEngine.XR.ARSubsystems;
#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(DebugInfoDisplayController))]
    public class EnvironmentRaycastHitDebugVisualizer : MonoBehaviour
    {
        static readonly Vector3 k_CanvasVerticalOffset = new(0, 0.1f, 0);

        [SerializeField, HideInInspector]
        DebugInfoDisplayController m_DebugInfoDisplayController;

        void Reset()
        {
            m_DebugInfoDisplayController = GetComponent<DebugInfoDisplayController>();
        }

        private void Awake()
        {
            if (m_DebugInfoDisplayController == null)
                m_DebugInfoDisplayController = GetComponent<DebugInfoDisplayController>();
        }

#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
        public void SetHitStatus(EnvironmentRaycastHitStatus newHitStatus)
        {
            m_DebugInfoDisplayController.SetBottomPivot();
            m_DebugInfoDisplayController.AppendDebugEntry("Hit Status: ", newHitStatus.ToString());
            m_DebugInfoDisplayController.RefreshDisplayInfo();

            m_DebugInfoDisplayController.SetPosition(this.transform.position + k_CanvasVerticalOffset);
        }
#endif
    }
}
