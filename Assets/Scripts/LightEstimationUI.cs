using System.Collections;
using System.Collections.Generic;
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
		SetUIValue(m_LightEstimation.brightness.HasValue, m_BrightnessVal, m_LightEstimation.brightness.Value.ToString());
		SetUIValue(m_LightEstimation.colorTemperature.HasValue, m_ColorTempVal, m_LightEstimation.colorTemperature.Value.ToString());
		SetUIValue(m_LightEstimation.colorCorrection.HasValue, m_ColorCorrectVal, m_LightEstimation.colorTemperature.Value.ToString());
	}

	void SetUIValue(bool ContainsValue, Text UIText, string DisplayValue)
	{
		if (UIText)
		{
			if (ContainsValue)
			{
				UIText.text = DisplayValue;
			}
			else
			{
				UIText.text = k_UnavailableText;
			}
		}
	}
	
}
