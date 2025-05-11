using System;
using TMPro;
using UnityEngine.UI;
#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
using UnityEngine.XR.ARSubsystems;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MetaSyncAnchors : MonoBehaviour
    {
        [SerializeField]
        MetaSharedAnchorsMenu m_MetaSharedAnchorsMenu;

        [SerializeField]
        Button m_SyncAnchorsButton;

        [SerializeField]
        GameObject m_SyncAnchorsUIGroup;

        [SerializeField]
        GameObject m_SyncingAnchorsUIGroup;

        [SerializeField]
        LoadingVisualizer m_LoadingVisualizer;

        [SerializeField]
        FadeAfterDuration m_ErrorResultMessageBanner;

        [SerializeField]
        FadeAfterDuration m_ZeroLoadedAnchorsMessageBanner;

        [SerializeField]
        TextMeshProUGUI m_ResultMessageLabel;

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
        public void SyncAnchors()
        {
            m_MetaSharedAnchorsMenu.LoadAllSharedAnchorsFromGroup();
        }

        void Awake()
        {
            if (m_MetaSharedAnchorsMenu == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_MetaSharedAnchorsMenu)} is null."), this);

            if (m_SyncAnchorsButton == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_SyncAnchorsButton)} is null."), this);

            if (m_SyncAnchorsUIGroup == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_SyncAnchorsUIGroup)} is null."), this);

            if (m_SyncingAnchorsUIGroup == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_SyncingAnchorsUIGroup)} is null."), this);

            if (m_LoadingVisualizer == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_LoadingVisualizer)} is null."), this);

            if (m_ErrorResultMessageBanner == null)
                Debug.LogException(new NullReferenceException($"{nameof(m_ErrorResultMessageBanner)} is null."), this);

            m_MetaSharedAnchorsMenu.syncAnchorRequested.AddListener(OnSyncAnchorsRequested);
            m_MetaSharedAnchorsMenu.syncAnchorCompleted.AddListener(OnSyncAnchorsCompleted);
            m_ErrorResultMessageBanner.FadeComplete.AddListener(OnErrorMessageBannerFadeComplete);
        }

        void OnDestroy()
        {
            if (m_MetaSharedAnchorsMenu != null)
            {
                m_MetaSharedAnchorsMenu.syncAnchorRequested.RemoveListener(OnSyncAnchorsRequested);
                m_MetaSharedAnchorsMenu.syncAnchorCompleted.RemoveListener(OnSyncAnchorsCompleted);
                m_ErrorResultMessageBanner.FadeComplete.RemoveListener(OnErrorMessageBannerFadeComplete);
            }
        }

        void OnSyncAnchorsRequested()
        {
            m_SyncAnchorsButton.interactable = false;
            m_SyncAnchorsUIGroup.SetActive(false);
            m_SyncingAnchorsUIGroup.SetActive(true);
            m_LoadingVisualizer.StartAnimating();
        }

        void OnErrorMessageBannerFadeComplete()
        {
            m_SyncAnchorsButton.interactable = true;
        }

        void OnSyncAnchorsCompleted(XRResultStatus resultStatus, int loadedAnchorsCount)
        {
            m_SyncAnchorsUIGroup.SetActive(true);
            m_SyncingAnchorsUIGroup.SetActive(false);
            m_LoadingVisualizer.StopAnimating();

            if (resultStatus.IsError())
            {
                m_ResultMessageLabel.text = resultStatus.ToString();
                m_ErrorResultMessageBanner.gameObject.SetActive(true);
            }

            if (resultStatus.IsSuccess() && loadedAnchorsCount == 0)
                m_ZeroLoadedAnchorsMessageBanner.gameObject.SetActive(true);

            m_SyncAnchorsButton.interactable = true;
        }
#endif // METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
    }
}
