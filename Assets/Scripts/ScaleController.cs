using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARSessionOrigin))]
public class ScaleController : MonoBehaviour
{
    [SerializeField]
    Slider m_Slider;

    public Slider slider
    {
        get { return m_Slider; }
        set { m_Slider = value; }
    }

    [SerializeField]
    public float m_Min = .1f;

    public float min
    {
        get { return m_Min; }
        set { m_Min = value; }
    }

    [SerializeField]
    public float m_Max = 10f;

    public float max
    {
        get { return m_Max; }
        set { m_Max = value; }
    }

    public void SetScale()
    {
        if (slider == null)
            return;

        float scale = slider.value * (max - min) + min;
        m_SessionOrigin.transform.localScale = Vector3.one * scale;
    }

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
    }

    void OnEnable()
    {
        if (slider != null)
        {
            var scale = m_SessionOrigin.transform.localScale.x;
            slider.value = (scale - min) / (max - min);
        }
    }

    ARSessionOrigin m_SessionOrigin;
}
