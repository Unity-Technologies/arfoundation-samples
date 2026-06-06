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
        GameObject m_SearchButton;

        [SerializeField]
        Button m_CloseButton;

        [SerializeField]
        TMP_InputField m_InputField;

        public void ShowSearchContent()
        {
            m_DropdownMenu.SetActive(false);
            m_SearchButton.SetActive(false);

            m_CloseButton.gameObject.SetActive(true);
            m_InputField.gameObject.SetActive(true);

            m_InputField.ActivateInputField();
        }

        public void HideSearchContent()
        {
            m_DropdownMenu.SetActive(true);
            m_SearchButton.SetActive(true);

            m_CloseButton.gameObject.SetActive(false);
            m_InputField.gameObject.SetActive(false);

            m_InputField.text = string.Empty;
        }
    }
}
