namespace UnityEngine.XR.ARFoundation.Samples
{
    public class OcclusionTextureDebugger : MonoBehaviour
    {
        [SerializeField]
        AROcclusionManager m_OcclusionManager;

        void OnEnable()
        {
            if (m_OcclusionManager == null)
            {
                m_OcclusionManager = FindAnyObjectByType<AROcclusionManager>();
                if (m_OcclusionManager != null)
                {
                    Debug.LogWarning(
                        $"Serialized field on {nameof(OcclusionTextureDebugger)} was not set. Set this in the Inspector for better performance.", this);
                }
                else
                {
                    Debug.LogError($"Serialized field on {nameof(OcclusionTextureDebugger)} was not set. Disabling component.", this);
                    enabled = false;
                    return;
                }
            }

            m_OcclusionManager.frameReceived += OnOcclusionFrameReceived;
        }

        void OnDisable()
        {
            m_OcclusionManager.frameReceived -= OnOcclusionFrameReceived;
        }

        static void OnOcclusionFrameReceived(AROcclusionFrameEventArgs eventArgs)
        {
            Debug.Log(eventArgs.ToString());
        }

        void Reset()
        {
            m_OcclusionManager = FindAnyObjectByType<AROcclusionManager>();
        }
    }
}
