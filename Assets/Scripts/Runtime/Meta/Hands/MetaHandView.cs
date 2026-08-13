using System;
using Unity.Collections;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Meshing;

namespace UnityEngine.XR.ARFoundation.Samples.Hands
{
    public class MetaHandView : IARHandView
    {
#if XR_HANDS_1_9_0_1_OR_NEWER
        SkinnedMeshRenderer m_SkinnedMeshRenderer;
        Transform m_XROriginTransform;
        Material m_PrevMaterial;
        AROcclusionSources m_OcclusionSources;
        ARShaderOcclusion m_ShaderOcclusion;

        Transform[] m_BoneTransforms;
        bool m_MeshInitialized;
        Handedness m_Handedness;

        public Renderer renderer => m_SkinnedMeshRenderer;
        public bool isInitialized => m_MeshInitialized;

        public MetaHandView(
            SkinnedMeshRenderer skinnedMeshRenderer,
            Material material,
            Transform xrOriginTransform,
            ARShaderOcclusion shaderOcclusion = null)
        {
            m_SkinnedMeshRenderer = skinnedMeshRenderer;
            m_SkinnedMeshRenderer.material = material;
            m_XROriginTransform = xrOriginTransform;
            m_ShaderOcclusion = shaderOcclusion;

            if (shaderOcclusion != null)
                shaderOcclusion.occlusionSourceSet += OnOcclusionSourceSet;
        }
#else
        public Renderer renderer => null;
        public bool isInitialized => false;
#endif

        void IDisposable.Dispose()
        {
#if XR_HANDS_1_9_0_1_OR_NEWER
            if (m_ShaderOcclusion != null)
                m_ShaderOcclusion.occlusionSourceSet -= OnOcclusionSourceSet;

            if (m_SkinnedMeshRenderer != null)
            {
                var mesh = m_SkinnedMeshRenderer.sharedMesh;
                if (mesh != null)
                    Object.Destroy(mesh);

                Object.Destroy(m_SkinnedMeshRenderer.gameObject);
            }
#endif
        }

        void IARHandView.Update(XRHandSubsystem subsystem, in XRHandMeshData meshData)
        {
#if XR_HANDS_1_9_0_1_OR_NEWER
            if (!m_MeshInitialized)
            {
                if (meshData.positions.Length == 0 || meshData.indices.Length == 0)
                    return;

                InitializeMesh(meshData);
            }

            UpdateBoneTransforms(subsystem);
#endif
        }

        void IARHandView.UpdatePoses(XRHandSubsystem subsystem)
        {
#if XR_HANDS_1_9_0_1_OR_NEWER
            if (m_MeshInitialized)
                UpdateBoneTransforms(subsystem);
#endif
        }

#if XR_HANDS_1_9_0_1_OR_NEWER
        void InitializeMesh(in XRHandMeshData meshData)
        {
            var mesh = m_SkinnedMeshRenderer.sharedMesh;

            // Set static geometry
            mesh.SetVertices(meshData.positions);
            mesh.SetIndices(meshData.indices, MeshTopology.Triangles, 0);

            if (meshData.normals.IsCreated && meshData.normals.Length == meshData.positions.Length)
                mesh.SetNormals(meshData.normals);
            else
                mesh.RecalculateNormals();

            if (meshData.uvs.IsCreated && meshData.uvs.Length == meshData.positions.Length)
                mesh.SetUVs(0, meshData.uvs);

            mesh.RecalculateBounds();

            int jointCount = XRHandJointID.EndMarker.ToIndex();

            // Bone weights — indices are already remapped to full joint-index space by MetaOpenXRHandMeshData
            if (meshData.bonesPerVertex.IsCreated && meshData.boneWeights.IsCreated)
                mesh.SetBoneWeights(meshData.bonesPerVertex, meshData.boneWeights);

            // Set bind poses (Unity expects inverse-bind-pose matrices)
            var rawBindPoses = meshData.jointBindPoseMatricesRaw;
            if (rawBindPoses.IsCreated)
            {
                var bindPoses = new Matrix4x4[jointCount];
                for (int i = 0; i < jointCount; i++)
                {
                    if (i < rawBindPoses.Length)
                        bindPoses[i] = rawBindPoses[i].inverse;
                    else
                        bindPoses[i] = Matrix4x4.identity;
                }

                mesh.bindposes = bindPoses;
            }

            // Create bone transforms
            var rootTransform = m_SkinnedMeshRenderer.transform;
            m_BoneTransforms = new Transform[jointCount];
            for (int i = 0; i < jointCount; i++)
            {
                var jointId = XRHandJointIDUtility.FromIndex(i);
                var boneObj = new GameObject(jointId.ToString());
                boneObj.transform.SetParent(rootTransform, false);
                boneObj.transform.localScale = Vector3.one;
                m_BoneTransforms[i] = boneObj.transform;
            }

            m_SkinnedMeshRenderer.bones = m_BoneTransforms;
            m_Handedness = meshData.handedness;
            m_MeshInitialized = true;
        }

        void UpdateBoneTransforms(XRHandSubsystem subsystem)
        {
            var hand = m_Handedness == Handedness.Left ? subsystem.leftHand : subsystem.rightHand;
            var rootPose = hand.rootPose;
            var rootTransform = m_SkinnedMeshRenderer.transform;

            // Cache origin transform properties to avoid redundant native calls per joint
            var originRotation = m_XROriginTransform.rotation;
            var originMatrix = m_XROriginTransform.localToWorldMatrix;

            // Joint poses are in XR Origin space — transform to world space
            rootTransform.SetPositionAndRotation(
                originMatrix.MultiplyPoint3x4(rootPose.position),
                originRotation * rootPose.rotation);

            int jointCount = XRHandJointID.EndMarker.ToIndex();
            for (int i = 0; i < jointCount; i++)
            {
                var jointId = XRHandJointIDUtility.FromIndex(i);
                var joint = hand.GetJoint(jointId);

                if (joint.TryGetPose(out var jointPose))
                {
                    m_BoneTransforms[i].SetPositionAndRotation(
                        originMatrix.MultiplyPoint3x4(jointPose.position),
                        originRotation * jointPose.rotation);
                }
            }
        }

        void OnOcclusionSourceSet(object sender, AROcclusionSourceEventArgs args)
        {
            SetMaterial(args.handsMaterial, args.occlusionSources);
        }

        void SetMaterial(Material material, AROcclusionSources sources)
        {
            if (material == null)
                return;

            m_OcclusionSources = sources;

            if ((sources & AROcclusionSources.HandMesh) == 0)
            {
                if (m_PrevMaterial != null)
                    m_SkinnedMeshRenderer.material = m_PrevMaterial;
                return;
            }

            m_PrevMaterial = m_SkinnedMeshRenderer.material;
            m_SkinnedMeshRenderer.material = material;
        }
#endif
    }
}
