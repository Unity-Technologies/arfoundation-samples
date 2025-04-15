#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

namespace UnityEditor.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// Custom editor for an <see cref="XRKeyboardDisplay"/>.
    /// </summary>
    [CustomEditor(typeof(XRKeyboardDisplay), true), CanEditMultipleObjects]
    public class XRKeyboardDisplayEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.inputField"/>.</summary>
        protected SerializedProperty m_InputField;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.keyboard"/>.</summary>
        protected SerializedProperty m_Keyboard;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.useSceneKeyboard"/>.</summary>
        protected SerializedProperty m_UseSceneKeyboard;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.updateOnKeyPress"/>.</summary>
        protected SerializedProperty m_UpdateOnKeyPress;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.alwaysObserveKeyboard"/>.</summary>
        protected SerializedProperty m_AlwaysObserveKeyboard;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.monitorInputFieldCharacterLimit"/>.</summary>
        protected SerializedProperty m_MonitorInputFieldCharacterLimit;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.clearTextOnSubmit"/>.</summary>
        protected SerializedProperty m_ClearTextOnSubmit;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.clearTextOnOpen"/>.</summary>
        protected SerializedProperty m_ClearTextOnOpen;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.onKeyboardOpened"/>.</summary>
        protected SerializedProperty m_OnKeyboardOpened;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.onKeyboardClosed"/>.</summary>
        protected SerializedProperty m_OnKeyboardClosed;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.onKeyboardFocusChanged"/>.</summary>
        protected SerializedProperty m_OnKeyboardFocusChanged;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardDisplay.onTextSubmitted"/>.</summary>
        protected SerializedProperty m_OnTextSubmitted;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            public static readonly GUIContent inputField = EditorGUIUtility.TrTextContent("Input Field", "Input field linked to this display");
            public static readonly GUIContent keyboard = EditorGUIUtility.TrTextContent("Keyboard", "Keyboard for this display to monitor and interact with. If empty this will default to the GlobalNonNativeKeyboard keyboard.");

            public static readonly GUIContent useSceneKeyboard = EditorGUIUtility.TrTextContent("Use Scene Keyboard", "If true, this display will use the keyboard reference. If false or if the keyboard field is empty, this display will use global keyboard.");
            public static readonly GUIContent updateOnKeyPress = EditorGUIUtility.TrTextContent("Update on Key Press", "If true, this display will update with each key press. If false, this display will update on OnTextSubmit.");
            public static readonly GUIContent alwaysObserveKeyboard = EditorGUIUtility.TrTextContent("Always Observe Keyboard", "If true, this display will always subscribe to the keyboard updates. If false, this display will subscribe to keyboard when the input field gains focus.");
            public static readonly GUIContent monitorInputFieldCharacterLimit = EditorGUIUtility.TrTextContent("Monitor Input Field Character Limit", "If true, this display will use the input field's character limit to limit the update text from the keyboard and will pass this into the keyboard when opening.");
            public static readonly GUIContent clearTextOnSubmit = EditorGUIUtility.TrTextContent("Clear Text on Submit", "If true, this display will clear the input field text on text submit from the keyboard.");
            public static readonly GUIContent clearTextOnOpen = EditorGUIUtility.TrTextContent("Clear Text on Open", "If true, this display will clear the input field text when the keyboard opens.");

            public static readonly GUIContent keyboardEvents = EditorGUIUtility.TrTextContent("Keyboard Display Events", "Events associated with the keyboard display");
            public static readonly GUIContent onKeyboardOpened = EditorGUIUtility.TrTextContent("On Keyboard Opened", "The event that is called when this display opens a keyboard.");
            public static readonly GUIContent onKeyboardClosed = EditorGUIUtility.TrTextContent("On Keyboard Closed", "The event that is called when the keyboard this display is observing is closed.");
            public static readonly GUIContent onKeyboardFocusChanged = EditorGUIUtility.TrTextContent("On Keyboard Focus Changed", "The event that is called when the keyboard changes focus and this display is not focused.");
            public static readonly GUIContent onTextSubmitted = EditorGUIUtility.TrTextContent("On Text Submitted", "The event that is called when this display receives a text submitted event from the keyboard. Invoked with the keyboard text as a parameter.");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            m_InputField = serializedObject.FindProperty("m_InputField");
            m_Keyboard = serializedObject.FindProperty("m_Keyboard");

            m_UseSceneKeyboard = serializedObject.FindProperty("m_UseSceneKeyboard");
            m_UpdateOnKeyPress = serializedObject.FindProperty("m_UpdateOnKeyPress");
            m_AlwaysObserveKeyboard = serializedObject.FindProperty("m_AlwaysObserveKeyboard");
            m_MonitorInputFieldCharacterLimit = serializedObject.FindProperty("m_MonitorInputFieldCharacterLimit");
            m_ClearTextOnSubmit = serializedObject.FindProperty("m_ClearTextOnSubmit");
            m_ClearTextOnOpen = serializedObject.FindProperty("m_ClearTextOnOpen");

            m_OnKeyboardOpened = serializedObject.FindProperty("m_OnKeyboardOpened");
            m_OnKeyboardClosed = serializedObject.FindProperty("m_OnKeyboardClosed");
            m_OnKeyboardFocusChanged = serializedObject.FindProperty("m_OnKeyboardFocusChanged");
            m_OnTextSubmitted = serializedObject.FindProperty("m_OnTextSubmitted");
        }

        /// <inheritdoc />
        protected override void DrawInspector()
        {
            DrawScript();

            EditorGUILayout.PropertyField(m_InputField, Contents.inputField);
            EditorGUILayout.PropertyField(m_UseSceneKeyboard, Contents.useSceneKeyboard);

            using (new EditorGUI.IndentLevelScope())
            {
                using (new EditorGUI.DisabledScope(!m_UseSceneKeyboard.boolValue || Application.isPlaying))
                {
                    EditorGUILayout.PropertyField(m_Keyboard, Contents.keyboard);
                }
            }

            EditorGUILayout.PropertyField(m_UpdateOnKeyPress, Contents.updateOnKeyPress);
            EditorGUILayout.PropertyField(m_AlwaysObserveKeyboard, Contents.alwaysObserveKeyboard);
            EditorGUILayout.PropertyField(m_MonitorInputFieldCharacterLimit, Contents.monitorInputFieldCharacterLimit);
            EditorGUILayout.PropertyField(m_ClearTextOnSubmit, Contents.clearTextOnSubmit);
            EditorGUILayout.PropertyField(m_ClearTextOnOpen, Contents.clearTextOnOpen);

            DrawKeyboardEvents();
        }

        void DrawKeyboardEvents()
        {
            m_OnTextSubmitted.isExpanded = EditorGUILayout.Foldout(m_OnTextSubmitted.isExpanded, Contents.keyboardEvents, toggleOnLabelClick: true);
            if (m_OnTextSubmitted.isExpanded)
            {
                EditorGUILayout.PropertyField(m_OnTextSubmitted, Contents.onTextSubmitted);
                EditorGUILayout.PropertyField(m_OnKeyboardOpened, Contents.onKeyboardOpened);
                EditorGUILayout.PropertyField(m_OnKeyboardClosed, Contents.onKeyboardClosed);
                EditorGUILayout.PropertyField(m_OnKeyboardFocusChanged, Contents.onKeyboardFocusChanged);
            }
        }
    }
}
#endif
