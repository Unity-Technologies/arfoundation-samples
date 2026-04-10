#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Pool;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// Virtual spatial keyboard.
    /// </summary>
    public class XRKeyboard : MonoBehaviour
    {
        /// <summary>
        /// Layout this keyboard is able to switch to with the corresponding layout command.
        /// </summary>
        /// <seealso cref="subsetLayout"/>
        [Serializable]
        public struct SubsetMapping
        {
            [SerializeField, Tooltip("This drives what GameObject layout is displayed.")]
            string m_LayoutString;

            /// <summary>
            /// This drives what GameObject layout is displayed.
            /// </summary>
            public string layoutString
            {
                get => m_LayoutString;
                set => m_LayoutString = value;
            }

            [SerializeField, Tooltip("GameObject root of the layout which contains the set of keys.")]
            XRKeyboardLayout m_LayoutRoot;

            /// <summary>
            /// GameObject root of the layout which contains the set of keys.
            /// </summary>
            public XRKeyboardLayout layoutRoot
            {
                get => m_LayoutRoot;
                set => m_LayoutRoot = value;
            }

            [SerializeField, Tooltip("Config asset which contains the key definitions for the layout when this is turned on.")]
            XRKeyboardConfig m_ToggleOnConfig;

            /// <summary>
            /// Config asset which contains the key definitions for the layout when this is turned on.
            /// </summary>
            public XRKeyboardConfig toggleOnConfig
            {
                get => m_ToggleOnConfig;
                set => m_ToggleOnConfig = value;
            }

            [SerializeField, Tooltip("Config asset which is the default config when this is turned off.")]
            XRKeyboardConfig m_ToggleOffConfig;

            /// <summary>
            /// Config asset which is the default config when this is turned off.
            /// </summary>
            public XRKeyboardConfig toggleOffConfig
            {
                get => m_ToggleOffConfig;
                set => m_ToggleOffConfig = value;
            }
        }

        [SerializeField, HideInInspector]
        string m_Text = string.Empty;

        /// <summary>
        /// String of text currently in the keyboard. Setter invokes <see cref="onTextUpdated"/> when updated.
        /// </summary>
        public string text
        {
            get => m_Text;
            protected set
            {
                if (m_Text != value)
                {
                    m_Text = value;
                    caretPosition = Math.Clamp(caretPosition, 0, m_Text.Length);
                    using (m_KeyboardTextEventArgs.Get(out var args))
                    {
                        args.keyboard = this;
                        args.keyboardText = text;
                        onTextUpdated?.Invoke(args);
                    }
                }
            }
        }

        [SerializeField, HideInInspector]
        TMP_InputField m_CurrentInputField;

        /// <summary>
        /// Current input field this keyboard is observing.
        /// </summary>
        protected TMP_InputField currentInputField
        {
            get => m_CurrentInputField;
            set
            {
                if (m_CurrentInputField == value)
                    return;

                StopObservingInputField(m_CurrentInputField);
                m_CurrentInputField = value;
                StartObservingInputField(m_CurrentInputField);

                using (m_KeyboardTextEventArgs.Get(out var args))
                {
                    args.keyboard = this;
                    args.keyboardText = text;
                    onFocusChanged?.Invoke(args);
                }
            }
        }

        [SerializeField]
        KeyboardTextEvent m_OnTextSubmitted = new KeyboardTextEvent();

        /// <summary>
        /// Event invoked when keyboard submits text.
        /// </summary>
        public KeyboardTextEvent onTextSubmitted
        {
            get => m_OnTextSubmitted;
            set => m_OnTextSubmitted = value;
        }

        [SerializeField]
        KeyboardTextEvent m_OnTextUpdated = new KeyboardTextEvent();

        /// <summary>
        /// Event invoked when keyboard text is updated.
        /// </summary>
        public KeyboardTextEvent onTextUpdated
        {
            get => m_OnTextUpdated;
            set => m_OnTextUpdated = value;
        }

        [SerializeField]
        KeyboardKeyEvent m_OnKeyPressed = new KeyboardKeyEvent();

        /// <summary>
        /// Event invoked after a key is pressed.
        /// </summary>
        public KeyboardKeyEvent onKeyPressed
        {
            get => m_OnKeyPressed;
            set => m_OnKeyPressed = value;
        }

        [SerializeField]
        KeyboardModifiersEvent m_OnShifted = new KeyboardModifiersEvent();

        /// <summary>
        /// Event invoked after keyboard shift is changed. These event args also contain the value for the caps lock state.
        /// </summary>
        public KeyboardModifiersEvent onShifted
        {
            get => m_OnShifted;
            set => m_OnShifted = value;
        }

        [SerializeField]
        KeyboardLayoutEvent m_OnLayoutChanged = new KeyboardLayoutEvent();

        /// <summary>
        /// Event invoked when keyboard layout is changed.
        /// </summary>
        public KeyboardLayoutEvent onLayoutChanged
        {
            get => m_OnLayoutChanged;
            set => m_OnLayoutChanged = value;
        }

        [SerializeField]
        KeyboardTextEvent m_OnOpened = new KeyboardTextEvent();

        /// <summary>
        /// Event invoked when the keyboard is opened.
        /// </summary>
        public KeyboardTextEvent onOpened
        {
            get => m_OnOpened;
            set => m_OnOpened = value;
        }

        [SerializeField]
        KeyboardTextEvent m_OnClosed;

        /// <summary>
        /// Event invoked after the keyboard is closed.
        /// </summary>
        public KeyboardTextEvent onClosed
        {
            get => m_OnClosed;
            set => m_OnClosed = value;
        }

        [SerializeField]
        KeyboardTextEvent m_OnFocusChanged = new KeyboardTextEvent();

        /// <summary>
        /// Event invoked when the keyboard changes or gains input field focus.
        /// </summary>
        public KeyboardTextEvent onFocusChanged
        {
            get => m_OnFocusChanged;
            set => m_OnFocusChanged = value;
        }

        [SerializeField]
        KeyboardEvent m_OnCharacterLimitReached = new KeyboardEvent();

        /// <summary>
        /// Event invoked when the keyboard tries to update text, but the character of the input field is reached.
        /// </summary>
        public KeyboardEvent onCharacterLimitReached
        {
            get => m_OnCharacterLimitReached;
            set => m_OnCharacterLimitReached = value;
        }

        [SerializeField]
        bool m_SubmitOnEnter = true;

        /// <summary>
        /// If true, <see cref="onTextSubmitted"/> will be invoked when the keyboard receives a return or enter command. Otherwise,
        /// it will treat return or enter as a newline.
        /// </summary>
        public bool submitOnEnter
        {
            get => m_SubmitOnEnter;
            set => m_SubmitOnEnter = value;
        }

        [SerializeField]
        bool m_CloseOnSubmit;

        /// <summary>
        /// If true, keyboard will close on enter or return command.
        /// </summary>
        public bool closeOnSubmit
        {
            get => m_CloseOnSubmit;
            set => m_CloseOnSubmit = value;
        }

        [SerializeField]
        float m_DoubleClickInterval = 2f;

        /// <summary>
        /// Interval in which a key pressed twice would be considered a double click.
        /// </summary>
        public float doubleClickInterval
        {
            get => m_DoubleClickInterval;
            set => m_DoubleClickInterval = value;
        }

        [SerializeField]
        List<SubsetMapping> m_SubsetLayout;

        /// <summary>
        /// List of layouts this keyboard is able to switch between given the corresponding layout command.
        /// </summary>
        /// <remarks>This supports multiple layout roots updating with the same <see cref="SubsetMapping.layoutString"/>.</remarks>
        public List<SubsetMapping> subsetLayout
        {
            get => m_SubsetLayout;
            set => m_SubsetLayout = value;
        }

        /// <summary>
        /// List of keys associated with this keyboard.
        /// </summary>
        public List<XRKeyboardKey> keys { get; set; }

        int m_CaretPosition;

        /// <summary>
        /// Caret index of this keyboard.
        /// </summary>
        public int caretPosition
        {
            get => m_CaretPosition;
            protected set => m_CaretPosition = value;
        }

        bool m_Shifted;

        /// <summary>
        /// (Read Only) Gets the shift state of the keyboard.
        /// </summary>
        public bool shifted => m_Shifted;

        bool m_CapsLocked;

        /// <summary>
        /// (Read Only) Gets the caps lock state of the keyboard.
        /// </summary>
        public bool capsLocked => m_CapsLocked;

        bool m_IsOpen;

        /// <summary>
        /// Returns true if the keyboard has been opened with the open function and the keyboard is active and enabled, otherwise returns false.
        /// </summary>
        public bool isOpen => (m_IsOpen && isActiveAndEnabled);

        Dictionary<string, List<SubsetMapping>> m_SubsetLayoutMap;
        HashSet<XRKeyboardLayout> m_KeyboardLayouts;

        // Reusable event args
        readonly LinkedPool<KeyboardTextEventArgs> m_KeyboardTextEventArgs = new LinkedPool<KeyboardTextEventArgs>(() => new KeyboardTextEventArgs(), collectionCheck: false);
        readonly LinkedPool<KeyboardLayoutEventArgs> m_KeyboardLayoutEventArgs = new LinkedPool<KeyboardLayoutEventArgs>(() => new KeyboardLayoutEventArgs(), collectionCheck: false);
        readonly LinkedPool<KeyboardModifiersEventArgs> m_KeyboardModifiersEventArgs = new LinkedPool<KeyboardModifiersEventArgs>(() => new KeyboardModifiersEventArgs(), collectionCheck: false);
        readonly LinkedPool<KeyboardKeyEventArgs> m_KeyboardKeyEventArgs = new LinkedPool<KeyboardKeyEventArgs>(() => new KeyboardKeyEventArgs(), collectionCheck: false);
        readonly LinkedPool<KeyboardBaseEventArgs> m_KeyboardBaseEventArgs = new LinkedPool<KeyboardBaseEventArgs>(() => new KeyboardBaseEventArgs(), collectionCheck: false);

        int m_CharacterLimit = -1;
        bool m_MonitorCharacterLimit;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Awake()
        {
            m_SubsetLayoutMap = new Dictionary<string, List<SubsetMapping>>();
            m_KeyboardLayouts = new HashSet<XRKeyboardLayout>();

            foreach (var subsetMapping in m_SubsetLayout)
            {
                if (m_SubsetLayoutMap.TryGetValue(subsetMapping.layoutString, out var subsetMappings))
                    subsetMappings.Add(subsetMapping);
                else
                    m_SubsetLayoutMap[subsetMapping.layoutString] = new List<SubsetMapping> { subsetMapping };

                m_KeyboardLayouts.Add(subsetMapping.layoutRoot);
            }

            keys = new List<XRKeyboardKey>();
            GetComponentsInChildren(true, keys);
            keys.ForEach(key => key.keyboard = this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDisable()
        {
            // Reset if this component is turned off without first calling close function
            m_IsOpen = false;
        }

        /// <summary>
        /// Processes a <see cref="KeyCode"/>.
        /// </summary>
        /// <param name="keyCode">Key code to process.</param>
        /// <returns>True on supported KeyCode.</returns>
        /// <remarks>
        /// Override this method to add support for additional <see cref="KeyCode"/>.
        /// </remarks>
        public virtual bool ProcessKeyCode(KeyCode keyCode)
        {
            var success = true;
            switch (keyCode)
            {
                case KeyCode.LeftShift:
                case KeyCode.RightShift:
                    Shift(!m_Shifted);
                    break;
                case KeyCode.CapsLock:
                    CapsLock(!m_CapsLocked);
                    break;
                case KeyCode.Backspace:
                    Backspace();
                    break;
                case KeyCode.Delete:
                    Delete();
                    break;
                case KeyCode.Clear:
                    Clear();
                    break;
                case KeyCode.Space:
                    UpdateText(" ");
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    if (submitOnEnter)
                    {
                        Submit();
                    }
                    else
                    {
                        UpdateText("\n");
                    }

                    break;
                default:
                    success = false;
                    break;
            }

            return success;
        }

        /// <summary>
        /// Attempts to process the key based on the key's character. Used as a fallback when KeyFunction is
        /// empty on the key.
        /// </summary>
        /// <param name="key">Key to attempt to process</param>
        public virtual void TryProcessKeyPress(XRKeyboardKey key)
        {
            if (key == null || !ReferenceEquals(key.keyboard, this))
                return;

            // Process key stroke
            if (onKeyPressed != null)
            {
                // Try to process key code
                if (ProcessKeyCode(key.keyCode))
                    return;

                var keyPress = key.GetEffectiveCharacter();

                // Monitor for subset change
                if (UpdateLayout(keyPress))
                    return;

                switch (keyPress)
                {
                    case "\\s":
                        // Shift
                        Shift(!m_Shifted);
                        break;
                    case "\\caps":
                        CapsLock(!m_CapsLocked);
                        break;
                    case "\\b":
                        // Backspace
                        Backspace();
                        break;
                    case "\\c":
                        // cancel
                        break;
                    case "\\r" when submitOnEnter:
                        {
                            Submit();
                            break;
                        }
                    case "\\cl":
                        // Clear
                        Clear();
                        break;
                    case "\\h":
                        // Hide
                        Close();
                        break;
                    default:
                        {
                            UpdateText(keyPress);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Pre-process function when a key is pressed.
        /// </summary>
        /// <param name="key">Key that is about to process.</param>
        public virtual void PreprocessKeyPress(XRKeyboardKey key)
        {
        }

        /// <summary>
        /// Post-process function when a key is pressed.
        /// </summary>
        /// <param name="key">Key that has just been processed.</param>
        public virtual void PostprocessKeyPress(XRKeyboardKey key)
        {
            using (m_KeyboardKeyEventArgs.Get(out var args))
            {
                args.keyboard = this;
                args.key = key;
                onKeyPressed.Invoke(args);
            }
        }

#region Process Key Functions

        /// <summary>
        /// Updates the keyboard text by inserting the <see cref="newText"/> string into the existing <see cref="text"/>.
        /// </summary>
        /// <param name="newText">The new text to insert into the current keyboard text.</param>
        /// <remarks>If the keyboard is set to monitor the input field's character limit, the keyboard will ensure
        /// the text does not exceed the <see cref="TMP_InputField.characterLimit"/>.</remarks>
        public virtual void UpdateText(string newText)
        {
            // Attempt to add key press to current text
            var updatedText = text;

            updatedText = updatedText.Insert(caretPosition, newText);

            var isUpdatedTextWithinLimits = !m_MonitorCharacterLimit || updatedText.Length <= m_CharacterLimit;
            if (isUpdatedTextWithinLimits)
            {
                caretPosition += newText.Length;
                text = updatedText;
            }
            else
            {
                using (m_KeyboardBaseEventArgs.Get(out var args))
                {
                    args.keyboard = this;
                    onCharacterLimitReached?.Invoke(args);
                }
            }

            // Turn off shift after typing a letter
            if (m_Shifted && !m_CapsLocked)
                Shift(!m_Shifted);
        }

        /// <summary>
        /// Process shift command for keyboard.
        /// </summary>
        public virtual void Shift(bool shiftValue)
        {
            m_Shifted = shiftValue;
            using (m_KeyboardModifiersEventArgs.Get(out var args))
            {
                args.keyboard = this;
                args.shiftValue = m_Shifted;
                args.capsLockValue = m_CapsLocked;
                onShifted.Invoke(args);
            }
        }

        /// <summary>
        /// Process caps lock command for keyboard.
        /// </summary>
        public virtual void CapsLock(bool capsLockValue)
        {
            m_CapsLocked = capsLockValue;
            Shift(capsLockValue);
        }

        /// <summary>
        /// Process backspace command for keyboard.
        /// </summary>
        public virtual void Backspace()
        {
            if (caretPosition > 0)
            {
                --caretPosition;
                text = text.Remove(caretPosition, 1);
            }
        }

        /// <summary>
        /// Process delete command for keyboard and deletes one character.
        /// </summary>
        public virtual void Delete()
        {
            if (caretPosition < text.Length)
            {
                text = text.Remove(caretPosition, 1);
            }
        }

        /// <summary>
        /// Invokes <see cref="onTextSubmitted"/> event and closes keyboard if <see cref="closeOnSubmit"/> is true.
        /// </summary>
        public virtual void Submit()
        {
            using (m_KeyboardTextEventArgs.Get(out var args))
            {
                args.keyboard = this;
                args.keyboardText = text;
                onTextSubmitted?.Invoke(args);
            }

            if (closeOnSubmit)
                Close(false);
        }

        /// <summary>
        /// Clears text to an empty string.
        /// </summary>
        public virtual void Clear()
        {
            text = string.Empty;
            caretPosition = text.Length;
        }

        /// <summary>
        /// Looks up the <see cref="SubsetMapping"/> associated with the <see cref="layoutKey"/> and updates the
        /// <see cref="XRKeyboardLayout"/> on the <see cref="SubsetMapping.layoutRoot"/>.  If the
        /// <see cref="XRKeyboardLayout.activeKeyMapping"/> is already <see cref="SubsetMapping.toggleOnConfig"/>,
        /// <see cref="SubsetMapping.toggleOffConfig"/> will be set as the active key mapping.
        /// </summary>
        /// <param name="layoutKey">The string of the new layout as it is registered in the <see cref="subsetLayout"/>.</param>
        /// <returns>Returns true if the layout was successfully found and changed.</returns>
        /// <remarks>By default, shift or caps lock will be turned off on layout change.</remarks>
        public virtual bool UpdateLayout(string layoutKey)
        {
            if (m_SubsetLayoutMap.TryGetValue(layoutKey, out var subsetMappings))
            {
                foreach (var subsetMapping in subsetMappings)
                {
                    var layout = subsetMapping.layoutRoot;
                    layout.activeKeyMapping = layout.activeKeyMapping != subsetMapping.toggleOnConfig ? subsetMapping.toggleOnConfig : subsetMapping.toggleOffConfig;
                }

                if (m_Shifted || m_CapsLocked)
                    CapsLock(false);

                using (m_KeyboardLayoutEventArgs.Get(out var args))
                {
                    args.keyboard = this;
                    args.layout = layoutKey;
                    onLayoutChanged.Invoke(args);
                }

                return true;
            }

            return false;
        }

#endregion

#region Open Functions

        /// <summary>
        /// Opens the keyboard with a <see cref="TMP_InputField"/> parameter as the active input field.
        /// </summary>
        /// <param name="inputField">The input field opening this keyboard.</param>
        /// <param name="observeCharacterLimit">If true, keyboard will observe the character limit from the <see cref="inputField"/>.</param>
        public virtual void Open(TMP_InputField inputField, bool observeCharacterLimit = false)
        {
            currentInputField = inputField;
            m_MonitorCharacterLimit = observeCharacterLimit;
            m_CharacterLimit = observeCharacterLimit ? currentInputField.characterLimit : -1;

            Open(currentInputField.text);
        }

        /// <summary>
        /// Opens the keyboard with any existing text.
        /// </summary>
        /// <remarks>
        /// Shortcut for <c>Open(text)</c>.
        /// </remarks>
        public void Open() => Open(text);

        /// <summary>
        /// Opens the keyboard with an empty string and clear any existing text in the input field or keyboard.
        /// </summary>
        /// <remarks>
        /// Shortcut for <c>Open(string.Empty)</c>.
        /// </remarks>
        public void OpenCleared() => Open(string.Empty);

        /// <summary>
        /// Opens the keyboard with a given string to populate the keyboard text.
        /// </summary>
        /// <param name="newText">Text string to set the keyboard <see cref="text"/> to.</param>
        /// <remarks>The <see cref="onOpened"/> event is fired before the text is updating with <see cref="newText"/>
        /// to give any observers that would be listening the opportunity to close and stop observing before the text is updated.
        /// This is a common use case for any <see cref="XRKeyboardDisplay"/> utilizing the global keyboard. </remarks>
        public virtual void Open(string newText)
        {
            if (!isActiveAndEnabled)
            {
                // Fire event before updating text because any displays observing keyboards will be listening to that text change
                // This gives them the opportunity to close and stop observing before the text is updated.
                using (m_KeyboardTextEventArgs.Get(out var args))
                {
                    args.keyboard = this;
                    args.keyboardText = text;
                    onOpened?.Invoke(args);
                }
            }

            caretPosition = newText.Length;
            text = newText;
            gameObject.SetActive(true);
            m_IsOpen = true;
        }

#endregion

#region Close Functions

        /// <summary>
        /// Process close command for keyboard.
        /// </summary>
        /// <remarks>Stops observing active input field, resets variables, and hides this GameObject.</remarks>
        public virtual void Close()
        {
            // Clear any input field the keyboard is observing
            currentInputField = null;

            m_MonitorCharacterLimit = false;
            m_CharacterLimit = -1;

            if (m_Shifted || m_CapsLocked)
                CapsLock(false);

            using (m_KeyboardTextEventArgs.Get(out var args))
            {
                args.keyboard = this;
                args.keyboardText = text;
                onClosed?.Invoke(args);
            }

            gameObject.SetActive(false);
            m_IsOpen = false;
        }

        /// <summary>
        /// Process close command for keyboard. Optional overload for clearing text and resetting layout on close.
        /// </summary>
        /// <param name="clearText">If true, text will be cleared upon keyboard closing. This will happen after the
        /// <see cref="onClosed"/> event is fired so the observers have time to stop listening.</param>
        /// <param name="resetLayout">If true, each <see cref="XRKeyboardLayout"/> will reset to the <see cref="XRKeyboardLayout.defaultKeyMapping"/>.</param>
        /// <remarks>Please note, if <see cref="clearText"/> is true, the text will be cleared and the <see cref="onTextUpdated"/>
        /// event will be fired. This means any observers will be notified of an empty string. To avoid unwanted behavior of
        /// the text clearing, use the <see cref="onClosed"/> event to unsubscribe to the keyboard events before the text is cleared.</remarks>
        public virtual void Close(bool clearText, bool resetLayout = true)
        {
            Close();

            if (clearText)
                text = string.Empty;

            // Reset keyboard layout on close
            if (resetLayout)
            {
                // Loop through each layout root and reset to default layouts
                foreach (var layoutRoot in m_KeyboardLayouts)
                {
                    layoutRoot.SetDefaultLayout();
                }

                // Fire event of layout change to ensure highlighted buttons are reset
                using (m_KeyboardLayoutEventArgs.Get(out var args))
                {
                    args.keyboard = this;
                    args.layout = "default";
                    onLayoutChanged.Invoke(args);
                }
            }
        }

#endregion

#region Input Field Handling

        protected virtual void StopObservingInputField(TMP_InputField inputField)
        {
            if (inputField == null)
                return;

            currentInputField.onValueChanged.RemoveListener(OnInputFieldValueChange);
        }

        protected virtual void StartObservingInputField(TMP_InputField inputField)
        {
            if (inputField == null)
                return;

            currentInputField.onValueChanged.AddListener(OnInputFieldValueChange);
        }

        /// <summary>
        /// Callback method invoked when the input field's text value changes.
        /// </summary>
        /// <param name="updatedText">The text of the input field.</param>
        protected virtual void OnInputFieldValueChange(string updatedText)
        {
            caretPosition = updatedText.Length;
            text = updatedText;
        }

#endregion
    }
}
#endif
