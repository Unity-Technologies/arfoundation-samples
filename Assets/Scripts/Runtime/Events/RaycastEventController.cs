using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Takes an <see cref="InputActionReferences"/> asset and performs a raycast on each screen tap or trigger press
    /// input. Raises an <see cref="ARRaycastHitEventAsset"/> if anything was hit.
    /// </summary>
    public class RaycastEventController : MonoBehaviour
    {
        static List<ARRaycastHit> s_Hits = new();
        static Ray s_RaycastRay;

        [SerializeField]
        [Tooltip("The active XR Origin in the scene.")]
        XROrigin m_XROrigin;

        [SerializeField]
        [Tooltip("The active AR Raycast Manager in the scene.")]
        ARRaycastManager m_RaycastManager;

        [SerializeField]
        [Tooltip("The Input Action References to use. You can create this by right clicking in the Project Window " +
             "and going to <b>XR</b> > AR Foundation > Input Action References.")]
        InputActionReferences m_InputActionReferences;

        [SerializeField]
        [Tooltip("Event to raise if anything was hit by the raycast.")]
        ARRaycastHitEventAsset m_ARRaycastHitEvent;

        [SerializeField]
        [Tooltip("The type of trackable the raycast will hit.")]
        TrackableType m_TrackableType = TrackableType.PlaneWithinPolygon;

        public InputActionReferences inputActions => m_InputActionReferences;

        public ARRaycastHitEventAsset raycastHitEventAsset
        {
            get => m_ARRaycastHitEvent;
            set => m_ARRaycastHitEvent = value;
        }

        public TrackableType trackableType
        {
            get => m_TrackableType;
            set => m_TrackableType = value;
        }

        Camera m_Camera;
        LayerMask m_UILayerMask;
        RaycastHit[] m_UIRaycastHits = new RaycastHit[1];

        void Awake()
        {
            m_Camera = Camera.main;
            var uiLayer = LayerMask.NameToLayer("UI");
            m_UILayerMask = 1 << uiLayer;
        }

        void OnEnable()
        {
            if (m_RaycastManager == null || m_XROrigin == null || m_InputActionReferences == null)
            {
                Debug.LogWarning($"{nameof(RaycastEventController)} component on {name} has null inputs and will have no effect in this scene.", this);
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
            if (context.control.device is not Pointer pointer)
            {
               Debug.LogError("Input actions are incorrectly configured. Expected a Pointer binding ScreenTapped.", this);
               return;
            }

            var tapPosition = pointer.position.ReadValue();
            if (m_ARRaycastHitEvent != null &&
                m_RaycastManager.Raycast(tapPosition, s_Hits, m_TrackableType))
            {
                m_ARRaycastHitEvent.Raise(s_Hits[0]);
            }
        }

        void RightTriggerPressed(InputAction.CallbackContext context)
        {
            RaycastFromHandPose(new Pose(
                m_InputActionReferences.rightHandPosition.action.ReadValue<Vector3>(),
                m_InputActionReferences.rightHandRotation.action.ReadValue<Quaternion>()));
        }

        void LeftTriggerPressed(InputAction.CallbackContext context)
        {
            RaycastFromHandPose(new Pose(
                m_InputActionReferences.leftHandPosition.action.ReadValue<Vector3>(),
                m_InputActionReferences.leftHandRotation.action.ReadValue<Quaternion>()));
        }

        void RaycastFromHandPose(Pose handPose)
        {
            s_RaycastRay = new Ray(handPose.position, handPose.forward);
            var size = Physics.RaycastNonAlloc(s_RaycastRay, m_UIRaycastHits, float.PositiveInfinity, m_UILayerMask);
            if (size > 0)
                return;

            if (m_ARRaycastHitEvent != null &&
                m_RaycastManager.Raycast(s_RaycastRay, s_Hits, m_TrackableType))
            {
                m_ARRaycastHitEvent.Raise(s_Hits[0]);
            }
        }

        /// <summary>
        /// Automatically initialize serialized fields when component is first added.
        /// </summary>
        void Reset()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
            m_XROrigin = GetComponent<XROrigin>();

            if (m_XROrigin == null)
            {
#if UNITY_2023_1_OR_NEWER
                m_XROrigin = FindAnyObjectByType<XROrigin>();
#else
                m_XROrigin = FindObjectOfType<XROrigin>();
#endif
            }

            if (m_RaycastManager == null)
            {
#if UNITY_2023_1_OR_NEWER
                m_RaycastManager = FindAnyObjectByType<ARRaycastManager>();
#else
                m_RaycastManager = FindObjectOfType<ARRaycastManager>();
#endif
            }
        }
    }
}
