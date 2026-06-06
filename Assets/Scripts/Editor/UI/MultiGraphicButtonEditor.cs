using UnityEngine;
using UnityEditor.UI;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomEditor(typeof(MultiGraphicButton))]
    public class MultiGraphicButtonEditor : ButtonEditor
    {
        SerializedProperty m_ExtraGraphicsProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_ExtraGraphicsProperty = serializedObject.FindProperty("m_ExtraGraphics");
            m_ExtraGraphicsProperty.isExpanded = true;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_ExtraGraphicsProperty, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
