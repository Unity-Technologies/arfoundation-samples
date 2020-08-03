using System;
using System.Text;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Change the prefab for the first image in library at runtime.
    /// </summary>
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class DynamicPrefab : MonoBehaviour
    {
        GameObject m_OriginalPrefab;

        [SerializeField]
        GameObject m_AlternativePrefab;

        public GameObject alternativePrefab
        {
            get => m_AlternativePrefab;
            set => m_AlternativePrefab = value;
        }

        enum State
        {
            OriginalPrefab,
            ChangeToOriginalPrefab,
            AlternativePrefab,
            ChangeToAlternativePrefab,
            Error
        }

        State m_State;

        string m_ErrorMessage = "";

        void OnGUI()
        {
            var fontSize = 50;
            GUI.skin.button.fontSize = fontSize;
            GUI.skin.label.fontSize = fontSize;

            float margin = 100;

            GUILayout.BeginArea(new Rect(margin, margin, Screen.width - margin * 2, Screen.height - margin * 2));

            switch (m_State)
            {
                case State.OriginalPrefab:
                    {
                        if (GUILayout.Button($"Alternative Prefab for {GetComponent<MultiTrackedImageInfoManager>().imageLibrary[0].name}"))
                        {
                            m_State = State.ChangeToAlternativePrefab;
                        }
                        break;
                    }
                case State.AlternativePrefab:
                    {
                        if (GUILayout.Button($"Original Prefab for {GetComponent<MultiTrackedImageInfoManager>().imageLibrary[0].name}"))
                        {
                            m_State = State.ChangeToOriginalPrefab;
                        }
                        break;
                    }
                case State.Error:
                    {
                        GUILayout.Label(m_ErrorMessage);
                        break;
                    }
            }

            GUILayout.EndArea();
        }

        void SetError(string errorMessage)
        {
            m_State = State.Error;
            m_ErrorMessage = $"Error: {errorMessage}";
        }

        void Update()
        {
            switch (m_State)
            {
                case State.ChangeToAlternativePrefab:
                    {
                        if (alternativePrefab == null)
                        {
                            SetError("No alternative prefab is given.");
                            break;
                        }

                        var manager = GetComponent<MultiTrackedImageInfoManager>();
                        if (manager == null)
                        {
                            SetError($"No {nameof(MultiTrackedImageInfoManager)} available.");
                            break;
                        }

                        var library = manager.imageLibrary;
                        if (library == null)
                        {
                            SetError($"No image library available.");
                            break;
                        }

                        if (m_OriginalPrefab == null)
                            m_OriginalPrefab = manager.GetPrefabForReferenceImage(library[0]);

                        manager.SetPrefabForReferenceImage(library[0], alternativePrefab);
                        m_State = State.AlternativePrefab;
                        break;
                    }

                case State.ChangeToOriginalPrefab:
                    {
                        if (m_OriginalPrefab == null)
                        {
                            SetError("No original prefab is given.");
                            break;
                        }

                        var manager = GetComponent<MultiTrackedImageInfoManager>();
                        if (manager == null)
                        {
                            SetError($"No {nameof(MultiTrackedImageInfoManager)} available.");
                            break;
                        }

                        var library = manager.imageLibrary;
                        if (library == null)
                        {
                            SetError($"No image library available.");
                            break;
                        }

                        manager.SetPrefabForReferenceImage(library[0], m_OriginalPrefab);
                        m_State = State.OriginalPrefab;
                        break;
                    }

            }
        }
    }
}