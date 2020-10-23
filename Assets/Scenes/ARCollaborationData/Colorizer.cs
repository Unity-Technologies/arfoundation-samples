using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Sets the color of <see cref="renderer"/>.
    /// </summary>
    public class Colorizer : MonoBehaviour
    {
        [SerializeField]
        Renderer m_Renderer;

#if UNITY_EDITOR
        public new Renderer renderer
#else
        public Renderer renderer
#endif
        {
            get => m_Renderer;
            set => m_Renderer = value;
        }

        public Color color
        {
            get => m_Renderer ? m_Renderer.material.color : Color.white;
            set
            {
                if (m_Renderer)
                {
                    m_Renderer.material.color = value;
                }
            }
        }
    }
}
