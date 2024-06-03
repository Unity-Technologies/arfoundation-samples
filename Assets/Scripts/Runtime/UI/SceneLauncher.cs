using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class SceneLauncher : MonoBehaviour
    {
        [SerializeField]
        SampleSceneDescriptor m_SceneDescriptor;
        public SampleSceneDescriptor sceneDescriptor => m_SceneDescriptor;

        [SerializeField, HideInInspector]
        Button m_Button;

        void OnEnable()
        {
            if (sceneDescriptor.EvaluateRequirements())
                m_Button.onClick.AddListener(LaunchScene);
            else
                m_Button.SetEnabled(false);
        }

        void OnDisable()
        {
            m_Button.onClick.RemoveListener(LaunchScene);
        }

        public void LaunchScene()
        {
            if (m_SceneDescriptor == null)
            {
                Debug.LogError("Scene Descriptor is null. Cannot launch scene", this);
                return;
            }

            if (!m_SceneDescriptor.EvaluateRequirements())
            {
                Debug.LogError($"Cannot launch scene {m_SceneDescriptor.sceneName} because it is not supported on this device.");
                return;
            }

            SceneManager.LoadScene(m_SceneDescriptor.sceneName, LoadSceneMode.Single);
        }

        void Reset()
        {
            m_Button = GetComponent<Button>();
        }
    }
}
