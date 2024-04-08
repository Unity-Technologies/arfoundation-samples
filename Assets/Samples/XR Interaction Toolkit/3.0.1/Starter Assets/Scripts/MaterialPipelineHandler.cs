using UnityEngine.Rendering;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    static class RenderPipelineValidation
    {
        static RenderPipelineValidation()
        {
            foreach (var pipelineHandler in GetAllInstances())
                pipelineHandler.AutoRefreshPipelineShaders();
        }

        static MaterialPipelineHandler[] GetAllInstances()
        {
            // Find all GUIDs for objects that match the type MaterialPipelineHandler
            string[] guids = AssetDatabase.FindAssets("t:MaterialPipelineHandler");

            MaterialPipelineHandler[] instances = new MaterialPipelineHandler[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                instances[i] = AssetDatabase.LoadAssetAtPath<MaterialPipelineHandler>(path);
            }

            return instances;
        }
    }
#endif

    /// <summary>
    /// Serializable class that contains the shader information for a material.
    /// </summary>
    [System.Serializable]
    public class ShaderContainer
    {
        public Material material;
        public bool useSRPShaderName = true;
        public string scriptableRenderPipelineShaderName = "Universal Render Pipeline/Lit";
        public Shader scriptableRenderPipelineShader;
        public bool useBuiltinShaderName = true;
        public string builtInPipelineShaderName = "Standard";
        public Shader builtInPipelineShader;
    }

    /// <summary>
    /// Scriptable object that allows for setting the shader on a material based on the current render pipeline.
    /// Will run automatically OnEnable in the editor to set the shaders on project bootup. Can be refreshed manually with editor button.
    /// This exists because while objects render correctly using shadergraph shaders, others do not and using the standard shader resolves various rendering issues.
    /// </summary>
    [CreateAssetMenu(fileName = "MaterialPipelineHandler", menuName = "XR/MaterialPipelineHandler", order = 0)]
    public class MaterialPipelineHandler : ScriptableObject
    {
        [SerializeField]
        [Tooltip("List of materials and their associated shaders.")]
        List<ShaderContainer> m_ShaderContainers;
        
        [SerializeField]
        [Tooltip("If true, the shaders will be refreshed automatically when the editor opens and when this scriptable object instance is enabled.")]
        bool m_AutoRefreshShaders = true;

#if UNITY_EDITOR
        void OnEnable()
        {
            if (Application.isPlaying)
                return;
            AutoRefreshPipelineShaders();
        }
#endif

        public void AutoRefreshPipelineShaders()
        {
            if (m_AutoRefreshShaders)
                SetPipelineShaders();
        }

        /// <summary>
        /// Applies the appropriate shader to the materials based on the current render pipeline.
        /// </summary>
        public void SetPipelineShaders()
        {
            if (m_ShaderContainers == null)
                return;

            bool isBuiltinRenderPipeline = GraphicsSettings.currentRenderPipeline == null;

            foreach (var info in m_ShaderContainers)
            {
                if (info.material == null)
                    continue;

                // Find the appropriate shaders based on the toggle
                Shader birpShader = info.useBuiltinShaderName ? Shader.Find(info.builtInPipelineShaderName) : info.builtInPipelineShader;
                Shader srpShader = info.useSRPShaderName ? Shader.Find(info.scriptableRenderPipelineShaderName) : info.scriptableRenderPipelineShader;

                // Determine current shader for comparison
                Shader currentShader = info.material.shader;

                // Update shader for the current render pipeline only if necessary
                if (isBuiltinRenderPipeline && birpShader != null && currentShader != birpShader)
                {
                    info.material.shader = birpShader;
                    MarkMaterialModified(info.material);
                }
                else if (!isBuiltinRenderPipeline && srpShader != null && currentShader != srpShader )
                {
                    info.material.shader = srpShader;
                    MarkMaterialModified(info.material);
                }
            }
        }

        static void MarkMaterialModified(Material material)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(material);
#endif
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Custom property drawer for the shader container class.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShaderContainer))]
    public class ShaderContainerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty materialProp = property.FindPropertyRelative("material");
            SerializedProperty useSRPShaderNameProp = property.FindPropertyRelative("useSRPShaderName");
            SerializedProperty scriptableShaderNameProp = property.FindPropertyRelative("scriptableRenderPipelineShaderName");
            SerializedProperty scriptableShaderProp = property.FindPropertyRelative("scriptableRenderPipelineShader");
            SerializedProperty useShaderNameProp = property.FindPropertyRelative("useBuiltinShaderName");
            SerializedProperty builtInNameProp = property.FindPropertyRelative("builtInPipelineShaderName");
            SerializedProperty builtInShaderProp = property.FindPropertyRelative("builtInPipelineShader");

            // Draw Material without the header.
            position.height = singleLineHeight;
            EditorGUI.PropertyField(position, materialProp);
            position.y += singleLineHeight + verticalSpacing;

            // SRP Shader header and fields.
            EditorGUI.LabelField(position, "Scriptable Render Pipeline Shader", EditorStyles.boldLabel);
            position.y += EditorGUIUtility.singleLineHeight + verticalSpacing;

            EditorGUI.PropertyField(position, useSRPShaderNameProp);
            position.y += singleLineHeight + verticalSpacing;

            if (useSRPShaderNameProp.boolValue)
            {
                EditorGUI.PropertyField(position, scriptableShaderNameProp);
                position.y += singleLineHeight + verticalSpacing;
            }
            else
            {
                EditorGUI.PropertyField(position, scriptableShaderProp);
                position.y += singleLineHeight + verticalSpacing;
            }

            // Built-in Shader header and fields.
            EditorGUI.LabelField(position, "Built-In Render Pipeline Shader", EditorStyles.boldLabel);
            position.y += singleLineHeight + verticalSpacing;

            EditorGUI.PropertyField(position, useShaderNameProp);
            position.y += singleLineHeight + verticalSpacing;

            if (useShaderNameProp.boolValue)
            {
                EditorGUI.PropertyField(position, builtInNameProp);
                position.y += singleLineHeight + verticalSpacing;
            }
            else
            {
                EditorGUI.PropertyField(position, builtInShaderProp);
                position.y += singleLineHeight + verticalSpacing;
            }

            // Draw a separator line at the end.
            position.y += verticalSpacing / 2; // Extra space for the line.
            position.height = 1;
            EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, 1), Color.gray);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            const int baseFieldCount = 4; // The Material field, the two toggles, and one for an optional field.
            int extraLineCount = property.FindPropertyRelative("useBuiltinShaderName").boolValue ? 0 : 1;
            extraLineCount += property.FindPropertyRelative("useSRPShaderName").boolValue ? 0 : 1;

            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            float headerHeight = EditorGUIUtility.singleLineHeight; // No longer need extra height for headers.

            // Calculate height for fields and headers
            float fieldsHeight = baseFieldCount * singleLineHeight + (baseFieldCount - 1 + extraLineCount) * verticalSpacing;

            // Allow space for header, separator line, and a bit of padding before the line.
            float headersHeight = 2 * (headerHeight + verticalSpacing);
            float separatorSpace = verticalSpacing / 2 + 1; // Additional vertical spacing and line height.

            return fieldsHeight + headersHeight + separatorSpace + singleLineHeight * 1.5f;
        }
    }

    /// <summary>
    /// Custom editor MaterialPipelineHandler
    /// </summary>
    [CustomEditor(typeof(MaterialPipelineHandler)), CanEditMultipleObjects]
    public class MaterialPipelineHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Draw the "Refresh Shaders" button
            if (GUILayout.Button("Refresh Shaders"))
            {
                foreach (var t in targets)
                {
                    var handler = (MaterialPipelineHandler)t;
                    handler.SetPipelineShaders();
                }
            }
        }
    }
#endif
}