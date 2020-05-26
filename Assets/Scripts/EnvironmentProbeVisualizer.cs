using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(MeshRenderer))]
    public class EnvironmentProbeVisualizer : MonoBehaviour
    {
        [SerializeField]
        ReflectionProbe m_ReflectionProbe;

        public ReflectionProbe reflectionProbe
        {
            get { return m_ReflectionProbe; }
            set { m_ReflectionProbe = value; }
        }

        void Update()
        {
            if (m_ReflectionProbe == null)
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                GetComponent<MeshRenderer>().enabled = true;

                transform.localPosition = m_ReflectionProbe.center;
                transform.localScale = m_ReflectionProbe.size;

                // Unity doesn't yet support rotated reflection probes, so the visualizer
                // needs to unrotate in order to display the box that will actually be used.
                transform.localRotation = Quaternion.Inverse(m_ReflectionProbe.transform.rotation);
            }
        }
    }
}