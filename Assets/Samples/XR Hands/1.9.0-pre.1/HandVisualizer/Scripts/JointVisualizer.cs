using UnityEngine;
using UnityEngine.XR.Hands.Processing;

namespace UnityEngine.XR.Hands.Samples.VisualizerSample
{
    public class JointVisualizer : MonoBehaviour
    {
        [SerializeField]
        GameObject m_JointVisual;

        [SerializeField]
        Material m_HighFidelityJointMaterial;

        [SerializeField]
        Material m_LowFidelityJointMaterial;

        bool m_HighFidelityJoint;

        Renderer m_JointRenderer;

        public void NotifyTrackingState(XRHandJointTrackingState jointTrackingState)
        {
            bool highFidelityJoint = (jointTrackingState & XRHandJointTrackingState.HighFidelityPose) == XRHandJointTrackingState.HighFidelityPose;
            if (m_HighFidelityJoint == highFidelityJoint)
                return;

            m_JointRenderer.material = highFidelityJoint ? m_HighFidelityJointMaterial : m_LowFidelityJointMaterial;

            m_HighFidelityJoint = highFidelityJoint;
        }

        void Start()
        {
            if (m_JointVisual.TryGetComponent<Renderer>(out var jointRenderer))
                m_JointRenderer = jointRenderer;
        }
    }
}
