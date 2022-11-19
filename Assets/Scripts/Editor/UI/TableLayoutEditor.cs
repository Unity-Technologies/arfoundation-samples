using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomEditor(typeof(TableLayout))]
    public class TableLayoutEditor : Editor
    {
        SerializedProperty m_ScriptProp;
        SerializedProperty m_CellsProp;
        SerializedProperty m_AlignmentProp;
        SerializedProperty m_ColumnsProp;
        SerializedProperty m_CellWidthProp;
        SerializedProperty m_CellHeightProp;
        SerializedProperty m_CellPaddingProp;

        void OnEnable()
        {
            m_ScriptProp = serializedObject.FindProperty("m_Script");
            m_CellsProp = serializedObject.FindProperty("m_Cells");
            m_AlignmentProp = serializedObject.FindProperty("m_Alignment");
            m_ColumnsProp = serializedObject.FindProperty("m_Columns");
            m_CellWidthProp = serializedObject.FindProperty("m_CellWidth");
            m_CellHeightProp = serializedObject.FindProperty("m_CellHeight");
            m_CellPaddingProp = serializedObject.FindProperty("m_CellPadding");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_ScriptProp);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(m_CellsProp);
            if (GUILayout.Button("Refresh from children"))
            {
                PopulateCellsFromChildren();
            }

            GUILayout.Space(12);
            EditorGUILayout.LabelField("Layout", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_AlignmentProp);
            EditorGUILayout.PropertyField(m_ColumnsProp);
            EditorGUILayout.PropertyField(m_CellWidthProp);
            EditorGUILayout.PropertyField(m_CellHeightProp);
            EditorGUILayout.PropertyField(m_CellPaddingProp);

            serializedObject.ApplyModifiedProperties();
        }

        void PopulateCellsFromChildren()
        {
            var transform = (serializedObject.targetObject as TableLayout)!.transform;
            var childCount = transform.childCount;

            var iterator = m_CellsProp.Copy();
            iterator.Next(true); // advance to generic field
            iterator.Next(true); // advance to array size field

            m_CellsProp.ClearArray();
            iterator.intValue = childCount;
            iterator.Next(true); // advance to first array index
            
            var lastIndex = childCount - 1;
            for (var i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                var rect = child.GetComponent<RectTransform>();
                if (rect == null)
                {
                    Debug.LogWarning($"{child.name} is missing a RectTransform component. Add the component and try again?");
                    continue;
                }

                iterator.objectReferenceValue = rect;

                if (i < lastIndex)
                    iterator.Next(false); // advance through array
            }
        }
    }
}
