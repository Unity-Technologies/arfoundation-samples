using System;
using System.Collections;
using System.Collections.Generic;
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

        WaitForSeconds m_WaitForSeconds;
        float m_FadeBeginTime;

        void OnEnable()
        {
            foreach (var g in m_GraphicsToFade)
            {
                m_StartColors.Add(g.color);
            }

            m_WaitForSeconds ??= new WaitForSeconds(m_SecondsBeforeFade);
            StartCoroutine(Fade());
        }

        IEnumerator Fade()
        {
            yield return m_WaitForSeconds;

            m_FadeBeginTime = Time.time;
            var fadePercent = (Time.time - m_FadeBeginTime) / m_SecondsToFade;
            while (fadePercent < 1)
            {
                for (var i = 0; i < m_GraphicsToFade.Count; i++)
                {
                    var c = m_StartColors[i];
                    m_GraphicsToFade[i].color = new Color(c.r, c.g, c.b, c.a * (1 - fadePercent));
                }

                fadePercent = (Time.time - m_FadeBeginTime) / m_SecondsToFade;
                yield return null;
            }

            for (var i = 0; i < m_GraphicsToFade.Count; i++)
            {
                m_GraphicsToFade[i].color = m_StartColors[i];
            }

            gameObject.SetActive(false);
        }
    }
}
