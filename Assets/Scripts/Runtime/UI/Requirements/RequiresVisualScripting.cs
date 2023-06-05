using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Button))]
    public class RequiresVisualScripting : MonoBehaviour
    {
        void Start()
        {
#if VISUALSCRIPTING_1_8_OR_NEWER
            return;
#endif
#pragma warning disable CS0162
            ARSceneSelectUI.DisableButton(GetComponent<Button>());
#pragma warning restore CS0162
        }
    }
}
