#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard.KeyFunctions
{
    /// <summary>
    /// Key function used to send a key code for the keyboard to process.
    /// </summary>
    [CreateAssetMenu(fileName = "Key Code Function", menuName = "XR/Spatial Keyboard/Key Code Key Function", order = 1)]
    public class KeyCodeFunction : KeyFunction
    {
        /// <inheritdoc />
        public override void ProcessKey(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            if (keyboardContext != null)
                keyboardContext.ProcessKeyCode(key.keyCode);
        }
    }
}
#endif
