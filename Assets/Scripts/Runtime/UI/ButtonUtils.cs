using TMPro;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public static class ButtonUtils
    {
        public static readonly Color enabledButtonGraphicColor = new(228f / 255, 228f / 255, 228f / 255, 1.0f);

        public static readonly Color disabledButtonGraphicColor = new(228f / 255, 228f / 255, 228f / 255, 0.1f);

        public static void SetEnabled(this Button button, bool isOn)
        {
            button.interactable = isOn;

            SetEnabledTextState(button, isOn);
            SetEnabledImageState(button, isOn);
        }

        static void SetEnabledTextState(Button button, bool isOn)
        {
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text == null)
                return;

            text.color = isOn ? enabledButtonGraphicColor : disabledButtonGraphicColor;
        }

        static void SetEnabledImageState(Button button, bool isOn)
        {
            var images = button.GetComponentsInChildren<Image>();
            if (images == null)
                return;

            foreach (var image in images)
            {
                if (image.gameObject == button.gameObject)
                    continue;

                image.color = isOn ? enabledButtonGraphicColor : disabledButtonGraphicColor;
            }
        }
    }
}
