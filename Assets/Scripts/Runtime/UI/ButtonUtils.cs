using TMPro;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public static class ButtonUtils
    {
        public static readonly Color enabledButtonTextColor = new(228f / 255, 228f / 255, 228f / 255, 1.0f);

        public static readonly Color disabledButtonTextColor = new(228f / 255, 228f / 255, 228f / 255, 0.1f);

        public static void SetEnabled(this Button button, bool isOn)
        {
            button.interactable = isOn;

            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text == null)
                return;

            text.color = isOn ? enabledButtonTextColor : disabledButtonTextColor;
        }
    }
}
