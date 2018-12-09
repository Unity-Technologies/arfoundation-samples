using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LightEstimation))]
public class LightEstimationUI : MonoBehaviour
{
    [SerializeField]
    Text m_BrightnessText;

    public Text brightnessText
    {
        get { return m_BrightnessText; }
        set { m_BrightnessText = brightnessText; }
    }

    [SerializeField]
    Text m_ColorTemperatureText;

    public Text colorTemperatureText
    {
        get { return m_ColorTemperatureText; }
        set { m_ColorTemperatureText = value; }
    }

    [SerializeField]
    Text m_ColorCorrectionText;

    public Text colorCorrectionText
    {
        get { return m_ColorCorrectionText; }
        set { m_ColorCorrectionText = value; }
    }

    LightEstimation m_LightEstimation;

    const string k_UnavailableText = "Unavailable";

    void Awake()
    {
        m_LightEstimation = GetComponent<LightEstimation>();
    }

    void Update()
    {
        SetUIValue(m_LightEstimation.brightness, brightnessText);
        SetUIValue(m_LightEstimation.colorTemperature, colorTemperatureText);
        SetUIValue(m_LightEstimation.colorCorrection, colorCorrectionText);
    }

    void SetUIValue<T>(T? displayValue, Text text) where T : struct
    {
        if (text != null)
            text.text = displayValue.HasValue ? displayValue.Value.ToString()
                : k_UnavailableText;
    }
}
