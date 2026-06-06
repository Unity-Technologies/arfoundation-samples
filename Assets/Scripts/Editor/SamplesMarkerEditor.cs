using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomEditor(typeof(SampleMarkers))]
    public class SampleMarkersEditor : Editor
    {
        SerializedProperty m_QRCodesProperty;

        GUIStyle m_TitleStyle;
        GUIStyle m_DescStyle;

        void OnEnable()
        {
            if (target == null)
                return;

            m_QRCodesProperty = serializedObject.FindProperty("m_QRCodes");
        }

        void InitStyles()
        {
            if (m_TitleStyle == null)
            {
                m_TitleStyle = new GUIStyle(EditorStyles.boldLabel);
                m_TitleStyle.fontSize = 15;
                m_TitleStyle.alignment = TextAnchor.MiddleLeft;
            }

            if (m_DescStyle == null)
            {
                m_DescStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
                m_DescStyle.alignment = TextAnchor.MiddleLeft;
            }
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            InitStyles();

            serializedObject.Update();

            DrawDefaultInspector();

            if (m_QRCodesProperty == null || !m_QRCodesProperty.isArray)
                return;

            for (var i = 0; i < m_QRCodesProperty.arraySize; i++)
            {
                var element = m_QRCodesProperty.GetArrayElementAtIndex(i);

                var titleProp = element.FindPropertyRelative("m_Title");
                var descProp = element.FindPropertyRelative("m_Description");
                var texProp = element.FindPropertyRelative("m_MarkerTexture");

                var texture = (Texture2D)texProp.objectReferenceValue;

                if (texture == null)
                    continue;

                var titleText = titleProp.stringValue;
                var descriptionText = descProp.stringValue;

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                GUILayout.BeginHorizontal();

                GUILayout.Label(string.IsNullOrEmpty(titleText) ? "Untitled" : titleText, m_TitleStyle, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Open Image", EditorStyles.miniButton, GUILayout.Width(80)))
                {
                    var assetPath = AssetDatabase.GetAssetPath(texture);
                    EditorUtility.OpenWithDefaultApp(assetPath);
                }

                GUILayout.EndHorizontal();

                GUILayout.Label(descriptionText, m_DescStyle);

                GUILayout.Space(10);

                var aspectRatio = (float)texture.width / Mathf.Max(1, texture.height);
                var width = EditorGUIUtility.currentViewWidth - 40;
                var height = width / aspectRatio;

                var rect = GUILayoutUtility.GetRect(width, height);

                if (Event.current.type == EventType.Repaint)
                    EditorGUI.DrawPreviewTexture(rect, texture, null, ScaleMode.ScaleToFit);

                GUILayout.Space(20);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
