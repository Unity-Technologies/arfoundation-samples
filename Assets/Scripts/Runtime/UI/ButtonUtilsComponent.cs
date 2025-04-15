using TMPro;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Exposes <see cref="ButtonUtils"/> via public instance methods, so that they can be used via `UnityEvent`.
    /// </summary>
    public class ButtonUtilsComponent : MonoBehaviour
    {
        public void DisableButton(Button button)
        {
            button.SetEnabled(false);
        }

        public void EnableButton(Button button)
        {
            button.SetEnabled(true);
        }

        public void DisableText(TextMeshProUGUI text)
        {
            text.color = ButtonUtils.disabledButtonGraphicColor;
        }

        public void EnableText(TextMeshProUGUI text)
        {
            text.color = ButtonUtils.enabledButtonGraphicColor;
        }
    }
}
