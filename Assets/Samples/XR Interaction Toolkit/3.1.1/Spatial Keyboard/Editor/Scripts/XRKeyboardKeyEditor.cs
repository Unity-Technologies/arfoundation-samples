#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

namespace UnityEditor.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// Custom editor for an <see cref="XRKeyboardKey"/>.
    /// </summary>
    [CustomEditor(typeof(XRKeyboardKey), true), CanEditMultipleObjects]
    public class XRKeyboardKeyEditor : ButtonEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.keyFunction"/>.</summary>
        protected SerializedProperty m_KeyFunction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.keyCode"/>.</summary>
        protected SerializedProperty m_KeyCode;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.character"/>.</summary>
        protected SerializedProperty m_Character;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.displayCharacter"/>.</summary>
        protected SerializedProperty m_DisplayCharacter;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.displayIcon"/>.</summary>
        protected SerializedProperty m_DisplayIcon;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.shiftCharacter"/>.</summary>
        protected SerializedProperty m_ShiftCharacter;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.shiftDisplayCharacter"/>.</summary>
        protected SerializedProperty m_ShiftDisplayCharacter;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.shiftDisplayIcon"/>.</summary>
        protected SerializedProperty m_ShiftDisplayIcon;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.updateOnKeyDown"/>.</summary>
        protected SerializedProperty m_UpdateOnKeyDown;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.textComponent"/>.</summary>
        protected SerializedProperty m_TextComponent;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.audioSource"/>.</summary>
        protected SerializedProperty m_AudioSource;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.highlightComponent"/>.</summary>
        protected SerializedProperty m_HighlightComponent;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardKey.iconComponent"/>.</summary>
        protected SerializedProperty m_IconComponent;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            public static readonly GUIContent keyFunction = EditorGUIUtility.TrTextContent("Key Function", "KeyFunction used for this key. The FunctionCallBack will be called on key press and used to communicate with the keyboard.");
            public static readonly GUIContent keyCode = EditorGUIUtility.TrTextContent("Key Code", "(Optional) KeyCode used for this key. Used in conjunction with KeyCodeFunction or as a fallback for standard commands.");
            public static readonly GUIContent character = EditorGUIUtility.TrTextContent("Character", "Character for this key in non-shifted state. This string will be passed to the keyboard and appended to the keyboard text string or processed as a keyboard command.");
            public static readonly GUIContent displayCharacter = EditorGUIUtility.TrTextContent("Display Character", "Display character for this key in a non-shifted state. This string will be displayed on the key text field. If empty, character will be used as a fall back.");
            public static readonly GUIContent displayIcon = EditorGUIUtility.TrTextContent("Display Icon", "Display icon for this key in a non-shifted state. This icon will be displayed on the key image field. If empty, the display character or character will be used as a fall back.");
            public static readonly GUIContent shiftCharacter = EditorGUIUtility.TrTextContent("Shift Character", "Character for this key in a shifted state. This string will be passed to the keyboard and appended to the keyboard text string or processed as a keyboard command.");
            public static readonly GUIContent shiftDisplayCharacter = EditorGUIUtility.TrTextContent("Shift Display Character", "Display character for this key in a shifted state. This string will be displayed on the key text field. If empty, shift character will be used as a fall back.");
            public static readonly GUIContent shiftDisplayIcon = EditorGUIUtility.TrTextContent("Shift Display Icon", "Display icon for this key in a shifted state. This icon will be displayed on the key image field. If empty, the shift display character or shift character will be used as a fall back.");

            public static readonly GUIContent updateOnDown = EditorGUIUtility.TrTextContent("Update on key down", "If true, the key pressed event will fire on button down. If false, the key pressed event will fire on OnClick.");
            public static readonly GUIContent textComponent = EditorGUIUtility.TrTextContent("Text Component", "Text field used to display key character.");
            public static readonly GUIContent audioSource = EditorGUIUtility.TrTextContent("Audio Source", "(Optional) Audio source played when key is pressed.");
            public static readonly GUIContent highlightComponent = EditorGUIUtility.TrTextContent("Highlight Component", "(Optional) Image used to highlight key indicating and active state.");
            public static readonly GUIContent iconComponent = EditorGUIUtility.TrTextContent("Icon Component", "(Optional) Image used for key icon, used as an alternative to a character.");
            public static readonly GUIContent buttonSettings = EditorGUIUtility.TrTextContent("Button Settings", "Settings for the keyboard key button.");
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            m_KeyFunction = serializedObject.FindProperty("m_KeyFunction");
            m_KeyCode = serializedObject.FindProperty("m_KeyCode");
            m_Character = serializedObject.FindProperty("m_Character");
            m_DisplayCharacter = serializedObject.FindProperty("m_DisplayCharacter");
            m_DisplayIcon = serializedObject.FindProperty("m_DisplayIcon");
            m_ShiftCharacter = serializedObject.FindProperty("m_ShiftCharacter");
            m_ShiftDisplayCharacter = serializedObject.FindProperty("m_ShiftDisplayCharacter");
            m_ShiftDisplayIcon = serializedObject.FindProperty("m_ShiftDisplayIcon");
            m_UpdateOnKeyDown = serializedObject.FindProperty("m_UpdateOnKeyDown");
            m_TextComponent = serializedObject.FindProperty("m_TextComponent");
            m_AudioSource = serializedObject.FindProperty("m_AudioSource");
            m_HighlightComponent = serializedObject.FindProperty("m_HighlightComponent");
            m_IconComponent = serializedObject.FindProperty("m_IconComponent");
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawCharacterSettings();
            DrawDisplaySettings();
            DrawFunctionSettings();
            DrawComponentReferences();

            // Draw basic key settings
            EditorGUILayout.PropertyField(m_UpdateOnKeyDown, Contents.updateOnDown);

            // Draw button settings if that section is expanded
            m_UpdateOnKeyDown.isExpanded = EditorGUILayout.Foldout(m_UpdateOnKeyDown.isExpanded, Contents.buttonSettings, toggleOnLabelClick: true);
            if (m_UpdateOnKeyDown.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    base.OnInspectorGUI();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawCharacterSettings()
        {
            EditorGUILayout.LabelField("Character Settings", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_Character, Contents.character);
                EditorGUILayout.PropertyField(m_ShiftCharacter, Contents.shiftCharacter);
            }
        }

        void DrawDisplaySettings()
        {
            EditorGUILayout.LabelField("Display Settings", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_DisplayCharacter, Contents.displayCharacter);
                EditorGUILayout.PropertyField(m_ShiftDisplayCharacter, Contents.shiftDisplayCharacter);
                EditorGUILayout.PropertyField(m_DisplayIcon, Contents.displayIcon);
                EditorGUILayout.PropertyField(m_ShiftDisplayIcon, Contents.shiftDisplayIcon);
            }
        }

        void DrawFunctionSettings()
        {
            EditorGUILayout.LabelField("Function Settings", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_KeyFunction, Contents.keyFunction);
                EditorGUILayout.PropertyField(m_KeyCode, Contents.keyCode);
            }
        }

        void DrawComponentReferences()
        {
            EditorGUILayout.LabelField("Component References", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_TextComponent, Contents.textComponent);
                EditorGUILayout.PropertyField(m_IconComponent, Contents.iconComponent);
                EditorGUILayout.PropertyField(m_HighlightComponent, Contents.highlightComponent);
                EditorGUILayout.PropertyField(m_AudioSource, Contents.audioSource);
            }
        }
    }
}
#endif
