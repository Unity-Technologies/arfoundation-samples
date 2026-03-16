#if XR_HANDS_1_2_OR_NEWER
using System.Collections.Generic;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Processing;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// A post processor for XR hand tracking data, using the One Euro filter to smooth hand positions.
    /// </summary>
#if XR_HANDS_1_2_OR_NEWER
    public class HandsOneEuroFilterPostProcessor : MonoBehaviour, IXRHandProcessor
#else
    public class HandsOneEuroFilterPostProcessor : MonoBehaviour
#endif
    {
        [SerializeField]
        [Tooltip("Smoothing amount at low speeds.")]
#pragma warning disable CS0414 // Field assigned but its value is never used -- Keep to retain serialized value when XR Hands is not installed
        float m_FilterMinCutoff = 0.1f;
#pragma warning restore CS0414

        [SerializeField]
        [Tooltip("Filter's responsiveness to speed changes.")]
#pragma warning disable CS0414 // Field assigned but its value is never used -- Keep to retain serialized value when XR Hands is not installed
        float m_FilterBeta = 0.2f;
#pragma warning restore CS0414

#if XR_HANDS_1_2_OR_NEWER
        /// <inheritdoc />
        public int callbackOrder => 0;

        readonly OneEuroFilterVector3 m_LeftHandFilter = new OneEuroFilterVector3(Vector3.zero);
        readonly OneEuroFilterVector3 m_RightHandFilter = new OneEuroFilterVector3(Vector3.zero);

        bool m_WasLeftHandTrackedLastFrame;
        bool m_WasRightHandTrackedLastFrame;

        XRHandSubsystem m_Subsystem;
        static readonly List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();
#endif

#if XR_HANDS_1_2_OR_NEWER
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDisable()
        {
            if (m_Subsystem != null)
            {
                m_Subsystem.UnregisterProcessor(this);
                m_Subsystem = null;
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            if (m_Subsystem != null && m_Subsystem.running)
                return;

            SubsystemManager.GetSubsystems(s_SubsystemsReuse);
            var foundRunningHandSubsystem = false;
            for (var i = 0; i < s_SubsystemsReuse.Count; ++i)
            {
                var handSubsystem = s_SubsystemsReuse[i];
                if (handSubsystem.running)
                {
                    m_Subsystem?.UnregisterProcessor(this);
                    m_Subsystem = handSubsystem;
                    foundRunningHandSubsystem = true;
                    break;
                }
            }

            if (!foundRunningHandSubsystem)
                return;

            m_WasLeftHandTrackedLastFrame = false;
            m_WasRightHandTrackedLastFrame = false;
            m_Subsystem.RegisterProcessor(this);
        }

        /// <inheritdoc />
        public void ProcessJoints(XRHandSubsystem subsystem, XRHandSubsystem.UpdateSuccessFlags successFlags, XRHandSubsystem.UpdateType updateType)
        {
            var leftHand = subsystem.leftHand;
            if (leftHand.isTracked)
            {
                var leftHandPose = leftHand.rootPose;
                if (!m_WasLeftHandTrackedLastFrame)
                {
                    m_LeftHandFilter.Initialize(leftHandPose.position);
                }
                else
                {
                    var newLeftPosition = m_LeftHandFilter.Filter(leftHandPose.position, Time.deltaTime, m_FilterMinCutoff, m_FilterBeta);
                    var newLeftPose = new Pose(newLeftPosition, leftHandPose.rotation);

                    leftHand.SetRootPose(newLeftPose);
                    subsystem.SetCorrespondingHand(leftHand);
                }
            }

            m_WasLeftHandTrackedLastFrame = leftHand.isTracked;

            var rightHand = subsystem.rightHand;
            if (rightHand.isTracked)
            {
                var rightHandPose = rightHand.rootPose;
                if (!m_WasRightHandTrackedLastFrame)
                {
                    m_RightHandFilter.Initialize(rightHandPose.position);
                }
                else
                {
                    var newRightPosition = m_RightHandFilter.Filter(rightHandPose.position, Time.deltaTime, m_FilterMinCutoff, m_FilterBeta);
                    var newRightPose = new Pose(newRightPosition, rightHandPose.rotation);

                    rightHand.SetRootPose(newRightPose);
                    subsystem.SetCorrespondingHand(rightHand);
                }
            }

            m_WasRightHandTrackedLastFrame = rightHand.isTracked;
        }
#else
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Awake()
        {
            Debug.LogWarning("HandsOneEuroFilterPostProcessor requires XR Hands (com.unity.xr.hands) 1.2.0 or newer. Disabling component.", this);
            enabled = false;
        }
#endif
    }
}
