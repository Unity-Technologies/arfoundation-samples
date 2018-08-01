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
		
		SetUIValue(m_LightEstimation.Brightness.HasValue, m_BrightnessVal, m_LightEstimation.Brightness.Value.ToString());
		SetUIValue(m_LightEstimation.ColorTemperature.HasValue, m_ColorTempVal, m_LightEstimation.ColorTemperature.Value.ToString());
		SetUIValue(m_LightEstimation.ColorCorrection.HasValue, m_ColorCorrectVal, m_LightEstimation.ColorTemperature.Value.ToString());
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
