using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Example script for demonstrating and playing with manual environment probe placement/removal.
    /// </summary>
    [RequireComponent(typeof(AREnvironmentProbeManager))]
    public class ManualEnvironmentProbeController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the AREnvironmentProbe prefab to be placed manually by this controller")]
        AREnvironmentProbe m_ProbePrefab;

        [SerializeField]
        [Tooltip("Reference to the AREnvironmentProbeManager used to query the current platform's subsystem descriptor for supported features")]
        AREnvironmentProbeManager m_ProbeManager;

        [SerializeField]
        [Tooltip("Reference to XROrigin to query for camera placement.")]
        XROrigin m_Origin;

        [SerializeField]
        [Tooltip("Reference to the button that creates a manual probe at the camera position. It is interactable only if manual placement is supported")]
        Button m_CreateButton;

        [SerializeField]
        [Tooltip("Reference to the canvas group of probe destroy button options. They are interactable only if manual removal is supported")]
        CanvasGroup m_DestroyButtonsGroup;

        readonly List<AREnvironmentProbe> m_Probes = new();
        Transform m_OriginCameraTransform;

        /// <summary>
        /// Returns true if manual placement is supported on the platform and our prefab is a valid reference.
        /// </summary>
        public bool canManuallyPlace => m_ProbePrefab != null && m_ProbeManager.descriptor.supportsManualPlacement;

        /// <summary>
        /// Returns true if manual removal is supported on the platform.
        /// </summary>
        public bool canManuallyRemove => m_ProbeManager.descriptor.supportsRemovalOfManual;

        Pose originCameraPose => new(m_OriginCameraTransform.position, m_OriginCameraTransform.rotation);

        void Start()
        {
            // descriptor info won't necessarily be ready in awake, so we get it here.
            var descriptor = m_ProbeManager.descriptor;
            m_CreateButton.interactable = descriptor?.supportsManualPlacement ?? false;
            m_DestroyButtonsGroup.interactable = descriptor?.supportsRemovalOfManual ?? false;
            m_OriginCameraTransform = m_Origin.Camera.transform;
        }

        /// <summary>
        /// Click handler method for the "Create Manual Probe" button.
        /// </summary>
        public void OnClickPlaceProbeAtCamera() => PlaceProbeAtCamera();

        /// <summary>
        /// Click handler method for the "Delete most recent probe" button.
        /// </summary>
        public void OnClickDeleteMostRecentProbe() => TryDeleteMostRecentProbe();

        /// <summary>
        /// Click handler method for the "Delete closest probe to camera" button.
        /// </summary>
        public void OnClickDeleteClosetToCamera() => TryDeleteClosestProbe();

        AREnvironmentProbe InstantiateProbe(Pose pose)
        {
            var probe = Instantiate(m_ProbePrefab, pose.position, pose.rotation);
            probe.name = $"{probe.name}-{probe.trackableId.ToString()}";
            m_Probes.Add(probe);

            return probe;
        }

        /// <summary>
        /// Places a probe at the given orientation.
        /// </summary>
        /// <param name="pose"><see cref="Pose"/> at which to place the environment probe </param>
        /// <returns>The <see cref="AREnvironmentProbe"/> instantiated at <paramref name="pose"/>, or <see langword="null"/> if manual placement is unsupported.</returns>
        public AREnvironmentProbe PlaceProbeAt(Pose pose) => canManuallyPlace ? InstantiateProbe(pose) : null;

        /// <summary>
        /// Places a probe at the XR Origin Camera's pose.
        /// </summary>
        /// <returns>The <see cref="AREnvironmentProbe"/> instantiated, or <see langword="null"/> if manual placement is unsupported.</returns>
        public AREnvironmentProbe PlaceProbeAtCamera() =>
            canManuallyPlace ? InstantiateProbe(originCameraPose) : null;

        bool DeleteProbe(AREnvironmentProbe probe)
        {
            var probeIndex = m_Probes.IndexOf(probe);
            if (probeIndex == -1)
                return false;

            Destroy(probe.gameObject);
            m_Probes.RemoveAt(probeIndex);

            return true;
        }

        /// <summary>
        /// Attempts to delete the probe if manual removal is supported.
        /// </summary>
        /// <param name="probe">The <see cref="AREnvironmentProbe"/> to be removed.</param>
        /// <returns>Returns <see langword="true"/> if the operation was successful. Otherwise, returns <see langword="false"/>.</returns>
        public bool TryDeleteProbe(AREnvironmentProbe probe)
        {
            return canManuallyRemove && DeleteProbe(probe);
        }

        /// <summary>
        /// Attempts to delete the most recently created probe.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the operation was successful. Otherwise, returns <see langword="false"/>.</returns>
        public bool TryDeleteMostRecentProbe()
        {
            return m_Probes.Count > 0 && DeleteProbe(m_Probes[^1]);
        }

        /// <summary>
        /// Attempts to delete the probe closest to the <see cref="XROrigin"/>'s <see cref="Camera"/>
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the operation was successful. Otherwise, returns <see langword="false"/>.</returns>
        public bool TryDeleteClosestProbe()
        {
            var probesCount = m_Probes.Count;
            if (probesCount == 0)
                return false;

            var camPose = originCameraPose;
            var camPosition = camPose.position;

            var closestSqrMagnitude = float.PositiveInfinity;
            var closestIndex = -1;

            for (var i = 0; i < m_Probes.Count; i++)
            {
                var probe = m_Probes[i];
                var probeTransform = probe.transform;
                var testSqrMagnitude = Vector3.SqrMagnitude(probeTransform.position - camPosition);

                if (testSqrMagnitude >= closestSqrMagnitude)
                    continue;

                closestIndex = i;
                closestSqrMagnitude = testSqrMagnitude;
            }

            Assert.AreNotEqual(closestIndex, -1);

            Destroy(m_Probes[closestIndex].gameObject);
            m_Probes.RemoveAt(closestIndex);

            return true;
        }
    }
}
