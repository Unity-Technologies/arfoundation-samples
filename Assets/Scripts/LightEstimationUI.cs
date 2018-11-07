using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LightEstimation))]
public class LightEstimationUI : MonoBehaviour
{
    [SerializeField]
    Text m_BrightnessVal;
    
    [SerializeField]
    Text m_ColorTempVal;
    
    [SerializeField]
    Text m_ColorCorrectVal;

    LightEstimation m_LightEstimation;

    const string k_UnavailableText = "Unavailable";

    void Awake()
    {
        m_LightEstimation = GetComponent<LightEstimation>();
    }

    void Update()
    {
        SetUIValue(m_LightEstimation.brightness, m_BrightnessVal);
        SetUIValue(m_LightEstimation.colorTemperature, m_ColorTempVal);
        SetUIValue(m_LightEstimation.colorCorrection, m_ColorCorrectVal);
    }

    void SetUIValue<T>(T? displayVar, Text uiText) where T : struct
    {
        if (uiText)
        {
            if (displayVar.HasValue)
            {
                uiText.text = displayVar.Value.ToString();
            }
            else
            {
                uiText.text = k_UnavailableText;
            }
        }
    }
    
}
