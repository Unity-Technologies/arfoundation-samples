using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Launches projectiles from a touch point with the specified <see cref="initialSpeed"/>.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ProjectileLauncher : MonoBehaviour
    {
        [SerializeField]
        Rigidbody m_ProjectilePrefab;

        public Rigidbody projectilePrefab
        {
            get => m_ProjectilePrefab;
            set => m_ProjectilePrefab = value;
        }

        [SerializeField]
        float m_InitialSpeed = 25;

        public float initialSpeed
        {
            get => m_InitialSpeed;
            set => m_InitialSpeed = value;
        }

        void Update()
        {
            if (m_ProjectilePrefab == null)
                return;

            if (Input.touchCount == 0)
                return;

            var touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                var ray = GetComponent<Camera>().ScreenPointToRay(touch.position);
                var projectile = Instantiate(m_ProjectilePrefab, ray.origin, Quaternion.identity);
                var rigidbody = projectile.GetComponent<Rigidbody>();
                rigidbody.velocity = ray.direction * m_InitialSpeed;
            }
        }
    }
}