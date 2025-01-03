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
        float m_GridPadding = .6f;

        [SerializeField]
        float m_DistanceToClosestObject = .66f;

        [SerializeField, HideInInspector]
        List<GameObject> m_SpawnedCopies = new();

        void OnEnable()
        {
            if (m_PrefabToSpawn == null)
            {
                Debug.LogError($"{nameof(SpawnObjectsInGrid)} component needs to be set up in Inspector. Disabling.", this);
                enabled = false;
            }
        }

        void Start()
        {
            if (m_SpawnedCopies.Count == 0)
                SpawnGrid();
        }

        [ContextMenu("Spawn Grid")]
        void SpawnGrid()
        {
            if (m_SpawnedCopies.Count > 0)
            {
                Debug.LogWarning("Ignoring request to spawn grid because objects are already spawned", this);
                return;
            }

            var pos = transform.position;
            var halfRows = m_GridRows / 2;
            var halfCols = m_GridColumns / 2;
            for (var i = 0; i < m_GridRows; ++i)
            {
                var rowMult = i - halfRows;
                for (var j = 0; j < m_GridColumns; ++j)
                {
                    var colMult = j - halfCols;
                    var spawnPos = new Vector3(
                        pos.x + rowMult * m_GridPadding,
                        m_SpawnHeight,
                        pos.z + colMult * m_GridPadding);

                    if (Vector2.Distance(new Vector2(spawnPos.x, spawnPos.z), new Vector2(pos.x, pos.z)) < m_DistanceToClosestObject)
                        continue;

                    var g = Instantiate(m_PrefabToSpawn, spawnPos, Quaternion.identity, transform);
                    m_SpawnedCopies.Add(g);
                }
            }
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
    }
}
