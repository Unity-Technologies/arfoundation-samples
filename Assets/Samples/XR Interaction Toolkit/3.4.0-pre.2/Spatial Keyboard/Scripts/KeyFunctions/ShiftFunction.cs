#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard.KeyFunctions
{
    /// <summary>
    /// Key function used to process shift and caps lock functionality.
    /// </summary>
    [CreateAssetMenu(fileName = "Shift Function", menuName = "XR/Spatial Keyboard/Shift Key Function", order = 1)]
    public class ShiftFunction : KeyFunction
    {
        [SerializeField]
        Sprite m_CapsLockDisplayIcon;

        public Sprite capsLockDisplayIcon
        {
            get => m_CapsLockDisplayIcon;
            set => m_CapsLockDisplayIcon = value;
        }

        /// <inheritdoc />
        public override Sprite GetDisplayIcon(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            // This method won't be called unless OverrideDisplayIcon below returns true,
            // so no need for logic to return a shift display icon, which is already set up
            // as the default in the UI.
            return m_CapsLockDisplayIcon;
        }

        /// <inheritdoc />
        public override bool OverrideDisplayIcon(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            return keyboardContext != null && keyboardContext.capsLocked;
        }

        /// <inheritdoc />
        public override void ProcessKey(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            if (keyboardContext == null)
                return;

            var keyCode = KeyCode.LeftShift;

            // Check the caps lock state of the keyboard. If they key is shifted, check if there is a double click.
            if (keyboardContext.capsLocked || (keyboardContext.shifted && key.timeSinceLastClick < keyboardContext.doubleClickInterval))
                keyCode = KeyCode.CapsLock;

            keyboardContext.ProcessKeyCode(keyCode);
        }

        /// <inheritdoc />
        public override void PostprocessKey(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            base.PostprocessKey(keyboardContext, key);
            RefreshKeyHighlight(keyboardContext, key);
        }

        /// <inheritdoc />
        public override void ProcessRefreshDisplay(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            base.ProcessRefreshDisplay(keyboardContext, key);
            RefreshKeyHighlight(keyboardContext, key);
        }

        protected void RefreshKeyHighlight(XRKeyboard keyboardContext, XRKeyboardKey key)
        {
            if (keyboardContext == null)
                return;

            var highlight = keyboardContext.capsLocked || keyboardContext.shifted;
            key.EnableHighlight(highlight);
        }
    }
}
#endif
