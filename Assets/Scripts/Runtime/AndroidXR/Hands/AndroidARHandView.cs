using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.Hands.Meshing;

namespace UnityEngine.XR.ARFoundation.Samples.Hands
{
    public class AndroidARHandView : IARHandView
    {
        Material m_PrevMaterial;
        AROcclusionSources m_OcclusionSources;

        ARShaderOcclusion m_ShaderOcclusion;

        public MeshFilter meshFilter { get; }
        public MeshRenderer meshRenderer { get; }

        public AndroidARHandView(MeshFilter meshFilter, MeshRenderer meshRenderer, Material material,
            ARShaderOcclusion shaderOcclusion = null)
        {
            if (meshFilter == null)
            {
                Debug.LogError("Missing MeshFilter component");
                return;
            }

            if (meshRenderer == null)
            {
                Debug.LogError("Missing MeshRenderer component");
                return;
            }

            this.meshFilter = meshFilter;
            this.meshRenderer = meshRenderer;
            this.meshRenderer.material = material;
            m_ShaderOcclusion = shaderOcclusion;

            if (shaderOcclusion != null)
            {
                shaderOcclusion.occlusionSourceSet += OnOcclusionSourceSet;
            }
        }

        void IDisposable.Dispose()
        {
            m_ShaderOcclusion.occlusionSourceSet -= OnOcclusionSourceSet;
        }

        void IARHandView.Update(in XRHandMeshData meshData)
        {
            UpdateVerticesAndIndices(meshData, meshFilter);

            if ((m_OcclusionSources & AROcclusionSources.HandMesh) != AROcclusionSources.HandMesh)
            {
                UpdateNormalsAndUVs(meshData, meshFilter);
            }
        }

        void UpdateVerticesAndIndices(in XRHandMeshData meshData, MeshFilter meshFilter)
        {
            if (meshData.TryGetRootPose(out var rootPose))
            {
                meshFilter.transform.position = rootPose.position;
                meshFilter.transform.rotation = rootPose.rotation;
            }

            if (meshData.positions.Length == 0 || meshData.indices.Length == 0)
            {
                Debug.LogWarning($"{meshFilter.name} MeshData contains invalid or empty positions or indices.");
                return;
            }

            if (!AreArraysEqual(meshData.positions, meshFilter.mesh.vertices))
            {
                meshFilter.mesh.SetVertices(meshData.positions);
                meshFilter.mesh.RecalculateBounds();
            }

            if (!AreArraysEqual(meshData.indices, meshFilter.mesh.triangles))
            {
                meshFilter.mesh.SetIndices(meshData.indices, MeshTopology.Triangles, 0);
            }
        }

        void UpdateNormalsAndUVs(in XRHandMeshData meshData, MeshFilter meshFilter)
        {
            var mesh = meshFilter.mesh;
            var vertexCount = mesh.vertexCount;

            if (meshData.normals.IsCreated && meshData.normals.Length == vertexCount)
            {
                mesh.SetNormals(meshData.normals);
            }
            else
            {
                mesh.RecalculateNormals();
            }

            if (meshData.uvs.IsCreated && meshData.uvs.Length == vertexCount)
            {
                mesh.SetUVs(0, meshData.uvs);
            }
            else
            {
                mesh.uv = null;
            }
        }


        static bool AreArraysEqual<T>(NativeArray<T> arrA, IList<T> arrB)
            where T : struct, IEquatable<T>
        {
            if (arrA.Length != arrB.Count)
            {
                return false;
            }

            for (int i = 0; i < arrA.Length; i++)
            {
                if (!arrA[i].Equals(arrB[i]))
                {
                    return false;
                }
            }

            return true;
        }


        void OnOcclusionSourceSet(object sender, AROcclusionSourceEventArgs args)
        {
            SetMaterial(args.handsMaterial, args.occlusionSources);
        }

        void SetMaterial(Material material, AROcclusionSources sources)
        {
            if (material == null)
            {
                return;
            }

            m_OcclusionSources = sources;

            if ((sources & AROcclusionSources.HandMesh) == 0)
            {
                if (m_PrevMaterial != null)
                {
                    meshRenderer.material = m_PrevMaterial;
                }

                return;
            }

            m_PrevMaterial = meshRenderer.material;
            meshRenderer.material = material;
        }
    }
}
