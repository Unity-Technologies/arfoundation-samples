using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class TestBodyAnchorScale : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARHumanBodyManager which will produce frame events.")]
    ARHumanBodyManager m_HumanBodyManager;

     /// <summary>
    /// Get or set the <c>ARHumanBodyManager</c>.
    /// </summary>
    public ARHumanBodyManager humanBodyManager
    {
        get { return m_HumanBodyManager; }
        set { m_HumanBodyManager = value; }
    }

    [SerializeField]
    Text m_ImageInfo;

    /// <summary>
    /// The UI Text used to display information about the image on screen.
    /// </summary>
    public Text imageInfo
    {
        get { return m_ImageInfo; }
        set { m_ImageInfo = value; }
    }

    void OnEnable()
    {
        Debug.Assert(m_ImageInfo != null, "need a text field");
        m_ImageInfo.enabled = true;

        Debug.Assert(m_HumanBodyManager != null, "Human body manager is required.");
        m_HumanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    void OnDisable()
    {
        Debug.Assert(m_ImageInfo != null, "need a text field");
        m_ImageInfo.enabled = false;

        Debug.Assert(m_HumanBodyManager != null, "Human body manager is required.");
        m_HumanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }

    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        bool hasScale = false;
        float scale = 0.0f;

        foreach (var humanBody in eventArgs.added)
        {
            if (!hasScale)
            {
                scale = humanBody.estimatedHeightScaleFactor;
                hasScale = true;
            }
        }

        foreach (var humanBody in eventArgs.updated)
        {
            if (!hasScale)
            {
                scale = humanBody.estimatedHeightScaleFactor;
                hasScale = true;
            }
        }

        Debug.Assert(m_ImageInfo != null, "need a text field");
        m_ImageInfo.text = hasScale ? scale.ToString("F10") : "<no scale>";
    }
}
