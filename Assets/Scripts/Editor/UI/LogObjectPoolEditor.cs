using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomEditor(typeof(LogObjectPool))]
    public class LogObjectPoolEditor : Editor
    {
        SerializedProperty m_ScriptProp;
        SerializedProperty m_ScrollRectProp;
        SerializedProperty m_ScrollViewContentProp;
        SerializedProperty m_PoolSizeProp;
        SerializedProperty m_PoolProp;
        SerializedProperty m_TemplateObjectsProp;
        SerializedProperty m_ObjectHeightProp;

        void OnEnable()
        {
            m_ScriptProp = serializedObject.FindProperty("m_Script");
            m_ScrollRectProp = serializedObject.FindProperty("m_ScrollRect");
            m_ScrollViewContentProp = serializedObject.FindProperty("m_ScrollViewContent");
            m_PoolSizeProp = serializedObject.FindProperty("m_PoolSize");
            m_PoolProp = serializedObject.FindProperty("m_Pool");
            m_TemplateObjectsProp = serializedObject.FindProperty("m_TemplateObjects");
            m_ObjectHeightProp = serializedObject.FindProperty("m_ObjectHeight");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_ScriptProp);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(m_ScrollRectProp);
            EditorGUILayout.PropertyField(m_ScrollViewContentProp);

            if (GUILayout.Button("Regenerate Object Pool"))
            {
                if (m_TemplateObjectsProp.arraySize == 0)
                {
                    Debug.LogError("Cannot regenerate Object Pool because the Template Objects list is empty.");
                    return;
                }

                float templateObjectHeight = -1;
                for (var i = 0; i < m_TemplateObjectsProp.arraySize; i++)
                {
                    var gameObject = m_TemplateObjectsProp.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
                    if (gameObject == null)
                    {
                        Debug.LogError("Cannot regenerate Object Pool because the Template Objects list" +
                            " contains a null GameObject.");

                        return;
                    }

                    var rect = gameObject.GetComponent<RectTransform>();
                    if (rect == null)
                    {
                        Debug.LogError("Cannot regenerate Object Pool because the Template Objects list"+
                            " contains a GameObject without a RectTransform component.");

                        return;
                    }

                    if (templateObjectHeight < 0)
                    {
                        templateObjectHeight = rect.GetHeight();
                    }
                    else if (rect.GetHeight() != templateObjectHeight)
                    {
                        Debug.LogError("Cannot regenerate Object Pool because the Template Objects list" +
                            " contains GameObjects with RectTransforms of non-equal height values.");

                        return;
                    }

                    if (gameObject.GetComponentInChildren<TextMeshProUGUI>() == null)
                    {
                        Debug.LogError("Cannot regenerate Object Pool because the Template Objects array" +
                            " contains a GameObject without a TextMeshProUGUI component in its children.");

                        return;
                    }
                }

                var transform = (serializedObject.targetObject as LogObjectPool)!.transform;
                var pool = transform.Find("Pool");
                if (pool == null)
                {
                    Debug.LogError("Please don't delete the Pool GameObject.");
                    return;
                }

                while (pool.childCount > 0)
                {
                    DestroyImmediate(pool.GetChild(0).gameObject);
                }

                int poolSize = m_PoolSizeProp.intValue;
                float poolHeight = 0;
                m_PoolProp.ClearArray();
                m_PoolProp.arraySize = poolSize;
                m_PoolProp.serializedObject.ApplyModifiedProperties();

                for (var i = 0; i < poolSize; i++)
                {
                    var poolObject = Instantiate(
                        m_TemplateObjectsProp.GetArrayElementAtIndex(i % m_TemplateObjectsProp.arraySize).objectReferenceValue as GameObject,
                        pool);

                    poolObject.name = $"Log Object {i}";

                    var rect = poolObject.GetComponent<RectTransform>();
                    rect.TranslateY(-poolHeight);
                    var height = rect.GetHeight();
                    poolHeight += height;
                    m_ObjectHeightProp.floatValue = height;

                    m_PoolProp.GetArrayElementAtIndex(i).objectReferenceValue = poolObject.GetComponentInChildren<TextMeshProUGUI>();
                }
            }

            GUILayout.Space(12);
            EditorGUILayout.PropertyField(m_PoolSizeProp);

            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_PoolProp);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(m_TemplateObjectsProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
