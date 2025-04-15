using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class SpawnObjectsInGrid : MonoBehaviour
    {
        [SerializeField]
        GameObject m_PrefabToSpawn;

        [SerializeField]
        float m_SpawnHeight = .4f;

        [Header("Grid settings")]
        [SerializeField]
        int m_GridRows = 10;

        [SerializeField]
        int m_GridColumns = 10;

        [SerializeField]
        int m_Layers = 1;

        [SerializeField]
        float m_Scale = .2f;

        [SerializeField]
        [Tooltip("distance between bounds of objects, not their centers")]
        float m_GridPadding = .4f;

        [SerializeField]
        float m_DistanceToClosestObject = .66f;

        [SerializeField, HideInInspector]
        List<GameObject> m_SpawnedCopies = new();

        XRInputSubsystem m_XRInputSubsystem;
        XROrigin m_XROrigin;

        void OnEnable()
        {
            if (m_PrefabToSpawn == null)
            {
                Debug.LogError($"{nameof(SpawnObjectsInGrid)} component needs to be set up in Inspector. Disabling.", this);
                enabled = false;
            }
        }

        void Awake()
        {
            m_XROrigin = GetXROrigin();

            if (m_XROrigin == null)
            {
                Debug.LogError("Can't find XROrigin.");
                enabled = false;
                return;
            }

            SubsystemsUtility.TryGetLoadedSubsystem<XRInputSubsystem, XRInputSubsystem>(out m_XRInputSubsystem);

            if (m_XRInputSubsystem == null)
            {
                Debug.LogError("Can't get input subsystem.");
                enabled = false;
                return;
            }

            m_XRInputSubsystem.trackingOriginUpdated += OnTrackingOriginUpdated;
        }

        void OnDestroy()
        {
            m_XRInputSubsystem.trackingOriginUpdated -= OnTrackingOriginUpdated;
        }

        void Start()
        {
            SpawnGrid();
            AwaitForCameraPossession(FilterNearbyObjects);
        }

        XROrigin GetXROrigin()
        {
            return GetComponent<XROrigin>() ?? FindAnyObjectByType<XROrigin>();
        }

        [ContextMenu("Spawn Grid")]
        void SpawnGridEditor()
        {
            m_XROrigin = GetXROrigin();
            SpawnGrid();
            FilterNearbyObjects();
        }

        [ContextMenu("Delete Grid")]
        void DeleteGrid()
        {
            foreach (var g in m_SpawnedCopies)
            {
                UnityObjectUtils.Destroy(g);
            }

            m_SpawnedCopies.Clear();
        }

        static float ComputeStartOffset(int numElements, float size, float padding)
        {
            return (size * numElements + (numElements - 1) * padding - size) * 0.5f;
        }

        void SpawnGrid()
        {
            if (m_SpawnedCopies.Count > 0)
            {
                Debug.LogWarning("Ignoring request to spawn grid because objects are already spawned", this);
                return;
            }

            m_PrefabToSpawn.transform.localScale = Vector3.one * m_Scale;
            var prefabMesh = m_PrefabToSpawn.GetComponent<MeshFilter>();
            var bounds = prefabMesh == null ? new Bounds(Vector3.zero, Vector3.zero) : prefabMesh.sharedMesh.bounds;
            var size = bounds.size * m_Scale;
            var originPos = m_XROrigin.transform.position;
            var startPos = new Vector3(
                originPos.x - ComputeStartOffset(m_GridColumns, size.x, m_GridPadding),
                m_SpawnHeight,
                originPos.z - ComputeStartOffset(m_GridRows, size.z, m_GridPadding));

            for (int layer = 0; layer < m_Layers; ++layer)
            {
                for (var row = 0; row < m_GridRows; ++row)
                {
                    for (var col = 0; col < m_GridColumns; ++col)
                    {
                        var spawnPos = new Vector3(
                            startPos.x + col * (m_GridPadding + size.x),
                            startPos.y + layer * (m_GridPadding + size.y),
                            startPos.z + row * (m_GridPadding + size.z));

                        var g = Instantiate(m_PrefabToSpawn, spawnPos, Quaternion.identity, transform);
                        m_SpawnedCopies.Add(g);
                    }
                }
            }
        }

        void OnTrackingOriginUpdated(XRInputSubsystem subsystem)
        {
            FilterNearbyObjects();
        }

        async void AwaitForCameraPossession(Action callback)
        {
            float epsilon = 1e-5f;

            while ((m_XROrigin.Camera.transform.position - Vector3.zero).sqrMagnitude < epsilon * epsilon)
            {
                await Awaitable.NextFrameAsync();
            }

            callback?.Invoke();
        }

        void FilterNearbyObjects()
        {
            if (m_SpawnedCopies == null)
            {
                Debug.LogError("Objects collection is not provided");
                return;
            }

            if (m_XROrigin == null)
            {
                Debug.LogError("XROrigin is not provided.");
                return;
            }

            var sqrDistThreshold = m_DistanceToClosestObject * m_DistanceToClosestObject;

            foreach (var obj in m_SpawnedCopies)
            {
                var objPosRelToOrig = m_XROrigin.transform.InverseTransformPoint(obj.transform.transform.position);
                var isFarEnough = (m_XROrigin.CameraInOriginSpacePos - objPosRelToOrig).sqrMagnitude > sqrDistThreshold;
                obj.SetActive(isFarEnough);
            }
        }
    }
}
