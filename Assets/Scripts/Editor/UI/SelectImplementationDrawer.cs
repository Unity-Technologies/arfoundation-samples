using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomPropertyDrawer(typeof(SelectImplementationAttribute))]
    public class SelectImplementationDrawer : PropertyDrawer
    {
        Type[] m_Implementations;
        string[] m_ImplementationNames;
        int m_SelectedNameIndex;

        static TypenameComparer s_Comparer = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_Implementations == null)
            {
                m_Implementations = TypeCache.GetTypesDerivedFrom(
                    (attribute as SelectImplementationAttribute)!.fieldType)
                    .Where(t => t.IsPublic && !t.IsAbstract && !t.IsInterface)
                    .ToArray();

                Array.Sort(m_Implementations, s_Comparer);

                m_ImplementationNames = new string[m_Implementations.Length + 1];
                m_ImplementationNames[0] = m_Implementations.Length > 0 ? "Choose a Type" : "No implementations available";

                for (var i = 0; i < m_Implementations.Length; i++)
                {
                    m_ImplementationNames[i + 1] = m_Implementations[i].Name;
                }
            }

            if (property.managedReferenceValue != null)
            {
                var instanceType = property.managedReferenceValue.GetType();

                var i = 1; // accounts for default "Choose a Type" option
                foreach (var t in m_Implementations)
                {
                    if (string.Equals(t.Name, instanceType.Name))
                    {
                        m_SelectedNameIndex = i; 
                        break;
                    }

                    i++;
                }
            }
            else
            {
                m_SelectedNameIndex = 0;
            }

            var selectedIndex = EditorGUILayout.Popup("Type", m_SelectedNameIndex, m_ImplementationNames);
            if (selectedIndex == 0)
            {
                property.managedReferenceValue = null;
            }
            else if (m_SelectedNameIndex != selectedIndex)
            {
                property.managedReferenceValue = Activator.CreateInstance(m_Implementations[selectedIndex - 1]);
                m_SelectedNameIndex = selectedIndex;
            }

            var childEnumerator = property.GetEnumerator();
            while (childEnumerator.MoveNext())
            {
                var childProperty = childEnumerator.Current as SerializedProperty;
                if (childProperty!.depth > 2)
                    continue;

                EditorGUILayout.PropertyField(childEnumerator.Current as SerializedProperty);
            }
            ((IDisposable)childEnumerator).Dispose();
        }
        
        class TypenameComparer : IComparer<Type>
        {
            int IComparer<Type>.Compare(Type x, Type y)
            {
                return string.Compare(x?.FullName, y?.FullName);
            }
        }
    }
}
