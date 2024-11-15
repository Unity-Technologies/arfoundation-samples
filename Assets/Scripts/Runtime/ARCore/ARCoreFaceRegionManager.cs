using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
#if UNITY_ANDROID && ARCORE_4_2_OR_NEWER && !UNITY_EDITOR
using UnityEngine.XR.ARCore;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// This component uses the ARCoreFaceSubsystem to query for face regions, special
    /// regions detected within a face, such as the nose tip. Each region has a pose
    /// associated with it. This component instantiates <see cref="regionPrefab"/>
    /// at each detected region.
    /// </summary>
    [RequireComponent(typeof(ARFaceManager))]
    [RequireComponent(typeof(XROrigin))]
    public class ARCoreFaceRegionManager : MonoBehaviour
    {
        [SerializeField]
        GameObject m_RegionPrefab;

        /// <summary>
        /// Get or set the prefab which will be instantiated at each detected face region.
        /// </summary>
        public GameObject regionPrefab
        {
            get { return m_RegionPrefab; }
            set { m_RegionPrefab = value; }
        }

        ARFaceManager m_FaceManager;

        XROrigin m_Origin;

#if UNITY_ANDROID && ARCORE_4_2_OR_NEWER && !UNITY_EDITOR
        NativeArray<ARCoreFaceRegionData> m_FaceRegions;

        Dictionary<TrackableId, Dictionary<ARCoreFaceRegion, GameObject>> m_InstantiatedPrefabs;
#endif

        // Start is called before the first frame update
        void Start()
        {
            m_FaceManager = GetComponent<ARFaceManager>();
            m_Origin = GetComponent<XROrigin>();
#if UNITY_ANDROID && ARCORE_4_2_OR_NEWER && !UNITY_EDITOR
            m_InstantiatedPrefabs = new Dictionary<TrackableId, Dictionary<ARCoreFaceRegion, GameObject>>();
#endif
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_ANDROID && ARCORE_4_2_OR_NEWER && !UNITY_EDITOR
            var subsystem = (ARCoreFaceSubsystem)m_FaceManager.subsystem;
            if (subsystem == null)
                return;

            foreach (var face in m_FaceManager.trackables)
            {
                Dictionary<ARCoreFaceRegion, GameObject> regionGos;
                if (!m_InstantiatedPrefabs.TryGetValue(face.trackableId, out regionGos))
                {
                    regionGos = new Dictionary<ARCoreFaceRegion, GameObject>();
                    m_InstantiatedPrefabs.Add(face.trackableId, regionGos);
                }

                subsystem.GetRegionPoses(face.trackableId, Allocator.Persistent, ref m_FaceRegions);
                for (int i = 0; i < m_FaceRegions.Length; ++i)
                {
                    var regionType = m_FaceRegions[i].region;

                    GameObject go;
                    if (!regionGos.TryGetValue(regionType, out go))
                    {
                        go = Instantiate(m_RegionPrefab, m_Origin.TrackablesParent);
                        regionGos.Add(regionType, go);
                    }

                    go.transform.localPosition = m_FaceRegions[i].pose.position;
                    go.transform.localRotation = m_FaceRegions[i].pose.rotation;
                }
            }
#endif
        }

        void OnDestroy()
        {
#if UNITY_ANDROID && ARCORE_4_2_OR_NEWER && !UNITY_EDITOR
            if (m_FaceRegions.IsCreated)
                m_FaceRegions.Dispose();
#endif
        }
    }
}
