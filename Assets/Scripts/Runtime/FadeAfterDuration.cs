using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class FadeAfterDuration : MonoBehaviour
    {
        [SerializeField]
        List<Graphic> m_GraphicsToFade = new();

        [SerializeField, Min(0)]
        float m_SecondsBeforeFade = 5;

        [SerializeField, Min(.001f)]
        float m_SecondsToFade = 2;

        List<Color> m_StartColors = new();
        float m_FadeBeginTime;

        [SerializeField]
        UnityEvent m_FadeComplete = new();
        public UnityEvent FadeComplete => m_FadeComplete;

        CancellationTokenSource m_CancellationTokenSource = new();

        public void ResetFade()
        {
            m_CancellationTokenSource.Cancel();
            m_CancellationTokenSource = new();
        }

        void Awake()
        {
            foreach (var g in m_GraphicsToFade)
            {
                m_StartColors.Add(g.color);
            }
        }

        void OnEnable()
        {
            Fade(m_CancellationTokenSource.Token);
        }

        async void Fade(CancellationToken cancellationToken)
        {
            try
            {
                try
                {
                    await Awaitable.WaitForSecondsAsync(m_SecondsBeforeFade, cancellationToken);
                }
                catch
                {
                    return;
                }

                m_FadeBeginTime = Time.time;
                var fadePercent = (Time.time - m_FadeBeginTime) / m_SecondsToFade;
                while (fadePercent < 1)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    for (var i = 0; i < m_GraphicsToFade.Count; i++)
                    {
                        var c = m_StartColors[i];
                        m_GraphicsToFade[i].color = new Color(c.r, c.g, c.b, c.a * (1 - fadePercent));
                    }

                    fadePercent = (Time.time - m_FadeBeginTime) / m_SecondsToFade;

                    try
                    {
                        await Awaitable.NextFrameAsync(cancellationToken);
                    }
                    catch
                    {
                        return;
                    }
                }

                for (var i = 0; i < m_GraphicsToFade.Count; i++)
                {
                    m_GraphicsToFade[i].color = m_StartColors[i];
                }

                gameObject.SetActive(false);
                m_FadeComplete?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void OnDestroy()
        {
            m_CancellationTokenSource.Cancel();
        }
    }
}
