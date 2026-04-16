using System;
#if OPENXR_1_13_OR_NEWER
using UnityEngine.XR.OpenXR;
#endif // OPENXR_1_13_OR_NEWER

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresOpenXRExtension : IBooleanExpression
    {
        [SerializeField, Tooltip("The OpenXR extension that must be enabled.")]
        string m_RequiredExtension;

        public bool Evaluate()
        {
#if !OPENXR_1_13_OR_NEWER
            return false;
#else
            return OpenXRRuntime.IsExtensionEnabled(m_RequiredExtension);
#endif
        }
    }
}
