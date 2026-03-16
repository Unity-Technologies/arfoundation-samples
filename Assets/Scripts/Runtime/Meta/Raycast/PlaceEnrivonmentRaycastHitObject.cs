#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Subscribes to an <see cref="EnvironmentRaycastHitEventAsset"/>. When the event is raised, the
    /// <see cref="prefabToPlace"/> is instantiated at, or moved to, the hit position.
    /// </summary>
    public class PlaceEnvironmentRaycastHitObject : MonoBehaviour
    {
        const float k_PrefabHalfSize = 0.025f;

        [SerializeField]
        [Tooltip("The prefab to be placed or moved.")]
        GameObject m_PrefabToPlace;

        [SerializeField]
        [Tooltip("The Scriptable Object Asset that contains the ARRaycastHit event.")]
        EnvironmentRaycastHitEventAsset m_RaycastHitEvent;

        GameObject m_SpawnedObject;

        /// <summary>
        /// The prefab to be placed or moved.
        /// </summary>
        public GameObject prefabToPlace
        {
            get => m_PrefabToPlace;
            set => m_PrefabToPlace = value;
        }

        /// <summary>
        /// The spawned prefab instance.
        /// </summary>
        public GameObject spawnedObject
        {
            get => m_SpawnedObject;
            set => m_SpawnedObject = value;
        }

#if METAOPENXR_2_4_0_OR_NEWER && (UNITY_EDITOR || UNITY_ANDROID)
        void OnEnable()
        {
            if (m_RaycastHitEvent == null || m_PrefabToPlace == null)
            {
                Debug.LogWarning($"{nameof(ARPlaceObject)} component on {name} has null inputs and will have no effect in this scene.", this);
                return;
            }

            if (m_RaycastHitEvent != null)
                m_RaycastHitEvent.eventRaised += PlaceObjectAt;
        }

        void OnDisable()
        {
            if (m_RaycastHitEvent != null)
                m_RaycastHitEvent.eventRaised -= PlaceObjectAt;
        }

        void PlaceObjectAt(object sender, EnvironmentRaycastHit hit)
        {
            Transform objectParent = null;

            if (m_SpawnedObject == null)
            {
                m_SpawnedObject = Instantiate(m_PrefabToPlace, objectParent);
            }

            m_SpawnedObject.GetComponent<EnvironmentRaycastHitDebugVisualizer>()?.SetHitStatus(hit.hitStatus);
            var forward = hit.hit.pose.rotation * Vector3.up;
            var offset = forward * k_PrefabHalfSize;
            m_SpawnedObject.transform.position = hit.hit.pose.position + offset;
            m_SpawnedObject.transform.parent = objectParent;
        }
#endif
    }
}
