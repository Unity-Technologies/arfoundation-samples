#if OPENXR_1_13_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomPropertyDrawer(typeof(SelectOpenXRFeatureTypenameAttribute))]
    public class SelectOpenXRFeatureTypenameDrawer : PropertyDrawer
    {
        Type[] m_FeatureTypes;
        string[] m_FeatureDisplayNames;
        int m_SelectedDisplayNameIndex;

        static TypenameCompareByOpenXRDisplayName s_Comparer = new();
        static GUIContent s_BackupLabel = new GUIContent("Feature");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_FeatureTypes == null)
            {
                m_FeatureTypes = TypeCache.GetTypesWithAttribute<OpenXRFeatureAttribute>()
                    .Where(t =>
                        t.IsPublic
                        && !t.IsAbstract
                        && !((OpenXRFeatureAttribute)Attribute.GetCustomAttribute(t, typeof(OpenXRFeatureAttribute))).Hidden
                        && !((OpenXRFeatureAttribute)Attribute.GetCustomAttribute(t, typeof(OpenXRFeatureAttribute))).Category.Equals(FeatureCategory.Interaction))
                    .ToArray();

                Array.Sort(m_FeatureTypes, s_Comparer);
                m_FeatureDisplayNames = new string[m_FeatureTypes.Length + 1];
                m_FeatureDisplayNames[0] = m_FeatureTypes.Length > 0 ? "Choose an OpenXR feature" : "No OpenXR features available";

                for (var i = 0; i < m_FeatureTypes.Length; i++)
                {
                    m_FeatureDisplayNames[i + 1] = GetOpenXRDisplayName(m_FeatureTypes[i]);
                }
            }

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                var serializedTypename = property.stringValue;

                var i = 1; // accounts for default "Choose a Type" option
                foreach (var t in m_FeatureTypes)
                {
                    if (string.Equals(t.AssemblyQualifiedName, serializedTypename))
                    {
                        m_SelectedDisplayNameIndex = i; 
                        break;
                    }

                    if (i == m_FeatureTypes.Length)
                    {
                        // There is a serialized string value, but it doesn't match a type that is currently installed
                        // In this case, do not modify the serialized string
                        EditorGUILayout.PropertyField(property, s_BackupLabel);
                        return;
                    }

                    i++;
                }
            }
            else
            {
                m_SelectedDisplayNameIndex = 0;
            }

            var selectedIndex = EditorGUILayout.Popup("Feature", m_SelectedDisplayNameIndex, m_FeatureDisplayNames);
            if (selectedIndex == 0)
            {
                property.stringValue = string.Empty;
            }
            else if (m_SelectedDisplayNameIndex != selectedIndex)
            {
                property.stringValue = m_FeatureTypes[selectedIndex - 1].AssemblyQualifiedName;
                m_SelectedDisplayNameIndex = selectedIndex;
            }
        }
        
        static string GetOpenXRDisplayName(Type t)
        {
            return ((OpenXRFeatureAttribute)Attribute.GetCustomAttribute(t, typeof(OpenXRFeatureAttribute))).UiName;
        }

        class TypenameCompareByOpenXRDisplayName : IComparer<Type>
        {
            int IComparer<Type>.Compare(Type x, Type y)
            {
                return string.Compare(GetOpenXRDisplayName(x), GetOpenXRDisplayName(y));
            }
        }
    }
}
#endif // OPENXR_1_13_OR_NEWER
