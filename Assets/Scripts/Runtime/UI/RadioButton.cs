using System.Collections.Generic;
using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// A simple radio button system for use with Unity UI Buttons.
    /// </summary>
    public class RadioButton : MonoBehaviour
    {
        [SerializeField]
        ColorAsset m_DeselectedColor;

        [SerializeField]
        ColorAsset m_SelectedColor;

        [SerializeField]
        TextMeshProUGUI m_Text;

        [SerializeField]
        bool m_SelectOnStart;

        [SerializeField]
        List<GameObject> m_ShowWhenSelected = new();

        static RadioButton s_SelectedButton;

        void Reset()
        {
            TryInitializeSerializedFields();
        }

        void Start()
        {
            if (m_SelectOnStart)
                Select();
        }

        public void Select()
        {
            if (s_SelectedButton != null)
                s_SelectedButton.Deselect();

            m_Text.color = m_SelectedColor.color;
            foreach (var g in m_ShowWhenSelected)
            {
                g.SetActive(true);
            }

            s_SelectedButton = this;
        }

        public void Deselect()
        {
            m_Text.color = m_DeselectedColor.color;
            foreach (var g in m_ShowWhenSelected)
            {
                g.SetActive(false);
            }

            s_SelectedButton = null;
        }

        [ContextMenu("Try Initialize Serialized Fields")]
        void TryInitializeSerializedFields()
        {
            m_Text = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
