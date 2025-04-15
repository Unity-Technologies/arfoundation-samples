#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// Scriptable object that defines key mappings to support swapping <see cref="XRKeyboardLayout"/>. There should be one
    /// instance of the <see cref="XRKeyboardConfig"/> for each layout (i.e. alphanumeric, symbols, etc.).
    /// </summary>
    public class XRKeyboardConfig : ScriptableObject
    {
        /// <summary>
        /// Class representing the data needed to populate keys.
        /// </summary>
        [Serializable]
        public class KeyMapping
        {
            [SerializeField]
            string m_Character;

            /// <summary>
            /// Character for this key in non-shifted state. This string will be passed to the keyboard and appended to the keyboard text string or processed as a keyboard command.
            /// </summary>
            public string character
            {
                get => m_Character;
                set => m_Character = value;
            }

            [SerializeField]
            string m_DisplayCharacter;

            /// <summary>
            /// Display character for this key in a non-shifted state. This string will be displayed on the key text field.
            /// If empty, character will be used as a fallback.
            /// </summary>
            public string displayCharacter
            {
                get => m_DisplayCharacter;
                set => m_DisplayCharacter = value;
            }

            [SerializeField]
            Sprite m_DisplayIcon;

            /// <summary>
            /// Display icon for this key in a non-shifted state. This icon will be displayed on the key image field.
            /// If empty, the display character or character will be used as a fallback.
            /// </summary>
            public Sprite displayIcon
            {
                get => m_DisplayIcon;
                set => m_DisplayIcon = value;
            }

            [SerializeField]
            string m_ShiftCharacter;

            /// <summary>
            /// Character for this key in a shifted state. This string will be passed to the keyboard and appended to
            /// the keyboard text string or processed as a keyboard command.
            /// </summary>
            public string shiftCharacter
            {
                get => m_ShiftCharacter;
                set => m_ShiftCharacter = value;
            }

            [SerializeField]
            string m_ShiftDisplayCharacter;

            /// <summary>
            /// Display character for this key in a shifted state. This string will be displayed on the key
            /// text field. If empty, shift character will be used as a fallback.
            /// </summary>
            public string shiftDisplayCharacter
            {
                get => m_ShiftDisplayCharacter;
                set => m_ShiftDisplayCharacter = value;
            }

            [SerializeField]
            Sprite m_ShiftDisplayIcon;

            /// <summary>
            /// Display icon for this key in a shifted state. This icon will be displayed on the key image field.
            /// If empty, the shift display character or shift character will be used as a fallback.
            /// </summary>
            public Sprite shiftDisplayIcon
            {
                get => m_ShiftDisplayIcon;
                set => m_ShiftDisplayIcon = value;
            }

            [SerializeField]
            bool m_OverrideDefaultKeyFunction;

            /// <summary>
            /// If true, this will expose a key function property to override the default key function of this config.
            /// </summary>
            public bool overrideDefaultKeyFunction
            {
                get => m_OverrideDefaultKeyFunction;
                set => m_OverrideDefaultKeyFunction = value;
            }

            [SerializeField]
            KeyFunction m_KeyFunction;

            /// <summary>
            /// <see cref="KeyFunction"/> used for this key. The function callback will be called on key press
            /// and used to communicate with the keyboard API.
            /// </summary>
            public KeyFunction keyFunction
            {
                get => m_KeyFunction;
                set => m_KeyFunction = value;
            }

            [SerializeField]
            KeyCode m_KeyCode;

            /// <summary>
            /// (Optional) <see cref="KeyCode"/> used for this key. Used with <see cref="keyFunction"/> to
            /// support already defined KeyCode values.
            /// </summary>
            public KeyCode keyCode
            {
                get => m_KeyCode;
                set => m_KeyCode = value;
            }

            [SerializeField]
            bool m_Disabled;

            /// <summary>
            /// If true, the key button interactable property will be set to false.
            /// </summary>
            public bool disabled
            {
                get => m_Disabled;
                set => m_Disabled = value;
            }
        }

        [SerializeField, Tooltip("Default key function for each key in this mapping.")]
        KeyFunction m_DefaultKeyFunction;

        /// <summary>
        /// Default key function for each key in this mapping.
        /// </summary>
        /// <remarks>This is a utility feature that reduces the authoring needed when most key mappings share the same
        /// functionality (i.e. value keys that append characters).</remarks>
        public KeyFunction defaultKeyFunction
        {
            get => m_DefaultKeyFunction;
            set => m_DefaultKeyFunction = value;
        }

        /// <summary>
        /// List of each key mapping in this layout.
        /// </summary>
        [SerializeField, Tooltip("List of each key mapping in this layout.")]
        List<KeyMapping> m_KeyMappings;

        /// <summary>
        /// List of each key mapping in this layout.
        /// </summary>
        public List<KeyMapping> keyMappings
        {
            get => m_KeyMappings;
            set => m_KeyMappings = value;
        }
    }
}
#endif
