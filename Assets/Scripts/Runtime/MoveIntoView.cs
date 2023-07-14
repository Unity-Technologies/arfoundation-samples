using System.Collections;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Moves a GameObject a fixed distance from the camera on Start, in the direction that the user is facing.
    /// </summary>
    public class MoveIntoView : MonoBehaviour
    {
        [SerializeField, Tooltip("The Camera to use for line of sight calculations")]
        Camera m_Camera;

        [SerializeField, Tooltip("Distance from the Camera to move this GameObject on Start"), Range(.3f, 3)]
        float m_DistanceFromCamera = 1.5f;

        [SerializeField, Tooltip("If true, rotate this GameObject to face the Camera after moving")]
        bool m_FaceCameraAfterMove = true;

#pragma warning disable 0108 // Hides Component.camera, a deprecated property in Unity 2022 and below
        public Camera camera
#pragma warning restore 0108
        {
            get => m_Camera;
            set => m_Camera = value;
        }

        public float distanceFromCamera
        {
            get => m_DistanceFromCamera;
            set => m_DistanceFromCamera = value;
        }

        public bool faceCameraAfterMove
        {
            get => m_FaceCameraAfterMove;
            set => m_FaceCameraAfterMove = value;
        }

        IEnumerator Start()
        {
            if (m_Camera == null)
            {
                Debug.LogWarning($"There is no Camera parameter assigned, so {nameof(MoveIntoView)} component on {gameObject.name} will have no effect.", this);
                yield break;
            }

            yield return null;
            PerformMove();
        }

        public void PerformMove()
        {
            var camTransform = m_Camera.transform;
            var camPos = camTransform.position;
            var menuOffset = camTransform.forward;

            // Project the camera's forward vector onto the XZ plane
            menuOffset.y = 0;
            menuOffset.Normalize();

            transform.position = camPos + menuOffset * m_DistanceFromCamera;
            if (m_FaceCameraAfterMove)
                transform.LookAt(m_Camera.transform);
        }

        void Reset()
        {
            m_Camera = Camera.main;
        }
    }
}
