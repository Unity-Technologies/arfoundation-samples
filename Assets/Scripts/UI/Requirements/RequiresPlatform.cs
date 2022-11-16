using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Button))]
    public class RequiresPlatform : MonoBehaviour
    {
        enum Platform { iOS, Android }

        Button m_Button;

        [SerializeField]
        Platform m_RequiredPlatform;

        [SerializeField]
        bool m_RequiresMinimumVersion;

        [SerializeField]
        int m_RequiredVersion;

        [SerializeField]
        bool m_AllowEditor;

        void Start()
        {
            m_Button = GetComponent<Button>();

#if !UNITY_IOS
            if (m_RequiredPlatform == Platform.iOS)
            {
                ARSceneSelectUI.DisableButton(m_Button);
                return;
            }

#elif !UNITY_ANDROID
            if (m_RequiredPlatform == Platform.Android)
            {
                ARSceneSelectUI.DisableButton(m_Button);
                return;
            }
#endif

            if (Application.isEditor && !m_AllowEditor)
            {
                ARSceneSelectUI.DisableButton(m_Button);
                return;
            }
            
#if UNITY_IOS
            if (m_RequiresMinimumVersion)
            {
                string version = iOS.Device.systemVersion;
                int major = int.Parse(version.Split(".")[0]);
                if (major < m_RequiredVersion)
                    ARSceneSelectUI.DisableButton(m_Button);
            }
#endif
        }
    }
}
