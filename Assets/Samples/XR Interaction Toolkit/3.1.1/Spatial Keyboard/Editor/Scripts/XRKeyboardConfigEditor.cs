#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

namespace UnityEditor.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// Custom editor for an <see cref="XRKeyboardConfig"/>.
    /// </summary>
    [CustomEditor(typeof(XRKeyboardConfig), true), CanEditMultipleObjects]
    public class XRKeyboardConfigEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardConfig.defaultKeyFunction"/>.</summary>
        protected SerializedProperty m_DefaultKeyFunction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRKeyboardConfig.keyMappings"/>.</summary>
        protected SerializedProperty m_KeyMappings;

        /// <summary>
        /// See <see cref="Editor"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            m_DefaultKeyFunction = serializedObject.FindProperty("m_DefaultKeyFunction");
            m_KeyMappings = serializedObject.FindProperty("m_KeyMappings");
        }

        /// <inheritdoc />
        protected override void DrawInspector()
        {
            DrawScript();
            EditorGUILayout.PropertyField(m_DefaultKeyFunction);
            EditorGUILayout.PropertyField(m_KeyMappings);
        }
    }
}
#endif
