using System;
using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CustomTMPDropdown : TMP_Dropdown
    {
        public event Action dropdownOpened;
        public event Action dropdownClosed;

        protected override GameObject CreateBlocker(Canvas rootCanvas)
        {
            dropdownOpened?.Invoke();
            return base.CreateBlocker(rootCanvas);
        }

        protected override void DestroyBlocker(GameObject blocker)
        {
            dropdownClosed?.Invoke();
            base.DestroyBlocker(blocker);
        }
    }
}
