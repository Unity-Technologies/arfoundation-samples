using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Takes an <see cref="InputActionReferences"/> asset and performs a raycast on each screen tap or trigger press
    /// input. Raises an <see cref="ARRaycastHitEventAsset"/> if anything was hit.
    /// </summary>
    public class EnvironmentRaycastEventController : MonoBehaviour
    {
#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
        static EnvironmentRaycastHit s_Hit = new();
#endif
        static Ray s_RaycastRay;

        [SerializeField]
        [Tooltip("The active XR Origin in the scene.")]
        XROrigin m_XROrigin;

        [SerializeField]
        [Tooltip("The active AR Raycast Manager in the scene.")]
        EnvironmentRaycaster m_RaycastManager;

        [SerializeField]
        [Tooltip("The Input Action References to use. You can create this by right clicking in the Project Window " +
             "and going to <b>XR</b> > AR Foundation > Input Action References.")]
        InputActionReferences m_InputActionReferences;

        [SerializeField]
        [Tooltip("Event to raise if anything was hit by the raycast.")]
        EnvironmentRaycastHitEventAsset m_AREnvironmentRaycastHitEvent;

        [SerializeField]
        [Tooltip("Optional event system to check for UI hits before emitting raycast events.")]
        EventSystem m_EventSystem;

        [SerializeField]
        [Tooltip("Optional graphic raycaster to check for UI hits before emitting raycast events.")]
        GraphicRaycaster m_GraphicRaycaster;

        public InputActionReferences inputActions => m_InputActionReferences;

        public EnvironmentRaycastHitEventAsset raycastHitEventAsset
        {
            get => m_AREnvironmentRaycastHitEvent;
            set => m_AREnvironmentRaycastHitEvent = value;
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
            if (m_XROrigin == null || m_InputActionReferences == null)
            {
                Debug.LogWarning($"{nameof(RaycastEventController)} component on {name} has null inputs and will have no effect in this scene.", this);
                return;
            }

            if (m_RaycastManager == null)
                Debug.LogWarning($"{nameof(RaycastEventController)} component on {name} has no raycast manager assigned and will not work.", this);

            if (m_InputActionReferences.rightTriggerPressed.action != null)
                m_InputActionReferences.rightTriggerPressed.action.performed += RightTriggerPressed;

            if (m_InputActionReferences.leftTriggerPressed.action != null)
                m_InputActionReferences.leftTriggerPressed.action.performed += LeftTriggerPressed;
        }

        void OnDisable()
        {
            if (m_InputActionReferences == null)
                return;

            if (m_InputActionReferences.rightTriggerPressed.action != null)
                m_InputActionReferences.rightTriggerPressed.action.performed -= RightTriggerPressed;

            if (m_InputActionReferences.leftTriggerPressed.action != null)
                m_InputActionReferences.leftTriggerPressed.action.performed -= LeftTriggerPressed;
        }

        Transform GetTrackablesParent()
        {
            return FindAnyObjectByType<XROrigin>()?.TrackablesParent;
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
#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
            s_RaycastRay = new Ray(handPose.position, handPose.forward);
            var size = Physics.RaycastNonAlloc(s_RaycastRay, m_UIRaycastHits, float.PositiveInfinity, m_UILayerMask);
            if (size > 0)
                return;

            if (m_AREnvironmentRaycastHitEvent != null &&
                (m_RaycastManager?.Raycast(s_RaycastRay, ref s_Hit) ?? false))
            {
                if (s_Hit.IsHit())
                    m_AREnvironmentRaycastHitEvent.Raise(s_Hit);
            }
#endif
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

            m_RaycastManager = GetComponent<EnvironmentRaycaster>();
            if (m_RaycastManager == null)
            {
                m_RaycastManager = FindAnyObjectByType<EnvironmentRaycaster>();
            }
        }
    }
}
