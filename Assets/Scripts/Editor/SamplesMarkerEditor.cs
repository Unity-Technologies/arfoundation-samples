using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomEditor(typeof(SampleMarkers))]
    public class SampleMarkersEditor : Editor
    {
        SerializedProperty m_QRCodesProp;
        SerializedProperty m_ArucoMarkersProp;
        SerializedProperty m_AprilTagsProp;

        GUIStyle m_TitleStyle;
        GUIStyle m_DescStyle;

        void OnEnable()
        {
            m_QRCodesProp = serializedObject.FindProperty("m_QRCodes");
            m_ArucoMarkersProp = serializedObject.FindProperty("m_ArucoMarkers");
            m_AprilTagsProp = serializedObject.FindProperty("m_AprilTags");
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
            InitStyles();

            serializedObject.Update();

            DrawDefaultInspector();

            if (m_QRCodesProp == null || !m_QRCodesProp.isArray)
                return;

            for (var i = 0; i < m_QRCodesProp.arraySize; i++)
            {
                DrawMarkerData(m_QRCodesProp.GetArrayElementAtIndex(i));
                GUILayout.Space(20);
            }

            for (var i = 0; i < m_ArucoMarkersProp.arraySize; i++)
            {
                DrawMarkerData(m_ArucoMarkersProp.GetArrayElementAtIndex(i));
                GUILayout.Space(20);
            }

            for (var i = 0; i < m_AprilTagsProp.arraySize; i++)
            {
                DrawMarkerData(m_AprilTagsProp.GetArrayElementAtIndex(i));
                GUILayout.Space(20);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawMarkerData(SerializedProperty element)
        {
            var titleProp = element.FindPropertyRelative("m_Title");
            var descProp = element.FindPropertyRelative("m_Description");
            var texProp = element.FindPropertyRelative("m_MarkerTexture");

            var texture = (Texture2D)texProp.objectReferenceValue;

            if (texture == null)
                return;

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
        }
    }
}
