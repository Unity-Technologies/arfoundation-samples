using Unity.XR.CompositionLayers;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class SetCompositionLayerOrder : MonoBehaviour
    {
        [SerializeField]
        int m_Layer;

        [SerializeField]
        CompositionLayer m_CompositionLayer;

        void Start()
        {
            m_CompositionLayer.Order = m_Layer;
        }
    }
}
