using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif // UNITY_IOS && !UNITY_EDITOR

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ToggleMeshClassification : MonoBehaviour
    {
        /// <summary>
        /// The mesh manager for the scene.
        /// </summary>
        [SerializeField]
        ARMeshManager m_MeshManager;

        /// <summary>
        /// Whether mesh classification should be enabled.
        /// </summary>
        [SerializeField]
        bool m_ClassificationEnabled = false;

        /// <summary>
        /// The mesh manager for the scene.
        /// </summary>
        public ARMeshManager meshManager { get => m_MeshManager; set => m_MeshManager = value; }

        /// <summary>
        /// Whether mesh classification should be enabled.
        /// </summary>
        public bool classificationEnabled
        {
            get => m_ClassificationEnabled;
            set
            {
                m_ClassificationEnabled = value;
                UpdateMeshSubsystem();
            }
        }

        /// <summary>
        /// On enable, update the mesh subsystem with the classification enabled setting.
        /// </summary>
        void OnEnable()
        {
            UpdateMeshSubsystem();
        }

        /// <summary>
        /// Update the mesh subsystem with the classiication enabled setting.
        /// </summary>
        void UpdateMeshSubsystem()
        {
    #if UNITY_IOS && !UNITY_EDITOR
            Debug.Assert(m_MeshManager != null, "mesh manager cannot be null");
            if ((m_MeshManager != null) && (m_MeshManager.subsystem is XRMeshSubsystem meshSubsystem))
            {
                meshSubsystem.SetClassificationEnabled(m_ClassificationEnabled);
            }
    #endif // UNITY_IOS && !UNITY_EDITOR
        }
    }
}