using System;
#if OPENXR_1_13_OR_NEWER
using UnityEngine.XR.OpenXR;
#endif // OPENXR_1_13_OR_NEWER

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresOpenXRFeature : IBooleanExpression
    {
        [SerializeField, SelectOpenXRFeatureTypename, Tooltip("The assembly qualified type name of the OpenXR feature.")]
        string m_RequiredFeature;

        public bool Evaluate()
        {
#if !OPENXR_1_13_OR_NEWER
            return false;
#else
            var feature = OpenXRSettings.Instance.GetFeature(Type.GetType(m_RequiredFeature));
            return feature != null && feature.enabled;
#endif
        }
    }
}
