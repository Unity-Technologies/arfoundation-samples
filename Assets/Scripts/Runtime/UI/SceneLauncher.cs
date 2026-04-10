using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.XR.OpenXR;
#endif

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

        Canvas m_Canvas;

        void OnEnable()
        {
            m_Canvas = GetComponent<Canvas>();
            if (m_Canvas != null)
                m_Canvas.enabled = false;

            var delaySeconds = Application.isEditor ? 1 : 0;
#if UNITY_ANDROID && !UNITY_EDITOR
            if (XRManagerUtility.IsLoaderActive<OpenXRLoader>())
                delaySeconds = 3;
#endif
            EvaluateRequirementsAfterDelay(delaySeconds);
        }

        async void EvaluateRequirementsAfterDelay(float seconds)
        {

            if (seconds > 0)
            {
                try
                {
                    await Awaitable.WaitForSecondsAsync(seconds);
                }
                catch (OperationCanceledException)
                {
                    // do nothing
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return;
                }
            }

            if (m_Canvas != null)
                m_Canvas.enabled = true;

            if (sceneDescriptor.EvaluateRequirements())
            {
                m_Button.onClick.AddListener(LaunchScene);
                m_Button.SetEnabled(true);
            }
            else
            {
                m_Button.SetEnabled(false);
            }
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
