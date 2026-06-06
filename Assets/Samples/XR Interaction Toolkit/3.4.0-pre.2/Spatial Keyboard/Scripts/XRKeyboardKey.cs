#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// Keyboard key used to interface with <see cref="XRKeyboard"/>.
    /// </summary>
    public class XRKeyboardKey : Button
    {
        [SerializeField, Tooltip("KeyFunction used for this key which is called when key is pressed. Used to communicate with the Keyboard.")]
        KeyFunction m_KeyFunction;

        /// <summary>
        /// <see cref="KeyFunction"/> used for this key which is called when key is pressed. Used to communicate with
        /// the Keyboard.
        /// </summary>
        public KeyFunction keyFunction
        {
            get => m_KeyFunction;
            set => m_KeyFunction = value;
        }

        [SerializeField, Tooltip("(Optional) KeyCode used for this key. Used in conjunction with Key Function or as a fallback for standard commands.")]
        KeyCode m_KeyCode;

        /// <summary>
        /// (Optional) <see cref="KeyCode"/> used for this key. Used in conjunction with Key Function or as a fallback for standard commands.
        /// </summary>
        public KeyCode keyCode
        {
            get => m_KeyCode;
            set => m_KeyCode = value;
        }

        [SerializeField, Tooltip("Character for this key in non-shifted state. This string will be passed to the keyboard and appended to the keyboard text string or processed as a keyboard command.")]
        string m_Character;

        /// <summary>
        /// Character for this key in non-shifted state. This string will be passed to the keyboard and appended
        /// to the keyboard text string or processed as a keyboard command (i.e. '\s' for shift)
        /// </summary>
        public string character
        {
            get => m_Character;
            set => m_Character = value;
        }

        [SerializeField, Tooltip("(Optional) Display character for this key in a non-shifted state. This string will be displayed on the key text field. If empty, character will be used as a fall back.")]
        string m_DisplayCharacter;

        /// <summary>
        /// (Optional) Display character for this key in a non-shifted state. This string will be displayed on the
        /// key text field. If left empty, <see cref="character"/> will be used instead.
        /// </summary>
        public string displayCharacter
        {
            get => m_DisplayCharacter;
            set
            {
                m_DisplayCharacter = value;
                RefreshDisplayCharacter();
            }
        }

        [SerializeField, Tooltip("(Optional) Display icon for this key in a non-shifted state. This icon will be displayed on the key icon image. If empty, the display character or character will be used as a fall back.")]
        Sprite m_DisplayIcon;

        /// <summary>
        /// (Optional) Display icon for this key in a non-shifted state. This icon will be displayed on the key icon image.
        /// If empty, the display character or character will be used instead.
        /// </summary>
        public Sprite displayIcon
        {
            get => m_DisplayIcon;
            set
            {
                m_DisplayIcon = value;
                if (m_IconComponent != null)
                {
                    m_IconComponent.sprite = m_DisplayIcon;
                    m_IconComponent.enabled = m_DisplayIcon != null;
                }

                RefreshDisplayCharacter();
            }
        }

        [SerializeField, Tooltip("Character for this key in a shifted state. This string will be passed to the keyboard and appended to the keyboard text string or processed as a keyboard command.")]
        string m_ShiftCharacter;

        /// <summary>
        /// Character for this key in a shifted state. This string will be passed to the keyboard and appended
        /// to the keyboard text string or processed as a keyboard command (i.e. '\s' for shift).
        /// </summary>
        public string shiftCharacter
        {
            get => m_ShiftCharacter;
            set => m_ShiftCharacter = value;
        }

        [SerializeField, Tooltip("(Optional) Display character for this key in a shifted state. This string will be displayed on the key text field. If empty, shift character will be used as a fall back.")]
        string m_ShiftDisplayCharacter;

        /// <summary>
        /// (Optional) Display character for this key in a shifted state. This string will be displayed on the key text field.
        /// If empty, <see cref="shiftCharacter"/> will be used instead, and finally <see cref="character"/> will
        /// be capitalized and used if <see cref="shiftCharacter"/> is empty.
        /// </summary>
        public string shiftDisplayCharacter
        {
            get => m_ShiftDisplayCharacter;
            set
            {
                m_ShiftDisplayCharacter = value;
                RefreshDisplayCharacter();
            }
        }

        [SerializeField, Tooltip("(Optional) Display icon for this key in a shifted state. This icon will be displayed on the key icon image. If empty, the shift display character or shift character will be used as a fall back.")]
        Sprite m_ShiftDisplayIcon;

        /// <summary>
        /// (Optional) Display icon for this key in a shifted state. This icon will be displayed on the key icon image.
        /// If empty, the shift display character or shift character will be used instead.
        /// </summary>
        public Sprite shiftDisplayIcon
        {
            get => m_ShiftDisplayIcon;
            set
            {
                m_ShiftDisplayIcon = value;
                if (m_IconComponent != null)
                {
                    m_IconComponent.sprite = shiftDisplayIcon;
                    m_IconComponent.enabled = shiftDisplayIcon != null;
                }

                RefreshDisplayCharacter();
            }
        }

        [SerializeField, Tooltip("If true, the key pressed event will fire on button down. If false, the key pressed event will fire on On Click.")]
        bool m_UpdateOnKeyDown;

        /// <summary>
        /// If true, key pressed will fire on button down instead of on button up.
        /// </summary>
        public bool updateOnKeyDown
        {
            get => m_UpdateOnKeyDown;
            set => m_UpdateOnKeyDown = value;
        }

        [SerializeField, Tooltip("Text field used to display key character.")]
        TMP_Text m_TextComponent;

        /// <summary>
        /// Text field used to display key character.
        /// </summary>
        public TMP_Text textComponent
        {
            get => m_TextComponent;
            set => m_TextComponent = value;
        }

        [SerializeField, Tooltip("Image component used to display icons for key.")]
        Image m_IconComponent;

        /// <summary>
        /// Image component used to display icons for key.
        /// </summary>
        public Image iconComponent
        {
            get => m_IconComponent;
            set => m_IconComponent = value;
        }

        [SerializeField, Tooltip("Image component used to highlight key indicating an active state.")]
        Image m_HighlightComponent;

        /// <summary>
        /// Image component used to highlight key indicating an active state.
        /// </summary>
        public Image highlightComponent
        {
            get => m_HighlightComponent;
            set => m_HighlightComponent = value;
        }

        [SerializeField, Tooltip("(Optional) Audio source played when key is pressed.")]
        AudioSource m_AudioSource;

        /// <summary>
        /// (Optional) Audio source played when key is pressed.
        /// </summary>
        public AudioSource audioSource
        {
            get => m_AudioSource;
            set => m_AudioSource = value;
        }

        XRKeyboard m_Keyboard;

        float m_LastClickTime;
        bool m_Shifted;

        /// <summary>
        /// True if this key is in a shifted state, otherwise returns false.
        /// </summary>
        public bool shifted
        {
            get => m_Shifted;
            set
            {
                m_Shifted = value;
                RefreshDisplayCharacter();
            }
        }

        /// <summary>
        /// Time the key was last pressed.
        /// </summary>
        public float lastClickTime => m_LastClickTime;

        /// <summary>
        /// Time since the key was last pressed.
        /// </summary>
        public float timeSinceLastClick => Time.time - m_LastClickTime;

        /// <summary>
        /// The keyboard associated with this key.
        /// </summary>
        public XRKeyboard keyboard
        {
            get => m_Keyboard;
            set
            {
                if (m_Keyboard != null)
                {
                    m_Keyboard.onShifted.RemoveListener(OnKeyboardShift);
                    m_Keyboard.onLayoutChanged.RemoveListener(OnKeyboardLayoutChange);
                }

                m_Keyboard = value;

                if (m_Keyboard != null)
                {
                    m_Keyboard.onShifted.AddListener(OnKeyboardShift);
                    m_Keyboard.onLayoutChanged.AddListener(OnKeyboardLayoutChange);
                }
            }
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            RefreshDisplayCharacter();
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_Keyboard != null)
            {
                m_Keyboard.onShifted.RemoveListener(OnKeyboardShift);
                m_Keyboard.onLayoutChanged.RemoveListener(OnKeyboardLayoutChange);
            }
        }

        /// <inheritdoc />
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (m_UpdateOnKeyDown && interactable)
                KeyClick();
        }

        /// <inheritdoc />
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (!m_UpdateOnKeyDown)
                KeyClick();
        }

        protected virtual void KeyClick()
        {
            // Local function of things to do to the key when pressed (Audio, etc.)
            KeyPressed();

            if (m_KeyFunction != null)
            {
                m_KeyFunction.PreprocessKey(m_Keyboard, this);
                m_KeyFunction.ProcessKey(m_Keyboard, this);
                m_KeyFunction.PostprocessKey(m_Keyboard, this);
            }
            else
            {
                // Fallback if key function is null
                m_Keyboard.PreprocessKeyPress(this);
                m_Keyboard.TryProcessKeyPress(this);
                m_Keyboard.PostprocessKeyPress(this);
            }

            m_LastClickTime = Time.time;
        }

        /// <summary>
        /// Local handling of this key being pressed.
        /// </summary>
        protected virtual void KeyPressed()
        {
            if (m_AudioSource != null)
            {
                if (m_AudioSource.isPlaying)
                    m_AudioSource.Stop();

                float pitchVariance = Random.Range(0.95f, 1.05f);
                m_AudioSource.pitch = pitchVariance;
                m_AudioSource.Play();
            }
        }

        protected virtual void OnKeyboardShift(KeyboardModifiersEventArgs args)
        {
            shifted = args.shiftValue;
        }

        protected virtual void OnKeyboardLayoutChange(KeyboardLayoutEventArgs args)
        {
            var enableHighlight = args.layout == m_Character && !m_HighlightComponent.enabled;
            EnableHighlight(enableHighlight);
        }

        /// <summary>
        /// Enables or disables the key highlight image.
        /// </summary>
        /// <param name="enable">If true, the highlight image is enabled. If false, the highlight image is disabled.</param>
        public void EnableHighlight(bool enable)
        {
            if (m_HighlightComponent != null)
            {
                m_HighlightComponent.enabled = enable;
            }
        }

        // Helper functions
        protected void RefreshDisplayCharacter()
        {
            if (m_KeyFunction != null && m_Keyboard != null)
                m_KeyFunction.ProcessRefreshDisplay(m_Keyboard, this);

            if (m_IconComponent != null)
            {
                m_IconComponent.sprite = GetEffectiveDisplayIcon();
                if (m_IconComponent.sprite != null)
                {
                    m_TextComponent.enabled = false;
                    m_IconComponent.enabled = true;
                    return;
                }
            }

            if (m_TextComponent != null)
            {
                m_TextComponent.text = GetEffectiveDisplayCharacter();
                m_TextComponent.enabled = true;
                m_IconComponent.enabled = false;
            }
        }

        protected virtual string GetEffectiveDisplayCharacter()
        {
            // If we've got a display character, prioritize that.
            string value;
            if (!string.IsNullOrEmpty(m_DisplayCharacter))
                value = m_DisplayCharacter;
            else if (!string.IsNullOrEmpty(m_Character))
                value = m_Character;
            else
                value = string.Empty;

            // If we're in shift mode, check our shift overrides.
            if (m_Shifted)
            {
                if (!string.IsNullOrEmpty(m_ShiftDisplayCharacter))
                    value = m_ShiftDisplayCharacter;
                else if (!string.IsNullOrEmpty(m_ShiftCharacter))
                    value = m_ShiftCharacter;
                else
                    value = value.ToUpper();
            }

            return value;
        }

        protected virtual Sprite GetEffectiveDisplayIcon()
        {
            if (m_KeyFunction != null && m_Keyboard != null && m_KeyFunction.OverrideDisplayIcon(m_Keyboard, this))
                return m_KeyFunction.GetDisplayIcon(m_Keyboard, this);

            return m_Shifted ? m_ShiftDisplayIcon : m_DisplayIcon;
        }

        /// <summary>
        /// Helper function that returns the current effective character for this key based on shifted state.
        /// </summary>
        /// <returns>Returns the <see cref="shiftCharacter"/> when this key is in the shifted state or <see cref="character"/>
        /// when this key is not shifted.</returns>
        public virtual string GetEffectiveCharacter()
        {
            if (m_Shifted)
            {
                if (!string.IsNullOrEmpty(m_ShiftCharacter))
                    return m_ShiftCharacter;

                return m_Character.ToUpper();
            }

            return m_Character;
        }

        /// <summary>
        /// Enables or disables the key button being interactable. The icon and text alpha will be adjusted to reflect
        /// the state of the button.
        /// </summary>
        /// <param name="enable">The desired interactable state of the key.</param>
        public virtual void SetButtonInteractable(bool enable)
        {
            const float enabledAlpha = 1f;
            const float disabledAlpha = 0.25f;

            interactable = enable;
            if (m_TextComponent != null)
            {
                m_TextComponent.alpha = enable ? enabledAlpha : disabledAlpha;
            }

            if (m_IconComponent != null)
            {
                var c = m_IconComponent.color;
                c.a = enable ? enabledAlpha : disabledAlpha;
                m_IconComponent.color = c;
            }
        }
    }
}
#endif
