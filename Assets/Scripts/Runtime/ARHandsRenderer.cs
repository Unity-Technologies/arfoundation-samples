using Unity.Collections;
using UnityEngine.XR.ARFoundation.Samples.Runtime;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Meshing;

namespace UnityEngine.XR.ARFoundation.Samples.Hands
{
    public class ARHandsRenderer : MonoBehaviour
    {
        [SerializeField]
        Material m_HandsMaterial;

        [SerializeField]
        ARShaderOcclusion m_ARShaderOcclusion;

        IARHandView m_LeftHand;
        IARHandView m_RightHand;

        XRHandSubsystem m_HandsSubsystem;

        async void Awake()
        {
            SubsystemsUtility.TryGetLoadedSubsystem<XRSessionSubsystem, XRSessionSubsystem>(out var sessionSubsystem);

            if (sessionSubsystem == null)
            {
                Debug.LogError("Can't get XRSessionSubsystem. Hand views won't be created.");
                return;
            }

            var handViewFactory = GetHandsFactory(sessionSubsystem);

            if (handViewFactory == null)
            {
                return;
            }

            m_LeftHand = handViewFactory.CreateHand("LeftHand", m_HandsMaterial, m_ARShaderOcclusion);
            m_RightHand = handViewFactory.CreateHand("RightHand", m_HandsMaterial, m_ARShaderOcclusion);

            m_HandsSubsystem = await SubsystemsUtility.GetRunningSubsystem<XRHandSubsystem, XRHandSubsystem>();

            if (m_HandsSubsystem != null)
            {
                m_HandsSubsystem.updatedHands += OnHandsUpdated;
            }
        }

        void OnDestroy()
        {
            if (m_HandsSubsystem != null)
            {
                m_HandsSubsystem.updatedHands -= OnHandsUpdated;
            }

            m_LeftHand?.Dispose();
            m_RightHand?.Dispose();
        }

        static IARHandViewFactory GetHandsFactory(XRSessionSubsystem sessionSubsystem)
        {
            switch (sessionSubsystem.subsystemDescriptor.id)
            {
                case "Android-Session":
                    return new AndroidARHandViewFactory();
                default:
                    Debug.LogError($"Hands renderer is not yet implemented for {sessionSubsystem.subsystemDescriptor.id}.");
                    return null;
            }
        }

        static void UpdateHand(XRHandSubsystem.UpdateSuccessFlags successFlags, XRHandSubsystem.UpdateSuccessFlags mask,
            IARHandView handView, XRHandMeshData handMeshData)
        {
            if ((successFlags & mask) != 0)
            {
                handView.meshRenderer.enabled = true;
                handView.Update(handMeshData);
            }
            else
            {
                handView.meshRenderer.enabled = false;
            }
        }

        void OnHandsUpdated(
            XRHandSubsystem subsystem,
            XRHandSubsystem.UpdateSuccessFlags successFlags,
            XRHandSubsystem.UpdateType type)
        {
            var queryParams = new XRHandMeshDataQueryParams
            {
                allocator = Allocator.Temp
            };

            if (!subsystem.TryGetMeshData(out var meshDataQueryResult, ref queryParams))
            {
                return;
            }

            using (meshDataQueryResult)
            {
                var leftHandSuccessMask = XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints |
                                           XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose;

                UpdateHand(successFlags, leftHandSuccessMask, m_LeftHand, meshDataQueryResult.leftHand);

                var rightHandSuccessMask = XRHandSubsystem.UpdateSuccessFlags.RightHandJoints |
                                            XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose;

                UpdateHand(successFlags, rightHandSuccessMask, m_RightHand, meshDataQueryResult.rightHand);
            }
        }
    }
}
