using UnityEngine.UI;

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

        [SerializeField]
        Text m_textField;

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

        public void ChangePrefab()
        {
            switch (m_State)
            {
                case State.OriginalPrefab: 
                    m_State = State.ChangeToAlternativePrefab;
                    break;
                case State.AlternativePrefab:
                    m_State = State.ChangeToOriginalPrefab;
                    break;
                default:
                    break;
            }
        }

        void UpdateMessage()
        {
            switch (m_State)
            {
                case State.OriginalPrefab:
                {
                    m_textField.text = $"Alternative Prefab for {GetComponent<PrefabImagePairManager>().imageLibrary[0].name}";
                    break;
                }
                case State.AlternativePrefab:
                {
                    m_textField.text = $"Original Prefab for {GetComponent<PrefabImagePairManager>().imageLibrary[0].name}";
                    break;
                }
                case State.Error:
                {
                    m_textField.text = m_ErrorMessage;
                    break;
                }
            }
        }

        void UpdatePrefab()
        {
            switch (m_State)
            {
                case State.ChangeToAlternativePrefab:
                {
                    if (!alternativePrefab)
                    {
                        SetError("No alternative prefab is given.");
                        break;
                    }

                    var manager = GetComponent<PrefabImagePairManager>();
                    if (!manager)
                    {
                        SetError($"No {nameof(PrefabImagePairManager)} available.");
                        break;
                    }

                    var library = manager.imageLibrary;
                    if (!library)
                    {
                        SetError($"No image library available.");
                        break;
                    }

                    if (!m_OriginalPrefab)
                        m_OriginalPrefab = manager.GetPrefabForReferenceImage(library[0]);

                    manager.SetPrefabForReferenceImage(library[0], alternativePrefab);
                    m_State = State.AlternativePrefab;
                    break;
                }

                case State.ChangeToOriginalPrefab:
                {
                    if (!m_OriginalPrefab)
                    {
                        SetError("No original prefab is given.");
                        break;
                    }

                    var manager = GetComponent<PrefabImagePairManager>();
                    if (!manager)
                    {
                        SetError($"No {nameof(PrefabImagePairManager)} available.");
                        break;
                    }

                    var library = manager.imageLibrary;
                    if (!library)
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

        void SetError(string errorMessage)
        {
            m_State = State.Error;
            m_ErrorMessage = $"Error: {errorMessage}";
        }

        void Update()
        {
            UpdatePrefab();
            UpdateMessage();
        }
    }
}
