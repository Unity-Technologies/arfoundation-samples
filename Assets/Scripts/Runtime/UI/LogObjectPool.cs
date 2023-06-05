using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Manages an object pool to display application logs to the screen.
    /// </summary>
    /// <remarks>
    /// This component is designed to work as part of a specific prefab structure and is not currently safe to add
    /// as a standalone component.
    /// </remarks>
    [AddComponentMenu("")]
    public class LogObjectPool : MonoBehaviour
    {
        [SerializeField, Range(0, 9999)]
        [Tooltip("Total number of GameObjects in the object pool.")]
#pragma warning disable CS0414
        int m_PoolSize = 20;
#pragma warning restore CS0414

        /// <summary>
        /// Object pool is initialized by <c>LogObjectPoolEditor</c> when user presses the "Regenerate Object Pool" button.
        /// </summary>
        [SerializeField]
        List<TextMeshProUGUI> m_Pool = new();

        [SerializeField]
        ScrollRect m_ScrollRect;

        [SerializeField]
        RectTransform m_ScrollViewContent;

        [SerializeField]
        [Tooltip("One or more template GameObjects to repeatedly instantiate in the object pool.")]
        List<GameObject> m_TemplateObjects;

        [SerializeField]
        float m_ObjectHeight;

        int m_NumObjectsEnabled;

        void Start()
        {
            if (m_ScrollViewContent == null)
            {
                Debug.LogError($"{nameof(LogObjectPool)} component on {name} has a null Scroll View Content.", this);
                return;
            }

            Application.logMessageReceived += HandleLog;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (m_NumObjectsEnabled == m_Pool.Count)
            {
                for (var i = 0; i < m_NumObjectsEnabled - 1; i++)
                {
                    m_Pool[i].text = m_Pool[i + 1].text;
                }
            }

            m_Pool[GetIndexOfLastObject()].text = logString;

            if (m_NumObjectsEnabled < m_Pool.Count)
            {
                m_Pool[m_NumObjectsEnabled].transform.parent.gameObject.SetActive(true);
                m_ScrollViewContent.SetHeight(m_ScrollViewContent.GetHeight() + m_ObjectHeight);
                m_NumObjectsEnabled++;
            }

            m_ScrollRect.normalizedPosition = new Vector2(0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetIndexOfLastObject()
        {
            if (m_NumObjectsEnabled < m_Pool.Count)
                return m_NumObjectsEnabled;

            return m_NumObjectsEnabled - 1;
        }
    }
}
