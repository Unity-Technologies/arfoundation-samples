using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple UI controller to display light estimation information.
/// </summary>
[RequireComponent(typeof(LightEstimation))]
public class LightEstimationUI : MonoBehaviour
{
    [Tooltip("The UI Text element used to display the estimated brightness in the physical environment.")]
    [SerializeField]
    Text m_BrightnessText;

    /// <summary>
    /// The UI Text element used to display the estimated brightness value.
    /// </summary>
    public Text brightnessText
    {
        get { return m_BrightnessText; }
        set { m_BrightnessText = brightnessText; }
    }

    [Tooltip("The UI Text element used to display the estimated color temperature in the physical environment.")]
    [SerializeField]
    Text m_ColorTemperatureText;

    /// <summary>
    /// The UI Text element used to display the estimated color temperature in the scene.
    /// </summary>
    public Text colorTemperatureText
    {
        get { return m_ColorTemperatureText; }
        set { m_ColorTemperatureText = value; }
    }

    [Tooltip("The UI Text element used to display the estimated color correction value for the physical environment.")]
    [SerializeField]
    Text m_ColorCorrectionText;

    /// <summary>
    /// The UI Text element used to display the estimated color correction value for the scene.
    /// </summary>
    public Text colorCorrectionText
    {
        get { return m_ColorCorrectionText; }
        set { m_ColorCorrectionText = value; }
    }

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
            text.text = displayValue.HasValue ? displayValue.Value.ToString(): k_UnavailableText;
    }

    const string k_UnavailableText = "Unavailable";

    LightEstimation m_LightEstimation;
}
