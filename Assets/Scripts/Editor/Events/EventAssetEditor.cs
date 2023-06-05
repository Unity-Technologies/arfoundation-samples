using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomEditor(typeof(EventAsset))]
    public class EventAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Raise"))
            {
                (serializedObject.targetObject as EventAsset)!.Raise();
            }
        }
    }
}
