#if METAOPENXR_2_1_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
using TMPro;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Meta;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaBoundaryVisibilitySample : MonoBehaviour
    {
        BoundaryVisibilityFeature m_Feature;

        [SerializeField]
        TextMeshProUGUI m_DebugText;

        public void ToggleBoundaryVisibility()
        {
            if (!enabled)
                return;

            var result = m_Feature.TryRequestBoundaryVisibility(GetOpposite(m_Feature.currentVisibility));
            if (result != XrResult.Success)
            {
                if ((int)result == BoundaryVisibilityFeature.XR_BOUNDARY_VISIBILITY_SUPPRESSION_NOT_ALLOWED_META)
                {
                    Debug.Log("Boundary visibility suppression is not allowed. This is expected if passthrough is not enabled.");
                }
                else
                {
                    Debug.Log($"Toggle boundary visibility returned a result other than unqualified success: {result}");
                }
            }
        }

        void Start()
        {
            m_Feature = OpenXRSettings.Instance.GetFeature<BoundaryVisibilityFeature>();

            if (m_Feature == null)
            {
                Debug.LogError($"{nameof(BoundaryVisibilityFeature)} is null");
                enabled = false;
                return;
            }

            if (!m_Feature.enabled)
            {
                Debug.LogError($"{nameof(BoundaryVisibilityFeature)} isn't enabled. Enable it in XR Plug-in Management.");
                enabled = false;
                return;
            }

            m_Feature.boundaryVisibilityChanged += OnVisibilityChanged;
            RenderDebugText();
        }

        void OnVisibilityChanged(object sender, XrBoundaryVisibility visibility)
        {
            RenderDebugText();
        }

        void RenderDebugText()
        {
            if (m_DebugText == null)
                return;

            m_DebugText.text = $"Current visibility:\n{m_Feature.currentVisibility}";
        }

        static XrBoundaryVisibility GetOpposite(XrBoundaryVisibility visibility)
        {
            return visibility == XrBoundaryVisibility.VisibilitySuppressed ?
                    XrBoundaryVisibility.VisibilityNotSuppressed :
                    XrBoundaryVisibility.VisibilitySuppressed;
        }
    }
}
#endif // METAOPENXR_2_1_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
