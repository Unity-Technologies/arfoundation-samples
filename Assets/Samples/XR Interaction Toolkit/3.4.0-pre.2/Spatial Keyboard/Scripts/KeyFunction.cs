#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// Abstract class defining callbacks for key functionality. Allows users to extend
    /// custom functionality of keys and keyboard.
    /// </summary>
    public abstract class KeyFunction : ScriptableObject
    {
        /// <summary>
        /// Pre-process function when a key is pressed.
        /// </summary>
        /// <param name="keyboardContext">The current keyboard associated with <see cref="key"/>.</param>
        /// <param name="key">The key that is being pressed.</param>
        public virtual void PreprocessKey(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            if (keyboardContext != null)
                keyboardContext.PreprocessKeyPress(key);
        }

        /// <summary>
        /// Primary function callback when a key is pressed. Use this function to interface directly with a keyboard
        /// and process logic based on the current keyboard and key context.
        /// </summary>
        /// <param name="keyboardContext">The current keyboard associated with <see cref="key"/>.</param>
        /// <param name="key">The key that is being pressed.</param>
        public abstract void ProcessKey(XRKeyboard keyboardContext, XRKeyboardKey key);

        /// <summary>
        /// Post-process function when a key is pressed. This function calls <see cref="XRKeyboard.PostprocessKeyPress"/> on the keyboard.
        /// </summary>
        /// <param name="keyboardContext">The current keyboard associated with <see cref="key"/>.</param>
        /// <param name="key">The key that is being pressed.</param>
        public virtual void PostprocessKey(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            if (keyboardContext != null)
                keyboardContext.PostprocessKeyPress(key);
        }

        /// <summary>
        /// Uses keyboard and key context to determine if this key function should override the key's display icon.
        /// </summary>
        /// <param name="keyboardContext">Current keyboard context.</param>
        /// <param name="key">Current keyboard key.</param>
        /// <returns>Returns true if this key function should override the display icon.</returns>
        public virtual bool OverrideDisplayIcon(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            return false;
        }

        /// <summary>
        /// Returns display icon for this key function based on the context of the key and keyboard.
        /// </summary>
        /// <param name="keyboardContext">Current keyboard context.</param>
        /// <param name="key">Current keyboard key.</param>
        /// <returns>Returns display icon for this key.</returns>
        public virtual Sprite GetDisplayIcon(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            return null;
        }

        /// <summary>
        /// Allows this key function to process when a key is refreshing its display.
        /// </summary>
        /// <param name="keyboardContext">The current keyboard associated with <see cref="key"/>.</param>
        /// <param name="key">The key that is refreshing the display.</param>
        public virtual void ProcessRefreshDisplay(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
        }
    }
}
#endif
