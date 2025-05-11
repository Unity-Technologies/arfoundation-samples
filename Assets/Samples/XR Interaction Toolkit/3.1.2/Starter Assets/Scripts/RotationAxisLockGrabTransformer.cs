using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// An XR grab transformer that allows for the locking of specific rotation axes. When an object is grabbed and manipulated,
    /// this class ensures that rotations are only applied to the specified axes, preserving the initial rotation for the others.
    /// </summary>
    public class RotationAxisLockGrabTransformer : XRBaseGrabTransformer
    {
        [SerializeField]
        [Tooltip("Defines which rotation axes are allowed when an object is grabbed. Axes not selected will maintain their initial rotation.")]
        XRGeneralGrabTransformer.ManipulationAxes m_PermittedRotationAxis = XRGeneralGrabTransformer.ManipulationAxes.All;

        /// <inheritdoc />
        protected override RegistrationMode registrationMode => RegistrationMode.SingleAndMultiple;

        Vector3 m_InitialEulerRotation;

        /// <inheritdoc />
        public override void OnLink(XRGrabInteractable grabInteractable)
        {
            base.OnLink(grabInteractable);
            m_InitialEulerRotation = grabInteractable.transform.rotation.eulerAngles;
        }

        /// <inheritdoc />
        public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
            Vector3 newRotationEuler = targetPose.rotation.eulerAngles;

            if ((m_PermittedRotationAxis & XRGeneralGrabTransformer.ManipulationAxes.X) == 0)
                newRotationEuler.x = m_InitialEulerRotation.x;

            if ((m_PermittedRotationAxis & XRGeneralGrabTransformer.ManipulationAxes.Y) == 0)
                newRotationEuler.y = m_InitialEulerRotation.y;

            if ((m_PermittedRotationAxis & XRGeneralGrabTransformer.ManipulationAxes.Z) == 0)
                newRotationEuler.z = m_InitialEulerRotation.z;

            targetPose.rotation = Quaternion.Euler(newRotationEuler);
        }
    }
}
