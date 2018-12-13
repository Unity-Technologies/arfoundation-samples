using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// This example demonstrates disabling vertical planes as they are
/// detected and instantiated by the <c>ARPlaneManager</c>.
/// </summary>
[RequireComponent(typeof(ARPlaneManager))]
public class DisableVerticalPlanes : MonoBehaviour
{
    [Tooltip("The UI Text element used to display log messages.")]
    [SerializeField]
    Text m_LogText;

    /// <summary>
    /// The UI Text element used to display log messages.
    /// </summary>
    public Text logText
    {
        get { return m_LogText; }
        set { m_LogText = value; }
    }

    void OnEnable()
    {
        GetComponent<ARPlaneManager>().planeAdded += OnPlaneAdded;
    }

    void OnDisable()
    {
        GetComponent<ARPlaneManager>().planeAdded -= OnPlaneAdded;
    }

    void OnPlaneAdded(ARPlaneAddedEventArgs eventArgs)
    {
        var plane = eventArgs.plane;

        // Check whether the plane is a vertical plane.
        if (plane.boundedPlane.Alignment == PlaneAlignment.Vertical)
        {
            // Disable the entire GameObject.
            plane.gameObject.SetActive(false);

            // Add to our log so the user knows something happened.
            if (logText != null)
                logText.text = string.Format("\n{0}", plane.boundedPlane.Id);
        }
    }
}
