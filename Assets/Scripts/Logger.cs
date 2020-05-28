using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class Logger : MonoBehaviour
    {
        [SerializeField]
        Text m_LogText;
        public Text logText
        {
            get { return s_LogText; }
            set
            {
                m_LogText = value;
                s_LogText = value;
            }
        }

        [SerializeField]
        int m_VisibleMessageCount = 40;
        public int visibleMessageCount
        {
            get { return s_VisibleMessageCount; }
            set
            {
                m_VisibleMessageCount = value;
                s_VisibleMessageCount = value;
            }
        }

        int m_LastMessageCount;

        static int s_VisibleMessageCount;

        static Text s_LogText;

        static List<string> s_Log = new List<string>();

        static StringBuilder s_StringBuilder = new StringBuilder();

        void Awake()
        {
            s_LogText = m_LogText;
            s_VisibleMessageCount = m_VisibleMessageCount;
            Log("Log console initialized.");
        }

        void Update()
        {
            lock (s_Log)
            {
                if (m_LastMessageCount != s_Log.Count)
                {
                    s_StringBuilder.Clear();
                    var startIndex = Mathf.Max(s_Log.Count - s_VisibleMessageCount, 0);
                    for (int i = startIndex; i < s_Log.Count; ++i)
                    {
                        s_StringBuilder.Append($"{i:000}> {s_Log[i]}\n");
                    }

                    s_LogText.text = s_StringBuilder.ToString();
                }

                m_LastMessageCount = s_Log.Count;
            }
        }

        public static void Log(string message)
        {
            lock (s_Log)
            {
                if (s_Log == null)
                    s_Log = new List<string>();

                s_Log.Add(message);
            }
        }
    }
}