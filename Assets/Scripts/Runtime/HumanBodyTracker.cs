using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class HumanBodyTracker : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The Skeleton prefab to be controlled.")]
        GameObject m_SkeletonPrefab;

        [SerializeField]
        [Tooltip("The ARHumanBodyManager which will produce body tracking events.")]
        ARHumanBodyManager m_HumanBodyManager;

        /// <summary>
        /// Get/Set the <c>ARHumanBodyManager</c>.
        /// </summary>
        public ARHumanBodyManager humanBodyManager
        {
            get { return m_HumanBodyManager; }
            set { m_HumanBodyManager = value; }
        }

        /// <summary>
        /// Get/Set the skeleton prefab.
        /// </summary>
        public GameObject skeletonPrefab
        {
            get { return m_SkeletonPrefab; }
            set { m_SkeletonPrefab = value; }
        }

        Dictionary<TrackableId, BoneController> m_SkeletonTracker = new Dictionary<TrackableId, BoneController>();

        void OnEnable()
        {
            Debug.Assert(m_HumanBodyManager != null, "Human body manager is required.");
            m_HumanBodyManager.trackablesChanged.AddListener(OnHumanBodiesChanged);
        }

        void OnDisable()
        {
            if (m_HumanBodyManager != null)
                m_HumanBodyManager.trackablesChanged.RemoveListener(OnHumanBodiesChanged);
        }

        void OnHumanBodiesChanged(ARTrackablesChangedEventArgs<ARHumanBody> eventArgs)
        {
            BoneController boneController;

            foreach (var humanBody in eventArgs.added)
            {
                if (!m_SkeletonTracker.TryGetValue(humanBody.trackableId, out boneController))
                {
                    Debug.Log($"Adding a new skeleton [{humanBody.trackableId}].");
                    var newSkeletonGO = Instantiate(m_SkeletonPrefab, humanBody.transform);
                    boneController = newSkeletonGO.GetComponent<BoneController>();
                    m_SkeletonTracker.Add(humanBody.trackableId, boneController);
                }

                boneController.InitializeSkeletonJoints();
                boneController.ApplyBodyPose(humanBody);
            }

            foreach (var humanBody in eventArgs.updated)
            {
                if (m_SkeletonTracker.TryGetValue(humanBody.trackableId, out boneController))
                {
                    boneController.ApplyBodyPose(humanBody);
                }
            }

            foreach (var (trackableId, _) in eventArgs.removed)
            {
                Debug.Log($"Removing a skeleton [{trackableId}].");
                if (m_SkeletonTracker.TryGetValue(trackableId, out boneController))
                {
                    Destroy(boneController.gameObject);
                    m_SkeletonTracker.Remove(trackableId);
                }
            }
        }
    }
}
