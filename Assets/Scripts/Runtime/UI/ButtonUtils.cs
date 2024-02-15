using TMPro;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public static class ButtonUtils
    {
        static readonly Color s_DisabledButtonTextColor = new(228f / 255, 228f / 255, 228f / 255, .5f);

        public static void DisableButton(Button button)
        {
            button.interactable = false;

            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text == null)
                return;

            text.color = s_DisabledButtonTextColor;
        }
    }
}
