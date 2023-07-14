using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomEditor(typeof(ReparentTransforms))]
    public class ReparentTransformsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.LabelField("Populate Elements To Reparent with children of:");
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Current parent"))
            {
                ReparentTransforms swapperScript = (ReparentTransforms)target;
                if (swapperScript == null)
                    return;

                var chosenParent = swapperScript.currentParent != null
                    ? swapperScript.currentParent
                    : swapperScript.transform;
                swapperScript.elementsToReparent = getElements(swapperScript, chosenParent);
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("New parent"))
            {
                ReparentTransforms swapperScript = (ReparentTransforms)target;
                if (swapperScript == null)
                    return;

                swapperScript.elementsToReparent = getElements(swapperScript, swapperScript.newParent);
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            if (GUILayout.Button("Test swap"))
            {
                ReparentTransforms swapperScript = (ReparentTransforms)target;
                if (swapperScript == null || swapperScript.elementsToReparent.Length == 0)
                    return;

                swapperScript.SwapTransformParentsInEditor();
            }

            serializedObject.ApplyModifiedProperties();
        }

        public Transform[] getElements(ReparentTransforms swapperScript, Transform fromParent)
        {
            return fromParent.GetComponentsInChildren<Transform>()
                .Where(
                    elem =>
                    {
                        var parentGameObject = fromParent.gameObject;
                        return (elem.gameObject != parentGameObject
                            && elem.parent.gameObject == parentGameObject);
                    }
                ).ToArray();
        }
    }
}
