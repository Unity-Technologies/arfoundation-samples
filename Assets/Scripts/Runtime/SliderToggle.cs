using System;
using TMPro;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// A component that combines the visuals of a Slider with the functionality of a Toggle.
    /// Valid states:
    /// 1. Enabled On
    /// 2. Enabled Off
    /// 3. Disabled Off
    /// </summary>
    public class SliderToggle : MonoBehaviour
    {
        enum State
        {
            ENABLED_ON,
            ENABLED_OFF,
            DISABLED_OFF
        }
        
        [Tooltip("The Slider element used to display the on/off state of this element.")]
        [SerializeField]
        Slider m_Slider;

        [Tooltip("The Button element used for hit detection.")]
        [SerializeField]
        Button m_Button;
        
        [Space]
        
        [Tooltip("The Image element used for displaying the background.")]
        [SerializeField]
        Image m_Background;

        [Tooltip("The background color to use when the SliderToggle is enabled and on.")]
        [SerializeField]
        Color m_EnabledOnBackgroundColor;
        
        [Tooltip("The background color to use when the SliderToggle is enabled and off.")]
        [SerializeField]
        Color m_EnabledOffBackgroundColor;
        
        [Tooltip("The background color to use when the SliderToggle is disabled.")]
        [SerializeField]
        Color m_DisabledBackgroundColor;
        
        [Space]
        
        [Tooltip("The Image element used for displaying the handle.")]
        [SerializeField]
        Image m_Handle;
        
        [Tooltip("The handle color to use when the SliderToggle is enabled.")]
        [SerializeField]
        Color m_EnabledHandleColor;
        
        [Tooltip("The handle color to use when the SliderToggle is disabled.")]
        [SerializeField]
        Color m_DisabledHandleColor;
        
        [Space]
        
        [Tooltip("The Image element that is used for the outline.")]
        [SerializeField]
        Image m_Outline;
        
        [Tooltip("The outline color to use when the SliderToggle is enabled and on.")]
        [SerializeField]
        Color m_EnabledOnOutlineColor;

        [Tooltip("The outline color to use when the SliderToggle is enabled and off.")]
        [SerializeField]
        Color m_EnabledOffOutlineColor;
        
        [Tooltip("The outline color to use when the SliderToggle is disabled.")]
        [SerializeField]
        Color m_DisabledOutlineColor;
        
        [Space]
        
        [Tooltip("The text label associated with this SliderToggle.")]
        [SerializeField]
        TextMeshProUGUI m_Text;
        
        [Tooltip("The text color to use when the SliderToggle is enabled.")]
        [SerializeField]
        Color m_EnabledTextColor;
        
        [Tooltip("The text color to use when the SliderToggle is disabled.")]
        [SerializeField]
        Color m_DisabledTextColor;

        public void SetSliderValue(bool value)
        {
            if (m_Slider == null)
            {
                throw new InvalidOperationException("The value of 'Slider' on component SliderToggle must be set in the inspector in order to use this component.");
            }
            
            SetState(value ? State.ENABLED_ON : State.ENABLED_OFF);
        }

        public void SetEnabled(bool value)
        {
            if (m_Slider == null)
            {
                throw new InvalidOperationException("The value of 'Slider' on component SliderToggle must be set in the inspector in order to use this component.");
            }
            
            if (value)
                // If setting enabled=true, use current slider value to determine on/off state
                SetState(m_Slider.value == 0 ? State.ENABLED_OFF : State.ENABLED_ON);
            else
                SetState(State.DISABLED_OFF);
        }

        void SetState(State state)
        {
            switch (state)
            {
                case State.ENABLED_ON:
                    m_Slider.value = 1.0f;
                    m_Background.color = m_EnabledOnBackgroundColor;
                    m_Handle.color = m_EnabledHandleColor;
                    m_Outline.color = m_EnabledOnOutlineColor;
                    m_Text.color = m_EnabledTextColor;
                    m_Button.enabled = true;
                    break;
                case State.ENABLED_OFF:
                    m_Slider.value = 0.0f;
                    m_Background.color = m_EnabledOffBackgroundColor;
                    m_Handle.color = m_EnabledHandleColor;
                    m_Outline.color = m_EnabledOffOutlineColor;
                    m_Text.color = m_EnabledTextColor;
                    m_Button.enabled = true;
                    break;
                case State.DISABLED_OFF:
                    m_Slider.value = 0.0f;
                    m_Background.color = m_DisabledBackgroundColor;
                    m_Handle.color = m_DisabledHandleColor;
                    m_Outline.color = m_DisabledOutlineColor;
                    m_Text.color = m_DisabledTextColor;
                    m_Button.enabled = false;
                    break;
            }
        }
    }
}
