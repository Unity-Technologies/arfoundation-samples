using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if !UNITY_6000_4_OR_NEWER && UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

using LegacyMeshId = UnityEngine.XR.MeshId;
using Object = UnityEngine.Object;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MeshClassificationFracking : MonoBehaviour
    {
        /// <summary>
        /// The number of mesh classifications detected.
        /// </summary>
        const int k_NumClassifications = 9;

#if !UNITY_6000_4_OR_NEWER && UNITY_IOS
        /// <summary>
        /// Whether mesh classification should be enabled.
        /// </summary>
        [SerializeField]
        bool m_ClassificationEnabled = false;
#endif

        /// <summary>
        /// The mesh manager for the scene.
        /// </summary>
        public ARMeshManager m_MeshManager;

        /// <summary>
        /// The mesh prefab for the None classification.
        /// </summary>
        public MeshFilter m_NoneMeshPrefab;

        /// <summary>
        /// The mesh prefab for the Other classification.
        /// </summary>
        public MeshFilter m_OtherMeshPrefab;

        /// <summary>
        /// The mesh prefab for the Wall classification.
        /// </summary>
        public MeshFilter m_WallMeshPrefab;

        /// <summary>
        /// The mesh prefab for the Floor classification.
        /// </summary>
        public MeshFilter m_FloorMeshPrefab;

        /// <summary>
        /// The mesh prefab for the Ceiling classification.
        /// </summary>
        public MeshFilter m_CeilingMeshPrefab;

        /// <summary>
        /// The mesh prefab for the Table classification.
        /// </summary>
        public MeshFilter m_TableMeshPrefab;

        /// <summary>
        /// The mesh prefab for the Seat classification.
        /// </summary>
        public MeshFilter m_SeatMeshPrefab;

        /// <summary>
        /// The mesh prefab for the Window classification.
        /// </summary>
        public MeshFilter m_WindowMeshPrefab;

        /// <summary>
        /// The mesh prefab for the Door classification.
        /// </summary>
        public MeshFilter m_DoorMeshPrefab;

#if !UNITY_6000_4_OR_NEWER && UNITY_IOS
        /// <summary>
        /// Update the mesh subsystem with the classiication enabled setting.
        /// </summary>
        void UpdateMeshSubsystem()
        {
            Debug.Assert(m_MeshManager != null, "mesh manager cannot be null");
            if ((m_MeshManager != null) && (m_MeshManager.subsystem is XRMeshSubsystem meshSubsystem))
            {
                meshSubsystem.SetClassificationEnabled(m_ClassificationEnabled);
            }
        }
#endif

#if !UNITY_EDITOR

        /// <summary>
        /// A mapping from tracking ID to instantiated mesh filters.
        /// </summary>
        readonly Dictionary<TrackableId, MeshFilter[]> m_MeshTrackingMap = new Dictionary<TrackableId, MeshFilter[]>();

        /// <summary>
        /// The delegate to call to breakup a mesh.
        /// </summary>
        Action<MeshUpdateInfo> m_BreakupMeshAction;

        /// <summary>
        /// The delegate to call to update a mesh.
        /// </summary>
        Action<MeshUpdateInfo> m_UpdateMeshAction;

        /// <summary>
        /// The delegate to call to remove a mesh.
        /// </summary>
        Action<MeshUpdateInfo> m_RemoveMeshAction;

        /// <summary>
        /// An array to store the triangle vertices of the base mesh.
        /// </summary>
        readonly List<int> m_BaseTriangles = new List<int>();

        /// <summary>
        /// An array to store the triangle vertices of the classified mesh.
        /// </summary>
        readonly List<int> m_ClassifiedTriangles = new List<int>();

        /// <summary>
        /// On awake, set up the mesh filter delegates.
        /// </summary>
        void Awake()
        {
            m_BreakupMeshAction = BreakupMesh;
            m_UpdateMeshAction = UpdateMesh;
            m_RemoveMeshAction = RemoveMesh;
        }

        /// <summary>
        /// On enable, subscribe to the mesh info changed event.
        /// </summary>
        void OnEnable()
        {
            Debug.Assert(m_MeshManager != null, "mesh manager cannot be null");
            m_MeshManager.meshInfosChanged.AddListener(OnMeshInfosChanged);
#if !UNITY_6000_4_OR_NEWER && UNITY_IOS
            UpdateMeshSubsystem();
#endif
        }

        /// <summary>
        /// On disable, unsubscribe from the mesh info changed event.
        /// </summary>
        void OnDisable()
        {
            Debug.Assert(m_MeshManager != null, "mesh manager cannot be null");
            m_MeshManager.meshInfosChanged.RemoveListener(OnMeshInfosChanged);
        }

        /// <summary>
        /// Handles mesh info changes by breaking up, updating, or removing submeshes.
        /// </summary>
        /// <param name="args">Mesh info change event arguments.</param>
        void OnMeshInfosChanged(ARMeshInfosChangedEventArgs args)
        {
            if (args.added != null)
            {
                foreach (var meshUpdateInfo in args.added)
                    m_BreakupMeshAction(meshUpdateInfo);
            }

            if (args.updated != null)
            {
                foreach (var meshUpdateInfo in args.updated)
                    m_UpdateMeshAction(meshUpdateInfo);
            }

            if (args.removed != null)
            {
                foreach (var meshUpdateInfo in args.removed)
                    m_RemoveMeshAction(meshUpdateInfo);
            }
        }

        /// <summary>
        /// Parse the trackable ID from the mesh filter name.
        /// </summary>
        /// <param name="meshFilterName">The mesh filter name containing the trackable ID.</param>
        /// <returns>
        /// The trackable ID parsed from the string.
        /// </returns>
        TrackableId ExtractTrackableId(string meshFilterName)
        {
            string[] nameSplit = meshFilterName.Split(' ');
            return new TrackableId(nameSplit[1]);
        }

        /// <summary>
        /// Given a base mesh, the face classifications for all faces in the mesh, and a single face classification to
        /// extract, extract into a new mesh only the faces that have the selected face classification.
        ///
        /// This method currently makes the assumption that the classification data and the face
        /// vertices are arranged in the same order.
        /// </summary>
        /// <param name="baseMesh">The original base mesh.</param>
        /// <param name="faceClassifications">The array of face classifications for each triangle in the
        /// <paramref name="baseMesh"/></param>
        /// <param name="selectedMeshClassification">A single classification to extract the faces from the
        /// <paramref="baseMesh"/>into the <paramref name="classifiedMesh"/></param>
        /// <param name="classifiedMesh">The output mesh to be updated with the extracted mesh.</param>
        void ExtractClassifiedMesh(Mesh baseMesh, NativeArray<XRMeshClassification> faceClassifications, XRMeshClassification selectedMeshClassification, Mesh classifiedMesh)
        {
            // Count the number of faces matching the selected classification.
            int classifiedFaceCount = 0;
            for (int i = 0; i < faceClassifications.Length; ++i)
            {
                if (faceClassifications[i] == selectedMeshClassification)
                {
                    ++classifiedFaceCount;
                }
            }

            // Clear the existing mesh.
            classifiedMesh.Clear();

            // If there were matching face classifications, build a new mesh from the base mesh.
            if (classifiedFaceCount > 0)
            {
                baseMesh.GetTriangles(m_BaseTriangles, 0);
                Debug.Assert(m_BaseTriangles.Count == (faceClassifications.Length * 3),
                            $"unexpected mismatch between triangle count ({m_BaseTriangles.Count}) and face classification count ({faceClassifications.Length * 3})");

                m_ClassifiedTriangles.Clear();
                m_ClassifiedTriangles.Capacity = classifiedFaceCount * 3;

                for (int i = 0; i < faceClassifications.Length; ++i)
                {
                    if (faceClassifications[i] == selectedMeshClassification)
                    {
                        int baseTriangleIndex = i * 3;

                        m_ClassifiedTriangles.Add(m_BaseTriangles[baseTriangleIndex + 0]);
                        m_ClassifiedTriangles.Add(m_BaseTriangles[baseTriangleIndex + 1]);
                        m_ClassifiedTriangles.Add(m_BaseTriangles[baseTriangleIndex + 2]);
                    }
                }

                classifiedMesh.vertices = baseMesh.vertices;
                classifiedMesh.normals = baseMesh.normals;
                classifiedMesh.SetTriangles(m_ClassifiedTriangles, 0);
            }
        }

        NativeArray<XRMeshClassification> CopyConvertFaceClassifications(NativeArray<uint> classifications)
        {
            var enumClassifications = new NativeArray<XRMeshClassification>(classifications.Length, Allocator.Temp);
            enumClassifications.CopyFrom(classifications.Reinterpret<XRMeshClassification>());
            return enumClassifications;
        }

        /// <summary>
        /// Convert and copy untyped per-vertex classifications into per-face ARFoundation
        /// classifications. "Fast," just takes the first vertex for the face and uses that value.
        /// Technically, a face could have up to 3 different classifications as it has 3 vertices.
        /// </summary>
        NativeArray<XRMeshClassification> CopyConvertVertexToFaceClassificationsFast(Mesh baseMesh, NativeArray<uint> classifications)
        {
            baseMesh.GetTriangles(m_BaseTriangles, 0);
            var classificationCount = m_BaseTriangles.Count / 3;
            var faceClassifications = new NativeArray<XRMeshClassification>(classificationCount, Allocator.Temp);
            for (int i = 0; i < classificationCount; i++)
            {
                // just use the first vertex of the triangle
                var vertexIndex = m_BaseTriangles[i * 3];
                Debug.Assert(
                    vertexIndex < classifications.Length,
                    $"vertexIndex {vertexIndex} is outside classifications indices (max {classifications.Length})"
                );
                faceClassifications[i] = (XRMeshClassification)classifications[vertexIndex];
            }
            return faceClassifications;
        }

        /// <summary>
        /// Break up a single mesh with multiple face classifications into submeshes, each with an unique and uniform mesh
        /// classification.
        /// </summary>
        /// <param name="meshUpdateInfo">Mesh update info containing the mesh filter and id.</param>
        void BreakupMesh(MeshUpdateInfo meshUpdateInfo)
        {
            XRMeshSubsystem meshSubsystem = m_MeshManager.subsystem as XRMeshSubsystem;
            if (meshSubsystem == null)
            {
                return;
            }

            var meshId = GetLegacyMeshId(meshUpdateInfo.id);
            var meshFilter = meshUpdateInfo.meshFilter;

            uint elementsPerVector = 0;
            NativeArray<uint> vertexIndexVectors = default;
            NativeArray<uint> classifications = default;
#if UNITY_6000_4_OR_NEWER
            if (!meshSubsystem.TryGetSubmeshClassifications(meshId, Allocator.Persistent, out elementsPerVector, out vertexIndexVectors, out classifications))
            {
                Debug.LogWarning("Failed to get mesh classifications.");
                return;
            }
#elif UNITY_IOS
            classifications = meshSubsystem.GetMeshClassifications(GetTrackableId(meshId), Allocator.Persistent);
#endif
            if (
                !classifications.IsCreated
                || classifications.Length <= 0
                || !(elementsPerVector == 0 || elementsPerVector == 1 || elementsPerVector == 3)
            )
            {
                return;
            }

            using (classifications)
            {
                var parent = meshFilter.transform.parent;
                MeshFilter[] meshFilters = new MeshFilter[k_NumClassifications];

                var pos = meshFilter.transform.localPosition;
                var rot = meshFilter.transform.localRotation;

                meshFilters[(int)XRMeshClassification.Unknown] =
                    (m_NoneMeshPrefab == null) ? null : Instantiate(m_NoneMeshPrefab, pos, rot, parent);
                meshFilters[(int)XRMeshClassification.Other] =
                    (m_OtherMeshPrefab == null) ? null : Instantiate(m_OtherMeshPrefab, pos, rot, parent);
                meshFilters[(int)XRMeshClassification.Floor] =
                    (m_FloorMeshPrefab == null) ? null : Instantiate(m_FloorMeshPrefab, pos, rot, parent);
                meshFilters[(int)XRMeshClassification.Ceiling] =
                    (m_CeilingMeshPrefab == null) ? null : Instantiate(m_CeilingMeshPrefab, pos, rot, parent);
                meshFilters[(int)XRMeshClassification.Wall] =
                    (m_WallMeshPrefab == null) ? null : Instantiate(m_WallMeshPrefab, pos, rot, parent);
                meshFilters[(int)XRMeshClassification.Table] =
                    (m_TableMeshPrefab == null) ? null : Instantiate(m_TableMeshPrefab, pos, rot, parent);
                meshFilters[(int)XRMeshClassification.Seat] =
                    (m_SeatMeshPrefab == null) ? null : Instantiate(m_SeatMeshPrefab, pos, rot, parent);
                meshFilters[(int)XRMeshClassification.Window] =
                    (m_WindowMeshPrefab == null) ? null : Instantiate(m_WindowMeshPrefab, pos, rot, parent);
                meshFilters[(int)XRMeshClassification.Door] =
                    (m_DoorMeshPrefab == null) ? null : Instantiate(m_DoorMeshPrefab, pos, rot, parent);

                m_MeshTrackingMap[meshUpdateInfo.id] = meshFilters;

                var baseMesh = meshFilter.sharedMesh;
                for (int i = 0; i < k_NumClassifications; ++i)
                {
                    var classifiedMeshFilter = meshFilters[i];
                    if (classifiedMeshFilter != null)
                    {
                        NativeArray<XRMeshClassification> faceClassifications = default;
                        if (elementsPerVector == 0 || elementsPerVector == 3)
                        {
                            faceClassifications = CopyConvertFaceClassifications(classifications);
                        }
                        if (elementsPerVector == 1)
                        {
                            faceClassifications = CopyConvertVertexToFaceClassificationsFast(
                                baseMesh,
                                classifications
                            );
                        }
                        if (!faceClassifications.IsCreated)
                        {
                            continue;
                        }

                        var classifiedMesh = classifiedMeshFilter.mesh;
                        try
                        {
                            ExtractClassifiedMesh(baseMesh, faceClassifications, (XRMeshClassification)i, classifiedMesh);
                            meshFilters[i].mesh = classifiedMesh;
                        }
                        finally
                        {
                            faceClassifications.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update the submeshes corresponding to the single mesh with multiple face classifications into submeshes.
        /// </summary>
        /// <param name="meshUpdateInfo">Mesh update info containing the mesh filter and id.</param>
        void UpdateMesh(MeshUpdateInfo meshUpdateInfo)
        {
            XRMeshSubsystem meshSubsystem = m_MeshManager.subsystem as XRMeshSubsystem;
            if (meshSubsystem == null)
            {
                Debug.LogWarning("MeshSubsystem is null.");
                return;
            }

            var meshId = GetLegacyMeshId(meshUpdateInfo.id);
            var meshFilter = meshUpdateInfo.meshFilter;

            uint elementsPerVector = 0;
            NativeArray<uint> vertexIndexVectors = default;
            NativeArray<uint> classifications = default;

#if UNITY_6000_4_OR_NEWER
            if (!meshSubsystem.TryGetSubmeshClassifications(meshId, Allocator.Persistent, out elementsPerVector, out vertexIndexVectors, out classifications))
            {
                Debug.LogWarning("Failed to get mesh classifications.");
                return;
            }
#elif UNITY_IOS
            classifications = meshSubsystem.GetMeshClassifications(GetTrackableId(meshId), Allocator.Persistent);
#endif
            if (
                !classifications.IsCreated
                || classifications.Length <= 0
                || !(elementsPerVector == 0 || elementsPerVector == 1 || elementsPerVector == 3)
            )
            {
                return;
            }

            using (classifications)
            {
                if (!m_MeshTrackingMap.TryGetValue(meshUpdateInfo.id, out var meshFilters))
                    return;

                var baseMesh = meshFilter.sharedMesh;
                for (int i = 0; i < k_NumClassifications; ++i)
                {
                    var classifiedMeshFilter = meshFilters[i];
                    if (classifiedMeshFilter != null)
                    {
                        classifiedMeshFilter.transform.localPosition = meshFilter.transform.localPosition;
                        classifiedMeshFilter.transform.localRotation = meshFilter.transform.localRotation;

                        NativeArray<XRMeshClassification> faceClassifications = default;
                        if (elementsPerVector == 0 || elementsPerVector == 3)
                        {
                            faceClassifications = CopyConvertFaceClassifications(classifications);
                        }
                        if (elementsPerVector == 1)
                        {
                            faceClassifications = CopyConvertVertexToFaceClassificationsFast(
                                baseMesh,
                                classifications
                            );
                        }
                        if (!faceClassifications.IsCreated)
                        {
                            continue;
                        }

                        var classifiedMesh = classifiedMeshFilter.mesh;
                        try
                        {
                            ExtractClassifiedMesh(baseMesh, faceClassifications, (XRMeshClassification)i, classifiedMesh);
                            meshFilters[i].mesh = classifiedMesh;
                        }
                        finally
                        {
                            faceClassifications.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove the submeshes corresponding to the single mesh.
        /// </summary>
        /// <param name="meshUpdateInfo">Mesh update info containing the mesh filter and id.</param>
        void RemoveMesh(MeshUpdateInfo meshUpdateInfo)
        {
            if (!m_MeshTrackingMap.TryGetValue(meshUpdateInfo.id, out var meshFilters))
                return; // Key not found, nothing to remove

            for (int i = 0; i < k_NumClassifications; ++i)
            {
                var classifiedMeshFilter = meshFilters[i];
                if (classifiedMeshFilter != null)
                {
                    Object.Destroy(classifiedMeshFilter);
                }
            }

            m_MeshTrackingMap.Remove(meshUpdateInfo.id);
        }

        /// <summary>
        /// Converts a TrackableId to a LegacyMeshId.
        /// </summary>
        /// <param name="trackableId">The TrackableId to convert.</param>
        /// <returns>The converted LegacyMeshId.</returns>
        static unsafe LegacyMeshId GetLegacyMeshId(TrackableId trackableId)
        {
            return *(LegacyMeshId*)&trackableId;
        }

        /// <summary>
        /// Converts a LegacyMeshId to a TrackableId.
        /// </summary>
        /// <param name="meshId">The LegacyMeshId to convert.</param>
        /// <returns>The converted TrackableId.</returns>
        static unsafe TrackableId GetTrackableId(LegacyMeshId meshId)
        {
            return *(TrackableId*)&meshId;
        }
#endif // !UNITY_EDITOR
    }
}
