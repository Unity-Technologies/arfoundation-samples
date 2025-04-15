#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

namespace UnityEditor.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// Custom editor for an <see cref="XRKeyboard"/>.
    /// </summary>
    [CustomEditor(typeof(XRKeyboard), true), CanEditMultipleObjects]
    public class XRKeyboardEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.submitOnEnter"/>.</summary>
        protected SerializedProperty m_SubmitOnEnter;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.closeOnSubmit"/>.</summary>
        protected SerializedProperty m_CloseOnSubmit;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.doubleClickInterval"/>.</summary>
        protected SerializedProperty m_DoubleClickInterval;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.subsetLayout"/>.</summary>
        protected SerializedProperty m_SubsetLayout;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.onTextSubmitted"/>.</summary>
        protected SerializedProperty m_OnTextSubmit;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.onTextUpdated"/>.</summary>
        protected SerializedProperty m_OnTextUpdate;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.onKeyPressed"/>.</summary>
        protected SerializedProperty m_OnKeyPressed;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.onShifted"/>.</summary>
        protected SerializedProperty m_OnShift;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.onLayoutChanged"/>.</summary>
        protected SerializedProperty m_OnLayoutChange;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.onOpened"/>.</summary>
        protected SerializedProperty m_OnOpen;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.onClosed"/>.</summary>
        protected SerializedProperty m_OnClose;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.onFocusChanged"/>.</summary>
        protected SerializedProperty m_OnFocusChanged;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboard.onCharacterLimitReached"/>.</summary>
        protected SerializedProperty m_OnCharacterLimitReached;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            public static readonly GUIContent submitOnEnter = EditorGUIUtility.TrTextContent("Submit On Enter", "If true, On Text Submit will be invoked when the keyboard receives a return or enter command. Otherwise it will treat return or enter as a newline.");
            public static readonly GUIContent closeOnSubmit = EditorGUIUtility.TrTextContent("Close On Submit", "If true, keyboard will close on enter or return command.");
            public static readonly GUIContent doubleClickInterval = EditorGUIUtility.TrTextContent("Double Click Interval", "Interval in which a key pressed twice would be considered a double click.");
            public static readonly GUIContent subsetLayout = EditorGUIUtility.TrTextContent("Subset Layout", "List of layouts this keyboard is able to switch between given the corresponding layout command.");

            public static readonly GUIContent keyboardEvents = EditorGUIUtility.TrTextContent("Keyboard Events", "Events associated with the keyboard.");
            public static readonly GUIContent onTextSubmit = EditorGUIUtility.TrTextContent("On Text Submitted", "Event invoked when keyboard submits text.");
            public static readonly GUIContent onTextUpdate = EditorGUIUtility.TrTextContent("On Text Updated", "Event invoked when keyboard text is updated.");
            public static readonly GUIContent onKeyPressed = EditorGUIUtility.TrTextContent("On Key Pressed", "Event invoked after a key is pressed.");
            public static readonly GUIContent onShift = EditorGUIUtility.TrTextContent("On Shifted", "Event invoked after keyboard shift is changed.");
            public static readonly GUIContent onLayoutChange = EditorGUIUtility.TrTextContent("On Layout Changed", "Event invoked when the keyboard is opened. Called with the keyboard and the new layout string key.");
            public static readonly GUIContent onOpen = EditorGUIUtility.TrTextContent("On Opened", "Event invoked when the keyboard is opened.");
            public static readonly GUIContent onClose = EditorGUIUtility.TrTextContent("On Closed", "Event invoked after the keyboard is closed.");
            public static readonly GUIContent onFocusChanged = EditorGUIUtility.TrTextContent("On Focus Changed", "Event invoked when the keyboard changes or gains input field focus.");
            public static readonly GUIContent onCharacterLimitReached = EditorGUIUtility.TrTextContent("On Character Limit Reached", "Event invoked when the keyboard tries to update text, but the character of the input field is reached.");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            m_SubmitOnEnter = serializedObject.FindProperty("m_SubmitOnEnter");
            m_CloseOnSubmit = serializedObject.FindProperty("m_CloseOnSubmit");
            m_DoubleClickInterval = serializedObject.FindProperty("m_DoubleClickInterval");
            m_SubsetLayout = serializedObject.FindProperty("m_SubsetLayout");

            m_OnTextSubmit = serializedObject.FindProperty("m_OnTextSubmitted");
            m_OnTextUpdate = serializedObject.FindProperty("m_OnTextUpdated");
            m_OnKeyPressed = serializedObject.FindProperty("m_OnKeyPressed");
            m_OnShift = serializedObject.FindProperty("m_OnShifted");
            m_OnLayoutChange = serializedObject.FindProperty("m_OnLayoutChanged");
            m_OnOpen = serializedObject.FindProperty("m_OnOpened");
            m_OnClose = serializedObject.FindProperty("m_OnClosed");
            m_OnFocusChanged = serializedObject.FindProperty("m_OnFocusChanged");
            m_OnCharacterLimitReached = serializedObject.FindProperty("m_OnCharacterLimitReached");
        }

        /// <inheritdoc />
        protected override void DrawInspector()
        {
            DrawScript();
            EditorGUILayout.PropertyField(m_SubmitOnEnter, Contents.submitOnEnter);
            EditorGUILayout.PropertyField(m_CloseOnSubmit, Contents.closeOnSubmit);
            EditorGUILayout.PropertyField(m_DoubleClickInterval, Contents.doubleClickInterval);
            EditorGUILayout.PropertyField(m_SubsetLayout, Contents.subsetLayout);

            DrawKeyboardEvents();
        }

        void DrawKeyboardEvents()
        {
            m_OnOpen.isExpanded = EditorGUILayout.Foldout(m_OnOpen.isExpanded, Contents.keyboardEvents, toggleOnLabelClick: true);
            if (m_OnOpen.isExpanded)
            {
                EditorGUILayout.PropertyField(m_OnOpen, Contents.onOpen);
                EditorGUILayout.PropertyField(m_OnClose, Contents.onClose);
                EditorGUILayout.PropertyField(m_OnFocusChanged, Contents.onFocusChanged);
                EditorGUILayout.PropertyField(m_OnTextSubmit, Contents.onTextSubmit);
                EditorGUILayout.PropertyField(m_OnTextUpdate, Contents.onTextUpdate);
                EditorGUILayout.PropertyField(m_OnKeyPressed, Contents.onKeyPressed);
                EditorGUILayout.PropertyField(m_OnShift, Contents.onShift);
                EditorGUILayout.PropertyField(m_OnLayoutChange, Contents.onLayoutChange);
                EditorGUILayout.PropertyField(m_OnCharacterLimitReached, Contents.onCharacterLimitReached);
            }
        }
    }
}
#endif
