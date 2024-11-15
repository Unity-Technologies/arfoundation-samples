using System;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [Serializable]
    public class RequiresGraphicsAPI : IBooleanExpression
    {
        [SerializeField]
        GraphicsDeviceType m_RequiredGraphicsDeviceType;

        public bool Evaluate()
        {
            return SystemInfo.graphicsDeviceType == m_RequiredGraphicsDeviceType;
        }
    }
}
