using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Launches projectiles from a touch point with the specified <see cref="initialSpeed"/>.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ProjectileLauncher : PressInputBase
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

        protected override void OnPressBegan(Vector3 position)
        {
            if (m_ProjectilePrefab == null)
                return;

            var ray = GetComponent<Camera>().ScreenPointToRay(position);
            var projectile = Instantiate(m_ProjectilePrefab, ray.origin, Quaternion.identity);
            var rigidbody = projectile.GetComponent<Rigidbody>();
            rigidbody.velocity = ray.direction * m_InitialSpeed;

        }
    }
}
