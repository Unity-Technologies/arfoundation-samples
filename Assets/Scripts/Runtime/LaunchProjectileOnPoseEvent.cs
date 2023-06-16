using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Subscribes to a <see cref="PoseEventAsset"/>. When the event is raised, instantiates and launches
    /// a projectile based on <see cref="Pose"/> data.
    /// </summary>
    public class LaunchProjectileOnPoseEvent : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The PoseEvent containing the position and rotation from which to launch the projectile.")]
        PoseEventAsset m_PoseEvent;

        [SerializeField]
        [Tooltip("The projectile prefab to be instantiated.")]
        GameObject m_ProjectilePrefab;

        [SerializeField]
        [Tooltip("The speed at which the projectile will be launched.")]
        [Range(.01f, 999)]
        float m_LaunchSpeed = 25;

        /// <summary>
        /// This class fires a projectile when this event is raised.
        /// </summary>
        public PoseEventAsset poseEvent
        {
            get => m_PoseEvent;
            set => m_PoseEvent = value;
        }

        /// <summary>
        /// The GameObject to instantiate and launch when <see cref="poseEvent"/> is raised.
        /// </summary>
        public GameObject projectilePrefab
        {
            get => m_ProjectilePrefab;
            set => m_ProjectilePrefab = value;
        }

        /// <summary>
        /// The speed at which the projectile will be launched.
        /// </summary>
        public float launchSpeed
        {
            get => m_LaunchSpeed;
            set => m_LaunchSpeed = value;
        }

        void OnEnable()
        {
            if (m_ProjectilePrefab == null || m_PoseEvent == null)
            {
                Debug.LogWarning(
                    $"{nameof(LaunchProjectileOnPoseEvent)} component on {name} has null inputs and will have no effect in this scene.", this);

                return;
            }

            if (m_PoseEvent != null)
                m_PoseEvent.eventRaised += LaunchFromPose;
        }

        void OnDisable()
        {
            if (m_PoseEvent != null)
                m_PoseEvent.eventRaised -= LaunchFromPose;
        }

        void LaunchFromPose(object sender, Pose pose)
        {
            var projectile = Instantiate(m_ProjectilePrefab, pose.position, pose.rotation);
            var projectileRigidbody = ComponentUtils.GetOrAddIf<Rigidbody>(projectile, true);
            projectileRigidbody.velocity = pose.forward * m_LaunchSpeed;
        }
    }
}
