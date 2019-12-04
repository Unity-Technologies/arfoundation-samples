using UnityEngine;
using UnityEngine.UI;

using SphericalHarmonicsL2 = UnityEngine.Rendering.SphericalHarmonicsL2;

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

    [Tooltip("The UI Text element used to display the estimated direction of the main light for the physical environment.")]
    [SerializeField]
    Text m_MainLightDirectionText;
    public Text mainLightDirectionText
    {
        get => m_MainLightDirectionText;
        set => m_MainLightDirectionText = value;
    }

    [Tooltip("The UI Text element used to display the estimated intensity in lumens of the main light for the physical environment.")]
    [SerializeField]
    Text m_MainLightIntensityLumens;
    public Text mainLightIntensityLumens
    {
        get => m_MainLightIntensityLumens;
        set => m_MainLightIntensityLumens = value;
    }

    [Tooltip("The UI Text element used to display the estimated color of the main light for the physical environment.")]
    [SerializeField]
    Text m_MainLightColor;
    public Text mainLightColorText
    {
        get => m_MainLightColor;
        set => m_MainLightColor = value;
    }

    [Tooltip("The UI Text element used to display the estimated spherical harmonics coefficients for the physical environment.")]
    [SerializeField]
    Text m_SphericalHarmonicsText;
    public Text ambientSphericalHarmonicsText
    {
        get => m_SphericalHarmonicsText;
        set => m_SphericalHarmonicsText = value;
    }

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
        SetUIValue(m_LightEstimation.mainLightDirection, mainLightDirectionText);
        SetUIValue(m_LightEstimation.mainLightColor, mainLightColorText);
        SetUIValue(m_LightEstimation.mainLightIntensityLumens, mainLightIntensityLumens);
        SetSphericalHarmonicsUIValue(m_LightEstimation.sphericalHarmonics, ambientSphericalHarmonicsText);
    }

    void SetSphericalHarmonicsUIValue(SphericalHarmonicsL2? maybeAmbientSphericalHarmonics, Text text)
    {
        if (text != null)
        {
            if (maybeAmbientSphericalHarmonics.HasValue)
            {
                string sphericalHarmonicsCoefficientsStr = "";
                if (m_LightEstimation.sphericalHarmonics.HasValue)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        if (i == 0)
                            sphericalHarmonicsCoefficientsStr += $"R: [";
                        else if (i == 1)
                            sphericalHarmonicsCoefficientsStr += $"G: [";
                        else
                            sphericalHarmonicsCoefficientsStr += $"B: [";
                        
                        for (int j = 0; j < 9; ++j)
                        {
                            sphericalHarmonicsCoefficientsStr += $"{m_LightEstimation.sphericalHarmonics.Value[i,j].ToString("0.000")}{(j != 8 ? ", " : "]\n")}";
                        }
                    }
                }
                text.text = sphericalHarmonicsCoefficientsStr;
            }
            else
            {
                text.text = k_UnavailableText;
            }
        }
    }

    void SetUIValue<T>(T? displayValue, Text text) where T : struct
    {
        if (text != null)
            text.text = displayValue.HasValue ? displayValue.Value.ToString(): k_UnavailableText;
    }

    const string k_UnavailableText = "Unavailable";

    LightEstimation m_LightEstimation;
}
