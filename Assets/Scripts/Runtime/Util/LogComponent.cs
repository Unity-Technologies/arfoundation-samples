namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// A component to log something. This is useful to debug whether a Button.onClicked event is triggering.
    /// </summary>
    public class LogComponent : MonoBehaviour
    {
        public void Log(string s)
        {
            Debug.Log(s);
        }
    }
}
