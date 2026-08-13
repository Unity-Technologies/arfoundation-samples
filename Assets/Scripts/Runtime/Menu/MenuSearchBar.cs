using System;
using TMPro;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MenuSearchBar : MonoBehaviour
    {
        [SerializeField]
        GameObject m_DropdownMenu;

        [SerializeField]
        GameObject m_BuildProfileDropdown;

        [SerializeField]
        GameObject m_SearchButton;

        [SerializeField]
        Button m_CloseButton;

        [SerializeField]
        TMP_InputField m_InputField;

        bool m_BuildProfileDropdownWasActive;

        public event Action<string> searchTextChanged;

        public string searchText => m_InputField.text;

        void OnEnable()
        {
            m_InputField.onValueChanged.AddListener(OnSearchTextChanged);
        }

        void OnDisable()
        {
            m_InputField.onValueChanged.RemoveListener(OnSearchTextChanged);
        }

        void OnSearchTextChanged(string text)
        {
            searchTextChanged?.Invoke(text);
        }

        public void ShowSearchContent()
        {
            m_DropdownMenu.SetActive(false);
            m_SearchButton.SetActive(false);

            if (m_BuildProfileDropdown != null)
            {
                m_BuildProfileDropdownWasActive =
                    m_BuildProfileDropdown.activeSelf;
                m_BuildProfileDropdown.SetActive(false);
            }

            m_CloseButton.gameObject.SetActive(true);
            m_InputField.gameObject.SetActive(true);

            m_InputField.ActivateInputField();
        }

        public void HideSearchContent()
        {
            m_DropdownMenu.SetActive(true);
            m_SearchButton.SetActive(true);

            if (m_BuildProfileDropdown != null
                && m_BuildProfileDropdownWasActive)
                m_BuildProfileDropdown.SetActive(true);

            m_CloseButton.gameObject.SetActive(false);
            m_InputField.gameObject.SetActive(false);

            m_InputField.text = string.Empty;
        }
    }
}
