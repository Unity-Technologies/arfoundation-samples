using TMPro;
using UnityEngine.UI;

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
using UnityEngine.XR.OpenXR.Features.Meta;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ColocationAdvertisementIndicator : MonoBehaviour
    {
        [SerializeField]
        Image m_StatusImage;

        [SerializeField]
        TextMeshProUGUI m_Label;

        [SerializeField]
        Sprite m_AdvertisementActiveSprite;

        [SerializeField]
        Sprite m_AdvertisementStartingSprite;

        [SerializeField]
        Sprite m_AdvertisementInactiveSprite;

#if METAOPENXR_2_2_OR_NEWER && (UNITY_ANDROID || UNITY_EDITOR)
        public void SetStatus(ColocationState state)
        {
            switch (state)
            {
                case ColocationState.Starting:
                    m_StatusImage.sprite = m_AdvertisementStartingSprite;
                    m_Label.text = "Advertisement attempting to restart...";
                    break;
                case ColocationState.Active:
                    m_StatusImage.sprite = m_AdvertisementActiveSprite;
                    m_Label.text = "Advertisement active";
                    break;
                case ColocationState.Stopping:
                    m_StatusImage.sprite = m_AdvertisementInactiveSprite;
                    m_Label.text = "Advertisement inactive";
                    break;
                case ColocationState.Inactive:
                    break;
            }
        }

        public void ShowIndicator()
        {
            gameObject.SetActive(true);
        }

        public void HideIndicator()
        {
            gameObject.SetActive(false);
        }
#endif
    }
}
