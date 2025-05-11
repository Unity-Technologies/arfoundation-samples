namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Destroys the GameObject it is attached to after a specified amount of time.
    /// </summary>
    public class DestroySelf : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The amount of time, in seconds, to wait after Start before destroying the GameObject.")]
        float m_Lifetime = 0.25f;

        /// <summary>
        /// The amount of time, in seconds, to wait after Start before destroying the GameObject.
        /// </summary>
        public float lifetime
        {
            get => m_Lifetime;
            set => m_Lifetime = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Start()
        {
            Destroy(gameObject, m_Lifetime);
        }
    }
}
