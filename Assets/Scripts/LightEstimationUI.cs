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

	const string k_UnsupportedText = "Not Supported";

	void Awake()
	{
		m_LightEstimation = GetComponent<LightEstimation>();
	}

	void Update()
	{
		if (m_LightEstimation.Brightness.HasValue)
		{
			m_BrightnessVal.text = m_LightEstimation.Brightness.Value.ToString();
		}
		else
		{
			m_BrightnessVal.text = k_UnsupportedText;
		}
		
		if (m_LightEstimation.ColorTemperature.HasValue)
		{
			m_ColorTempVal.text = m_LightEstimation.ColorTemperature.Value.ToString();
		}
		else
		{
			m_ColorTempVal.text = k_UnsupportedText;
		}

		if (m_LightEstimation.ColorCorrection.HasValue)
		{
			m_ColorCorrectVal.text = m_LightEstimation.ColorCorrection.Value.ToString();
		}
		else
		{
			m_ColorCorrectVal.text = k_UnsupportedText;
		}
	}
	
}
