using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// An <see cref="XRSimpleInteractable"/> that disables XRI selection, keeping it hover-only.
    /// Used on the composition layer quad so trigger presses drive UGUI events without
    /// causing <c>hasSelection = true</c> on the real interactor, which would exit hover via
    /// <c>XRRayInteractor.CanHover</c> and freeze the <c>InteractableUIMirror</c> proxy.
    /// </summary>
    public class HoverOnlyXRInteractable : XRSimpleInteractable
    {
        public override bool IsSelectableBy(IXRSelectInteractor interactor) => false;
    }
}
