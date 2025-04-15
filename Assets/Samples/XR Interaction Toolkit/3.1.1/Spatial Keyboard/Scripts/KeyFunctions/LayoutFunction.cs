#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard.KeyFunctions
{
    /// <summary>
    /// Key function used to update the keyboard layout.
    /// </summary>
    [CreateAssetMenu(fileName = "Layout Function", menuName = "XR/Spatial Keyboard/Layout Key Function", order = 1)]
    public class LayoutFunction : KeyFunction
    {
        /// <inheritdoc />
        public override void ProcessKey(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            if (keyboardContext != null)
                keyboardContext.UpdateLayout(key.GetEffectiveCharacter());
        }
    }
}
#endif
