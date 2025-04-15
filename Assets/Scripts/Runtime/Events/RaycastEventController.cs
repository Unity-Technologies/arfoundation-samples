using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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

        [SerializeField]
        [Tooltip("Optional event system to check for UI hits before emitting raycast events.")]
        EventSystem m_EventSystem;

        [SerializeField]
        [Tooltip("Optional graphic raycaster to check for UI hits before emitting raycast events.")]
        GraphicRaycaster m_GraphicRaycaster;

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

        LayerMask m_UILayerMask;
        RaycastHit[] m_UIRaycastHits = new RaycastHit[1];
        bool m_HasGraphicRaycaster;
        List<RaycastResult> m_GraphicRaycastResults = new();
        PointerEventData m_PointerEventData;

        void Awake()
        {
            var uiLayer = LayerMask.NameToLayer("UI");
            m_UILayerMask = 1 << uiLayer;

            // if a GraphicRaycaster has been assigned, but not an EventSystem, try to
            // find one.  If one cannot be found, then we clear out the GraphicRaycaster
            // since an EventSystem is required to do the UI raycast.
            if (m_GraphicRaycaster != null && m_EventSystem == null)
            {
                m_EventSystem = FindAnyObjectByType<EventSystem>();
                if (m_EventSystem == null)
                {
                    m_GraphicRaycaster = null;
                }
            }
            
            m_HasGraphicRaycaster = m_GraphicRaycaster != null;
            m_PointerEventData = new PointerEventData(m_EventSystem);
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

            if (m_HasGraphicRaycaster)
            {
                m_GraphicRaycastResults.Clear();
                m_PointerEventData.Reset();
                m_PointerEventData.position = pointer.position.ReadValue();
                m_GraphicRaycaster.Raycast(m_PointerEventData, m_GraphicRaycastResults);
                if (m_GraphicRaycastResults.Count > 0)
                {
                    return;
                }
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
            m_XROrigin = GetComponent<XROrigin>();
            if (m_XROrigin == null)
            {
                m_XROrigin = FindAnyObjectByType<XROrigin>();
            }

            m_RaycastManager = GetComponent<ARRaycastManager>();
            if (m_RaycastManager == null)
            {
                m_RaycastManager = FindAnyObjectByType<ARRaycastManager>();
            }
        }
    }
}
