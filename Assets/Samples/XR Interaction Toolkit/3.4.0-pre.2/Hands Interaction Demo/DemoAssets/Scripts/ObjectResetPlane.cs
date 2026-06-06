using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Provides the ability to reset specified objects if they fall below a certain position - designated by this transform's height.
    /// </summary>
    public class ObjectResetPlane : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Which objects to reset if falling out of range.")]
        List<Transform> m_ObjectsToReset = new List<Transform>();

        [SerializeField]
        [Tooltip("How often to check if objects should be reset.")]
        float m_CheckDuration = 2f;

        [SerializeField]
        [Tooltip("The object root used to compute local positions relative to. Objects will respawn relative to their position in this transform's hierarchy.")]
        Transform m_ObjectRoot = null;

        readonly List<Pose> m_OriginalPositions = new List<Pose>();

        float m_CheckTimer;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Start()
        {
            foreach (var currentTransform in m_ObjectsToReset)
            {
                if (currentTransform != null)
                {
                    var position = currentTransform.position;

                    if (m_ObjectRoot != null)
                        position = m_ObjectRoot.InverseTransformPoint(currentTransform.position);

                    m_OriginalPositions.Add(new Pose(position, currentTransform.rotation));
                }
                else
                {
                    Debug.LogWarning("Objects To Reset contained a null element. Update the reference or delete the array element of the missing object.", this);
                    m_OriginalPositions.Add(new Pose());
                }
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            m_CheckTimer -= Time.deltaTime;

            if (m_CheckTimer > 0)
                return;

            m_CheckTimer = m_CheckDuration;

            var resetPlane = transform.position.y;

            for (var transformIndex = 0; transformIndex < m_ObjectsToReset.Count; transformIndex++)
            {
                var currentTransform = m_ObjectsToReset[transformIndex];
                if (currentTransform == null)
                    continue;

                if (currentTransform.position.y < resetPlane)
                {
                    var originalWorldPosition = m_OriginalPositions[transformIndex].position;
                    if (m_ObjectRoot != null)
                        originalWorldPosition = m_ObjectRoot.TransformPoint(originalWorldPosition);

                    currentTransform.SetPositionAndRotation(originalWorldPosition, m_OriginalPositions[transformIndex].rotation);

                    var rigidBody = currentTransform.GetComponentInChildren<Rigidbody>();
                    if (rigidBody != null)
                    {
                        StartCoroutine(ResetRigidbodyRoutine(rigidBody));
                    }
                }
            }
        }

        IEnumerator ResetRigidbodyRoutine(Rigidbody body)
        {
            body.isKinematic = true;
            yield return new WaitForFixedUpdate();
            body.isKinematic = false;

        }
    }
}
