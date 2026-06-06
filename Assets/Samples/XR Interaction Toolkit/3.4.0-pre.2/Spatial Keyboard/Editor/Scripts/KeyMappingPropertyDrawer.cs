#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

namespace UnityEditor.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    [CustomPropertyDrawer(typeof(XRKeyboardConfig.KeyMapping))]
    public class KeyMappingPropertyDrawer : PropertyDrawer
    {
        class SerializedPropertyFields
        {
            public SerializedProperty character;
            public SerializedProperty shiftCharacter;
            public SerializedProperty displayCharacter;
            public SerializedProperty shiftDisplayCharacter;
            public SerializedProperty displayIcon;
            public SerializedProperty shiftDisplayIcon;
            public SerializedProperty overrideDefaultKeyFunction;
            public SerializedProperty keyFunction;
            public SerializedProperty keyCode;
            public SerializedProperty disabled;

            public void FindProperties(SerializedProperty property)
            {
                character = property.FindPropertyRelative("m_Character");
                shiftCharacter = property.FindPropertyRelative("m_ShiftCharacter");

                displayCharacter = property.FindPropertyRelative("m_DisplayCharacter");
                shiftDisplayCharacter = property.FindPropertyRelative("m_ShiftDisplayCharacter");
                displayIcon = property.FindPropertyRelative("m_DisplayIcon");
                shiftDisplayIcon = property.FindPropertyRelative("m_ShiftDisplayIcon");

                overrideDefaultKeyFunction = property.FindPropertyRelative("m_OverrideDefaultKeyFunction");
                keyFunction = property.FindPropertyRelative("m_KeyFunction");
                keyCode = property.FindPropertyRelative("m_KeyCode");
                disabled = property.FindPropertyRelative("m_Disabled");
            }
        }

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            public static readonly GUIContent character = EditorGUIUtility.TrTextContent("Character", "Character for this key in non-shifted state. This string will be passed to the keyboard and appended to the keyboard text string or processed as a keyboard command.");
            public static readonly GUIContent shiftCharacter = EditorGUIUtility.TrTextContent("Shift Character", "Character for this key in a shifted state. This string will be passed to the keyboard and appended to the keyboard text string or processed as a keyboard command.");

            public static readonly GUIContent displayCharacter = EditorGUIUtility.TrTextContent("Display Character", "Display character for this key in a non-shifted state. This string will be displayed on the key text field. If empty, character will be used as a fallback.");
            public static readonly GUIContent shiftDisplayCharacter = EditorGUIUtility.TrTextContent("Shift Display Character", "Display character for this key in a shifted state. This string will be displayed on the key text field. If empty, shift character will be used as a fallback.");
            public static readonly GUIContent displayIcon = EditorGUIUtility.TrTextContent("Display Icon", "Display icon for this key in a non-shifted state. This icon will be displayed on the key image field. If empty, the display character or character will be used as a fallback.");
            public static readonly GUIContent shiftDisplayIcon = EditorGUIUtility.TrTextContent("Shift Display Icon", "Display icon for this key in a shifted state. This icon will be displayed on the key image field. If empty, the shift display character or shift character will be used as a fallback.");

            public static readonly GUIContent overrideDefaultKeyFunction = EditorGUIUtility.TrTextContent("Override Default Key Function", "If true, this will expose a key function property to override the default key function of this config.");
            public static readonly GUIContent keyFunction = EditorGUIUtility.TrTextContent("Key Function", "KeyFunction used for this key. The function callback will be called on key press and used to communicate with the keyboard API.");
            public static readonly GUIContent keyCode = EditorGUIUtility.TrTextContent("Key Code", "(Optional) KeyCode used for this key. Used with Key Function to support already defined KeyCode values.");
            public static readonly GUIContent disabled = EditorGUIUtility.TrTextContent("Disabled", "If true, the key button interactable property will be set to false.");
        }

        readonly SerializedPropertyFields m_Fields = new SerializedPropertyFields();

        /// <summary>
        /// See <see cref="PropertyDrawer"/>.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                // 1 Foldout header + 3 boldLabel headers + 10 or 9 PropertyField
                m_Fields.FindProperties(property);
                var numLines = m_Fields.overrideDefaultKeyFunction.boolValue ? 14 : 13;
                return EditorGUIUtility.singleLineHeight * numLines + EditorGUIUtility.standardVerticalSpacing * (numLines - 1);
            }

            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// See <see cref="PropertyDrawer"/>.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var propertyRect = position;
            propertyRect.height = EditorGUIUtility.singleLineHeight;

            m_Fields.FindProperties(property);

            property.isExpanded = EditorGUI.Foldout(propertyRect, property.isExpanded, GetPreviewString(m_Fields), true);

            // Draw expanded properties
            if (property.isExpanded)
            {
                var yDelta = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                propertyRect.y += yDelta;

                // Character settings
                EditorGUI.LabelField(propertyRect, "Character Settings", EditorStyles.boldLabel);
                propertyRect.y += yDelta;
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUI.PropertyField(propertyRect, m_Fields.character, Contents.character);
                    propertyRect.y += yDelta;
                    EditorGUI.PropertyField(propertyRect, m_Fields.shiftCharacter, Contents.shiftCharacter);
                    propertyRect.y += yDelta;
                }

                // Display settings
                EditorGUI.LabelField(propertyRect, "Display Settings", EditorStyles.boldLabel);
                propertyRect.y += yDelta;
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUI.PropertyField(propertyRect, m_Fields.displayCharacter, Contents.displayCharacter);
                    propertyRect.y += yDelta;
                    EditorGUI.PropertyField(propertyRect, m_Fields.shiftDisplayCharacter, Contents.shiftDisplayCharacter);
                    propertyRect.y += yDelta;
                    EditorGUI.PropertyField(propertyRect, m_Fields.displayIcon, Contents.displayIcon);
                    propertyRect.y += yDelta;
                    EditorGUI.PropertyField(propertyRect, m_Fields.shiftDisplayIcon, Contents.shiftDisplayIcon);
                    propertyRect.y += yDelta;
                }

                // Function settings
                EditorGUI.LabelField(propertyRect, "Function Settings", EditorStyles.boldLabel);
                propertyRect.y += yDelta;
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUI.PropertyField(propertyRect, m_Fields.overrideDefaultKeyFunction, Contents.overrideDefaultKeyFunction);
                    propertyRect.y += yDelta;
                    if (m_Fields.overrideDefaultKeyFunction.boolValue)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUI.PropertyField(propertyRect, m_Fields.keyFunction, Contents.keyFunction);
                            propertyRect.y += yDelta;
                        }
                    }

                    EditorGUI.PropertyField(propertyRect, m_Fields.keyCode, Contents.keyCode);
                    propertyRect.y += yDelta;
                    EditorGUI.PropertyField(propertyRect, m_Fields.disabled, Contents.disabled);
                    propertyRect.y += yDelta;
                }
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        static string GetPreviewString(SerializedPropertyFields fields)
        {
            if (fields.overrideDefaultKeyFunction.boolValue)
            {
                var keyFunctionName = fields.keyFunction.objectReferenceValue != null
                    ? fields.keyFunction.objectReferenceValue.name
                    : "None";
                return $"{fields.character.stringValue} [{keyFunctionName}]";
            }

            return fields.character.stringValue;
        }
    }
}
#endif
