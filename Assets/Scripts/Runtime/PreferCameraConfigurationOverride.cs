
namespace UnityEngine.XR.ARFoundation.Samples.Runtime
{
    public class PreferCameraConfigurationOverride : MonoBehaviour
    {
        [SerializeField]
        ARSession m_Session;

        void Start()
        {
            Debug.Assert(m_Session != null, "ARSession must not be null");
            Debug.Assert(m_Session.subsystem != null, "ARSession must have a subsystem");

            m_Session.subsystem.configurationChooser = new PreferCameraConfigurationChooser();
        }

        void Reset() => m_Session = FindAnyObjectByType<ARSession>();
    }
}
