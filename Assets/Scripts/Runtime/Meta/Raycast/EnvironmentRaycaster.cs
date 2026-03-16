using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using UnityEngine.SubsystemsImplementation.Extensions;

#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(XROrigin))]
    public sealed class EnvironmentRaycaster : MonoBehaviour
    {
        XROrigin m_Origin;
        ARRaycastManager m_Manager;

#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)

        MetaOpenXRRaycastSubsystem m_MetaSubsystem;

        private void Reset()
        {
            m_Manager = FindAnyObjectByType<ARRaycastManager>();
            m_Origin = FindAnyObjectByType<XROrigin>();
        }

        private void Start()
        {
            if (m_Origin == null)
                m_Origin = FindAnyObjectByType<XROrigin>();
            if (m_Manager ==  null)
                m_Manager = FindAnyObjectByType<ARRaycastManager>();
            if (m_Manager != null)
            {
                m_MetaSubsystem = m_Manager.subsystem as MetaOpenXRRaycastSubsystem;
            }
        }

        /// <summary>
        /// Cast a <c>Ray</c> against trackables, that is, detected features such as planes.
        /// </summary>
        /// <param name="ray">The <c>Ray</c>, in Unity world space, to cast.</param>
        /// <param name="hitResults">Contents are replaced with the raycast results, if successful.
        /// Results are sorted by distance in closest-first order.</param>
        /// <param name="trackableTypes">(Optional) The types of trackables to cast against.</param>
        /// <returns><see langword="true"/> if the raycast hit a trackable in the <paramref name="trackableTypes"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool Raycast(
            Ray ray,
            ref EnvironmentRaycastHit hitResult,
            float maxDistance = float.MaxValue)
        {
            if (hitResult == null)
                throw new ArgumentNullException(nameof(hitResult));

            var sessionSpaceRay = m_Origin.TrackablesParent.InverseTransformRay(ray);
            var hit = RaycastWorldSpace(sessionSpaceRay, maxDistance);

            return TransformAndDisposeNativeHitResult(hit,ref hitResult,ray.origin);
        }

        Result<EnvironmentRaycastHit> RaycastWorldSpace(Ray ray, float maxDistance = float.MaxValue)
        {
            bool doNativeRaycast =
                m_MetaSubsystem != null
                && m_MetaSubsystem.subsystemDescriptor.supportsWorldBasedRaycast;

            if (doNativeRaycast)
                return RaycastRay(ray, maxDistance);
            else
                return new Result<EnvironmentRaycastHit>(new XRResultStatus(XRResultStatus.StatusCode.Unsupported), EnvironmentRaycastHit.defaultValue);
        }

        Result<EnvironmentRaycastHit> RaycastRay(
            Ray ray,
            float maxDistance = float.MaxValue)
        {
            if (maxDistance == float.MaxValue)
                return m_MetaSubsystem.RaycastEnvironment(ray);
            else
                return m_MetaSubsystem.RaycastEnvironment(ray, maxDistance);
        }

        bool TransformAndDisposeNativeHitResult(
            Result<EnvironmentRaycastHit> nativeHit,
            ref EnvironmentRaycastHit managedHit,
            Vector3 rayOrigin)
        {
            // Result is in "trackables space", so transform result back into world space
            float distanceInWorldSpace = (nativeHit.value.hit.pose.position - rayOrigin).magnitude;
            managedHit = new EnvironmentRaycastHit(
                nativeHit.value.hitStatus,
                new XRRaycastHit(TrackableId.invalidId, nativeHit.value.hit.pose, distanceInWorldSpace, TrackableType.Depth));

            return nativeHit.status.IsSuccess();
        }
#endif
    }
}
