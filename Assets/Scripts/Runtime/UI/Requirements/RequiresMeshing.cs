using System.Collections;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Button))]
    public class RequiresMeshing : MonoBehaviour
    {
        static bool s_MeshingSupported;
        static bool s_MeshingChecked;

        Button m_Button;

        IEnumerator Start()
        {
            m_Button = GetComponent<Button>();
            yield return null;

            if (!s_MeshingChecked)
            {
                s_MeshingChecked = true;
                var activeLoader = LoaderUtility.GetActiveLoader();
                if(activeLoader && activeLoader.GetLoadedSubsystem<XRMeshSubsystem>() != null)
                    s_MeshingSupported = true;
            }

            if (!s_MeshingSupported)
                ARSceneSelectUI.DisableButton(m_Button);
        }
    }
}
