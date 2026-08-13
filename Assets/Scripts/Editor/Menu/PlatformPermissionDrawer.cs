using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEditor.XR.ARFoundation.Samples
{
    [CustomPropertyDrawer(typeof(RequiresPermission.PlatformPermission))]
    public class PlatformPermissionDrawer : PropertyDrawer
    {
        const float k_PlatformFieldWidth = 120f;
        const float k_Spacing = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var platformProp = property.FindPropertyRelative("platform");
            var permissionProp = property.FindPropertyRelative("permissionId");

            var platformRect = new Rect(position.x, position.y, k_PlatformFieldWidth, position.height);
            var permissionRect = new Rect(
                position.x + k_PlatformFieldWidth + k_Spacing,
                position.y,
                position.width - k_PlatformFieldWidth - k_Spacing,
                position.height);

            EditorGUI.PropertyField(platformRect, platformProp, GUIContent.none);
            EditorGUI.PropertyField(permissionRect, permissionProp, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
