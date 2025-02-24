using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Takes an <see cref="InputActionReferences"/> asset and performs a raycast on each screen tap or trigger press
    /// input. Raises an <see cref="ARRaycastHitEventAsset"/> if anything was hit.
    /// </summary>
    public class PoseEventController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The active XR Origin in the scene.")]
        XROrigin m_XROrigin;

        [SerializeField]
        [Tooltip(
            "The Input Action References to use. You can create this by right clicking in the Project Window " +
            "and going to <b>XR</b> > AR Foundation > Input Action References.")]
        InputActionReferences m_InputActionReferences;

        [SerializeField]
        [Tooltip("Event to raise if anything was hit by the raycast.")]
        PoseEventAsset m_PoseEvent;

        public XROrigin xrOrigin
        {
            get => m_XROrigin;
            set => m_XROrigin = value;
        }

        public InputActionReferences inputActions => m_InputActionReferences;

        public PoseEventAsset raycastHitEventAsset
        {
            get => m_PoseEvent;
            set => m_PoseEvent = value;
        }

        void OnEnable()
        {
            if (m_XROrigin == null || m_InputActionReferences == null)
            {
                Debug.LogWarning(
                    $"{nameof(RaycastEventController)} component on {name} has null inputs and will have no effect in this scene.",
                    this);
                return;
            }

            if (m_InputActionReferences.screenTap.action != null)
                m_InputActionReferences.screenTap.action.performed += ScreenTapped;

            if (m_InputActionReferences.rightTriggerPressed.action != null)
                m_InputActionReferences.rightTriggerPressed.action.performed += RightTriggerPressed;

            if (m_InputActionReferences.leftTriggerPressed.action != null)
                m_InputActionReferences.leftTriggerPressed.action.performed += LeftTriggerPressed;
        }

        void OnDisable()
        {
            if (m_InputActionReferences == null)
                return;

            if (m_InputActionReferences.screenTap.action != null)
                m_InputActionReferences.screenTap.action.performed -= ScreenTapped;

            if (m_InputActionReferences.rightTriggerPressed.action != null)
                m_InputActionReferences.rightTriggerPressed.action.performed -= RightTriggerPressed;

            if (m_InputActionReferences.leftTriggerPressed.action != null)
                m_InputActionReferences.leftTriggerPressed.action.performed -= LeftTriggerPressed;
        }

        void ScreenTapped(InputAction.CallbackContext context)
        {
            var pointer = context.control.device as Pointer;
            if (pointer == null)
            {
                Debug.LogError(
                    "Input actions are incorrectly configured. Expected a Pointer binding ScreenTapped.",
                    this);
                return;
            }

            var tapPosition = pointer.position.ReadValue();

            if (m_XROrigin == null)
                return;

            var screenRay = m_XROrigin.Camera.ScreenPointToRay(tapPosition);
            var worldPos = screenRay.origin;
            var worldRot = Quaternion.LookRotation(screenRay.direction);
            var outputPose = new Pose(worldPos, worldRot);

            if (m_PoseEvent != null)
            {
                m_PoseEvent.Raise(outputPose);
            }
        }

        void RightTriggerPressed(InputAction.CallbackContext context)
        {
            RaycastFromHandPose(
                new Pose(
                    m_InputActionReferences.rightHandPosition.action.ReadValue<Vector3>(),
                    m_InputActionReferences.rightHandRotation.action.ReadValue<Quaternion>()));
        }

        void LeftTriggerPressed(InputAction.CallbackContext context)
        {
            RaycastFromHandPose(
                new Pose(
                    m_InputActionReferences.leftHandPosition.action.ReadValue<Vector3>(),
                    m_InputActionReferences.leftHandRotation.action.ReadValue<Quaternion>()));
        }

        void RaycastFromHandPose(Pose handPose)
        {
            if (m_PoseEvent != null)
            {
                m_PoseEvent.Raise(handPose);
            }
        }

        /// <summary>
        /// Automatically initialize serialized fields when component is first added.
        /// </summary>
        void Reset()
        {
            m_XROrigin = GetComponent<XROrigin>();
            if (m_XROrigin == null)
            {
                m_XROrigin = FindAnyObjectByType<XROrigin>();
            }
        }
    }
}
