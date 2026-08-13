using System;
using TMPro;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Button))]
    public class SceneButtonView : MonoBehaviour
    {
        [SerializeField, ReadOnlyField]
        Button m_LaunchButton;

        [Header("Labels")]
        [SerializeField]
        TextMeshProUGUI m_SceneNameLabel;

        [SerializeField]
        TextMeshProUGUI m_DescriptionLabel;

        [Header("Images")]
        [SerializeField]
        GameObject m_FallbackImage;

        [SerializeField]
        Image m_PreviewImage;

        [SerializeField]
        GameObject m_UnsupportedBadge;

        string m_SceneDisplayName;
        Action m_OnLaunchClicked;

        public string sceneDisplayName => m_SceneDisplayName;

        void Reset()
        {
            m_LaunchButton = GetComponent<Button>();
        }

        void Awake()
        {
            if (m_LaunchButton == null)
                m_LaunchButton = GetComponent<Button>();
        }

        public void SetDisplayNameLabel(string richText)
        {
            m_SceneNameLabel.text = richText;
        }

        public void Initialize(
            string sceneDisplayName,
            string description,
            bool isSupported,
            Sprite previewImage,
            Action onLaunchClicked)
        {
            m_SceneDisplayName = sceneDisplayName;
            m_SceneNameLabel.text = sceneDisplayName;
            m_DescriptionLabel.text = description;
            m_UnsupportedBadge.SetActive(!isSupported);

            if (previewImage != null)
            {
                m_FallbackImage.SetActive(false);
                m_PreviewImage.gameObject.SetActive(true);
                m_PreviewImage.sprite = previewImage;
            }

            m_OnLaunchClicked = onLaunchClicked;
            m_LaunchButton.onClick.AddListener(HandleLaunchClicked);
        }

        void OnDestroy()
        {
            if (m_LaunchButton != null)
                m_LaunchButton.onClick.RemoveListener(HandleLaunchClicked);
        }

        void HandleLaunchClicked()
        {
            m_OnLaunchClicked?.Invoke();
        }
    }
}
