#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard.KeyFunctions
{
    /// <summary>
    /// Key function used to update the keyboard text with a string value.
    /// </summary>
    [CreateAssetMenu(fileName = "Value Key Function", menuName = "XR/Spatial Keyboard/Value Key Function", order = 1)]
    public class ValueKeyFunction : KeyFunction
    {
        /// <inheritdoc />
        public override void ProcessKey(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            if (keyboardContext != null)
                keyboardContext.UpdateText(key.GetEffectiveCharacter());
        }
    }
}
#endif
