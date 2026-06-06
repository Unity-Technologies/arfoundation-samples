using System;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CoachingCardController : MonoBehaviour
    {
        const string k_CoachingCardSeenKey = "HasSeenCoachingCard";
        const float k_FadeInDuration = 0.3f;
        const float k_FadeOutDuration = 0.15f;

        [SerializeField]
        CanvasGroup m_CoachingCardCanvasGroup;

        [SerializeField]
        CanvasGroup m_FadedBackdropCanvasGroup;

        void Start()
        {
            var hasSeen = PlayerPrefs.GetInt(k_CoachingCardSeenKey, 0) == 1;

            if (!hasSeen)
                return;

            m_CoachingCardCanvasGroup.gameObject.SetActive(false);
            m_FadedBackdropCanvasGroup.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        [ContextMenu("Reset Popup Logic (Debug)")]
        public void ResetPopupPrefs()
        {
            Debug.Log($"Resetting PlayerPref: {k_CoachingCardSeenKey}");
            PlayerPrefs.DeleteKey(k_CoachingCardSeenKey);
            PlayerPrefs.Save();
        }
#endif

        public async void Show()
        {
            try
            {
                m_CoachingCardCanvasGroup.gameObject.SetActive(true);
                m_FadedBackdropCanvasGroup.gameObject.SetActive(true);

                await TransitionCoachingCard(1f, k_FadeInDuration);

            }
            catch (OperationCanceledException)
            {
                // do nothing
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        public async void Hide()
        {
            try
            {
                await TransitionCoachingCard(0f, k_FadeOutDuration);

                m_CoachingCardCanvasGroup.gameObject.SetActive(false);
                m_FadedBackdropCanvasGroup.gameObject.SetActive(false);

                PlayerPrefs.SetInt(k_CoachingCardSeenKey, 1);
                PlayerPrefs.Save();
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        async Awaitable TransitionCoachingCard(float targetAlpha, float duration)
        {
            var startAlpha = m_CoachingCardCanvasGroup.alpha;

            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var t =  elapsedTime / k_FadeOutDuration;
                var alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                m_CoachingCardCanvasGroup.alpha = alpha;
                m_FadedBackdropCanvasGroup.alpha = alpha;

                await Awaitable.NextFrameAsync();
            }

            m_CoachingCardCanvasGroup.alpha = targetAlpha;
            m_FadedBackdropCanvasGroup.alpha = targetAlpha;
        }
    }
}
