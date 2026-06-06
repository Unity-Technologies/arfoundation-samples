using UnityEngine.Events;
#if UIELEMENTS_MODULE_AVAILABLE
using UnityEngine.UIElements;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Samples.WorldSpaceUI
{
    /// <summary>
    /// Sample class that demonstrates how to bind to a UI Toolkit button click event.
    /// </summary>
    public class ButtonEventSample : MonoBehaviour
    {
        [SerializeField]
        UnityEvent m_OnButtonClicked = new UnityEvent();

        /// <summary>
        /// Event to be invoked when the UI Toolkit button is clicked.
        /// </summary>
        public UnityEvent onButtonClicked
        {
            get => m_OnButtonClicked;
            set => m_OnButtonClicked = value;
        }

        const string k_LabelName = "DebugLabel";
#if UIELEMENTS_MODULE_AVAILABLE
        Button m_Button;
        Label m_Label;
#endif

        void Start()
        {
#if UIELEMENTS_MODULE_AVAILABLE
            var uiToolkitDoc = GetComponent<UIDocument>();
            if(uiToolkitDoc != null)
            {
                var root = uiToolkitDoc.rootVisualElement;
                m_Button = root.Q<Button>();
                if(m_Button != null)
                    m_Button.clicked += HandleButtonClicked;

                // Find label by name
                m_Label = root.Q<Label>(k_LabelName);
            }
#endif
        }

        private void HandleButtonClicked()
        {
            if (m_OnButtonClicked != null)
                m_OnButtonClicked.Invoke();

            if (m_Label != null)
                m_Label.text = "Button clicked at: " + Time.time;
        }
    }
}
