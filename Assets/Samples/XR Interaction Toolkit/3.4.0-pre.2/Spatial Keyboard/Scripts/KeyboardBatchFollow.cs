namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// This component moves a set of transforms to the same local z-axis position as a poke follow transform.
    /// This is useful for batchable objects that need to move together.
    /// </summary>
    public class KeyboardBatchFollow : MonoBehaviour
    {
        [Tooltip("The transform to follow.")]
        [SerializeField]
        Transform m_FollowTransform;

        /// <summary>
        /// The transform to follow.
        /// </summary>
        public Transform followTransform
        {
            get => m_FollowTransform;
            set => m_FollowTransform = value;
        }

        [Tooltip("The transforms to move to the same local z-axis position as the poke follow transform.")]
        [SerializeField]
        Transform[] m_FollowerTransforms;

        /// <summary>
        /// The transforms to move to the same local z-axis position as the poke follow transform.
        /// </summary>
        public Transform[] followerTransforms
        {
            get => m_FollowerTransforms;
            set => m_FollowerTransforms = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            if (m_FollowerTransforms == null || m_FollowerTransforms.Length == 0)
                return;

            for (var index = 0; index < m_FollowerTransforms.Length; ++index)
            {
                var follower = m_FollowerTransforms[index];
                var localPosition = follower.localPosition;
                localPosition.z = 0f;
                follower.localPosition = localPosition;
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void LateUpdate()
        {
            if (m_FollowTransform == null || m_FollowerTransforms == null || m_FollowerTransforms.Length == 0)
                return;

            var followLocalZ = m_FollowTransform.localPosition.z;

            for (var index = 0; index < m_FollowerTransforms.Length; ++index)
            {
                var follower = m_FollowerTransforms[index];
                var localPosition = follower.localPosition;
                localPosition.z = followLocalZ;
                follower.localPosition = localPosition;
            }
        }
    }
}
