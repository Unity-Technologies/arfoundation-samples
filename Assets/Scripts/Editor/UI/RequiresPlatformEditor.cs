using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomEditor(typeof(RequiresPlatform))]
    public class RequiresPlatformEditor : Editor
    {
        SerializedProperty m_ScriptProp;
        SerializedProperty m_RequiredPlatformProp;
        SerializedProperty m_RequiresMinimumVersionProp;
        SerializedProperty m_RequiredVersionProp;
        SerializedProperty m_AllowEditorProp;

        void OnEnable()
        {
            m_ScriptProp = serializedObject.FindProperty("m_Script");
            m_RequiredPlatformProp = serializedObject.FindProperty("m_RequiredPlatform");
            m_RequiresMinimumVersionProp = serializedObject.FindProperty("m_RequiresMinimumVersion");
            m_RequiredVersionProp = serializedObject.FindProperty("m_RequiredVersion");
            m_AllowEditorProp = serializedObject.FindProperty("m_AllowEditor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_ScriptProp);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(m_RequiredPlatformProp);

            var fullRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none);
            var y = fullRect.y;
            var height = EditorGUIUtility.singleLineHeight;
            var widthOfToggle = 16;
            var horizontalSpace = 2;
            
            var labelRect = new Rect(fullRect.x, y, EditorGUIUtility.labelWidth, height);
            var toggleRect = new Rect(labelRect.xMax + horizontalSpace, y, widthOfToggle, height);
            var versionRect = new Rect(
                toggleRect.xMax + horizontalSpace,
                y,
                fullRect.width - labelRect.width - toggleRect.width - 2 * horizontalSpace,
                height);

            EditorGUI.LabelField(labelRect, "Requires Minimum Version");
            m_RequiresMinimumVersionProp.boolValue = EditorGUI.Toggle(toggleRect, m_RequiresMinimumVersionProp.boolValue);

            bool requiresMinimumVersion = m_RequiresMinimumVersionProp.boolValue;

            if (requiresMinimumVersion)
                m_RequiredVersionProp.intValue = EditorGUI.IntField(versionRect, m_RequiredVersionProp.intValue);

            EditorGUILayout.PropertyField(m_AllowEditorProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
