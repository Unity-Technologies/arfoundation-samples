using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// Custom editor for a <see cref="KeyboardOptimizer"/>.
    /// </summary>
    [CustomEditor(typeof(KeyboardOptimizer), true), CanEditMultipleObjects]
    public class KeyboardOptimizerEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="KeyboardOptimizer.optimizeOnStart"/>.</summary>
        protected SerializedProperty m_OptimizeOnStart;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="KeyboardOptimizer.batchGroupParentTransform"/>.</summary>
        protected SerializedProperty m_BatchGroupParentTransform;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="KeyboardOptimizer.buttonParentTransform"/>.</summary>
        protected SerializedProperty m_ButtonParentTransform;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="KeyboardOptimizer.imageParentTransform"/>.</summary>
        protected SerializedProperty m_ImageParentTransform;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="KeyboardOptimizer.textParentTransform"/>.</summary>
        protected SerializedProperty m_TextParentTransform;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="KeyboardOptimizer.iconParentTransform"/>.</summary>
        protected SerializedProperty m_IconParentTransform;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="KeyboardOptimizer.highlightParentTransform"/>.</summary>
        protected SerializedProperty m_HighlightParentTransform;

        KeyboardOptimizer m_KeyboardTarget;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="KeyboardOptimizer.optimizeOnStart"/>.</summary>
            public static readonly GUIContent optimizeOnStart = EditorGUIUtility.TrTextContent("Optimize On Start", "If enabled, the optimization will be called on Start.");

            /// <summary><see cref="GUIContent"/> for the <see cref="KeyboardOptimizer.batchGroupParentTransform"/> property.</summary>
            public static readonly GUIContent batchGroupParentTransform = EditorGUIUtility.TrTextContent("Batch Group Parent Transform", "The parent transform for batch groups.");

            /// <summary><see cref="GUIContent"/> for the <see cref="KeyboardOptimizer.buttonParentTransform"/> property.</summary>
            public static readonly GUIContent buttonParentTransform = EditorGUIUtility.TrTextContent("Button Parent Transform", "The parent transform for buttons.");

            /// <summary><see cref="GUIContent"/> for the <see cref="KeyboardOptimizer.imageParentTransform"/> property.</summary>
            public static readonly GUIContent imageParentTransform = EditorGUIUtility.TrTextContent("Image Parent Transform", "The parent transform for images.");

            /// <summary><see cref="GUIContent"/> for the <see cref="KeyboardOptimizer.textParentTransform"/> property.</summary>
            public static readonly GUIContent textParentTransform = EditorGUIUtility.TrTextContent("Text Parent Transform", "The parent transform for text elements.");

            /// <summary><see cref="GUIContent"/> for the <see cref="KeyboardOptimizer.iconParentTransform"/> property.</summary>
            public static readonly GUIContent iconParentTransform = EditorGUIUtility.TrTextContent("Icon Parent Transform", "The parent transform for icons.");

            /// <summary><see cref="GUIContent"/> for the <see cref="KeyboardOptimizer.highlightParentTransform"/> property.</summary>
            public static readonly GUIContent highlightParentTransform = EditorGUIUtility.TrTextContent("Highlight Parent Transform", "The parent transform for highlights.");

            /// <summary><see cref="GUIContent"/> for Batch Transform References header label.</summary>
            public static readonly GUIContent batchTransformReferencesHeader = EditorGUIUtility.TrTextContent("Batch Transform References");

            /// <summary><see cref="GUIContent"/> for Optimize button.</summary>
            public static readonly GUIContent optimizeButton = EditorGUIUtility.TrTextContent("Optimize", "This is the same as call Optimize() in the script.");

            /// <summary><see cref="GUIContent"/> for Unoptimize button.</summary>
            public static readonly GUIContent unoptimizeButton = EditorGUIUtility.TrTextContent("Unoptimize", "This is the same as call Unoptimize() in the script.");

            /// <summary><see cref="GUIContent"/> for the Optimization header label.</summary>
            public static readonly GUIContent optimizationHeader = EditorGUIUtility.TrTextContent("Optimization");

            /// <summary><see cref="GUIContent"/> for the message label when multi-object editing.</summary>
            public static readonly GUIContent optimizationOnlySingleObject = EditorGUIUtility.TrTextContent("Optimization is only available when a single GameObject is selected.");

            /// <summary><see cref="GUIContent"/> for the message label when not in a scene.</summary>
            public static readonly GUIContent optimizationOnlyInScene = EditorGUIUtility.TrTextContent("Optimization is only available with scene objects during runtime.");

            /// <summary><see cref="GUIContent"/> for the message label when not in Play mode.</summary>
            public static readonly GUIContent optimizationOnlyDuringRuntime = EditorGUIUtility.TrTextContent("Optimization is only available during runtime.");
        }

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_OptimizeOnStart = serializedObject.FindProperty("m_OptimizeOnStart");
            m_BatchGroupParentTransform = serializedObject.FindProperty("m_BatchGroupParentTransform");
            m_ButtonParentTransform = serializedObject.FindProperty("m_ButtonParentTransform");
            m_ImageParentTransform = serializedObject.FindProperty("m_ImageParentTransform");
            m_TextParentTransform = serializedObject.FindProperty("m_TextParentTransform");
            m_IconParentTransform = serializedObject.FindProperty("m_IconParentTransform");
            m_HighlightParentTransform = serializedObject.FindProperty("m_HighlightParentTransform");

            m_KeyboardTarget = (KeyboardOptimizer)target;
        }

        /// <inheritdoc />
        /// <seealso cref="DrawBeforeProperties"/>
        /// <seealso cref="DrawProperties"/>
        /// <seealso cref="BaseInteractionEditor.DrawDerivedProperties"/>
        protected override void DrawInspector()
        {
            DrawBeforeProperties();
            DrawProperties();
            DrawDerivedProperties();
            DrawOptimizationControls();
        }

        /// <summary>
        /// This method is automatically called by <see cref="DrawInspector"/> to
        /// draw the section of the custom inspector before <see cref="DrawProperties"/>.
        /// By default, this draws the read-only Script property.
        /// </summary>
        protected virtual void DrawBeforeProperties()
        {
            DrawScript();
        }

        /// <summary>
        /// This method is automatically called by <see cref="DrawInspector"/> to
        /// draw the property fields. Override this method to customize the
        /// properties shown in the Inspector. This is typically the method overridden
        /// when a derived behavior adds additional serialized properties
        /// that should be displayed in the Inspector.
        /// </summary>
        protected virtual void DrawProperties()
        {
            EditorGUILayout.PropertyField(m_OptimizeOnStart, Contents.optimizeOnStart);

            m_BatchGroupParentTransform.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_BatchGroupParentTransform.isExpanded, Contents.batchTransformReferencesHeader);
            if (m_BatchGroupParentTransform.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_BatchGroupParentTransform, Contents.batchGroupParentTransform);
                    EditorGUILayout.PropertyField(m_ButtonParentTransform, Contents.buttonParentTransform);
                    EditorGUILayout.PropertyField(m_ImageParentTransform, Contents.imageParentTransform);
                    EditorGUILayout.PropertyField(m_TextParentTransform, Contents.textParentTransform);
                    EditorGUILayout.PropertyField(m_IconParentTransform, Contents.iconParentTransform);
                    EditorGUILayout.PropertyField(m_HighlightParentTransform, Contents.highlightParentTransform);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        /// <summary>
        /// This method is automatically called by <see cref="DrawInspector"/> to
        /// draw the Optimization section.
        /// </summary>
        protected virtual void DrawOptimizationControls()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Contents.optimizationHeader, EditorStyles.boldLabel);

            var isOptimizationAvailable = true;
            if (targets.Length > 1)
            {
                EditorGUILayout.HelpBox(Contents.optimizationOnlySingleObject.text, MessageType.None);
                isOptimizationAvailable = false;
            }
            else if (!m_KeyboardTarget.gameObject.scene.IsValid())
            {
                EditorGUILayout.HelpBox(Contents.optimizationOnlyInScene.text, MessageType.None);
                isOptimizationAvailable = false;
            }
            else if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox(Contents.optimizationOnlyDuringRuntime.text, MessageType.None);
                isOptimizationAvailable = false;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!isOptimizationAvailable || m_KeyboardTarget.isCurrentlyOptimized))
                {
                    if (GUILayout.Button(Contents.optimizeButton))
                        ((KeyboardOptimizer)target).Optimize();
                }

                using (new EditorGUI.DisabledScope(!isOptimizationAvailable || !m_KeyboardTarget.isCurrentlyOptimized))
                {
                    if (GUILayout.Button(Contents.unoptimizeButton))
                        ((KeyboardOptimizer)target).Unoptimize();
                }
            }
        }
    }
}
