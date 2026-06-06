using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MultiGraphicButton : Button
    {
        [Tooltip("Drag the extra images/text you want to tint here.")]
        [SerializeField]
        Graphic[] m_ExtraGraphics;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            var color = state switch
            {
                SelectionState.Normal => colors.normalColor,
                SelectionState.Highlighted => colors.highlightedColor,
                SelectionState.Pressed => colors.pressedColor,
                SelectionState.Selected => colors.selectedColor,
                SelectionState.Disabled => colors.disabledColor,
                _ => colors.normalColor
            };

            if (m_ExtraGraphics == null)
                return;

            var duration = instant ? 0f : colors.fadeDuration;
            foreach (var graphic in m_ExtraGraphics)
            {
                if (graphic == null)
                    continue;

                graphic.CrossFadeColor(color * colors.colorMultiplier, duration, true, true);
            }
        }
    }
}
