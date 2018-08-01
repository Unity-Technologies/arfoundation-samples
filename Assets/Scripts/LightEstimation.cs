using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Light))]
public class LightEstimation : MonoBehaviour
{
	Light m_Light;

	float? m_Brightness;

	public float? Brightness
	{
		get { return m_Brightness; }
	}

	float? m_ColorTemperature;

	public float? ColorTemperature
	{
		get { return m_ColorTemperature; }
	}

	Color? m_ColorCorrection;

	public Color? ColorCorrection
	{
		get { return m_ColorCorrection; }
	}

	void Awake ()
	{
		m_Light = GetComponent<Light>();
		ARSubsystemManager.cameraFrameReceived += FrameChanged;
	}

	void FrameChanged(ARCameraFrameEventArgs args)
	{
		if (args.lightEstimation.averageBrightness.HasValue)
		{
			m_Brightness = args.lightEstimation.averageBrightness.Value;
			m_Light.intensity = m_Brightness.Value;
		}

		if (args.lightEstimation.averageColorTemperature.HasValue)
		{
			m_ColorTemperature = args.lightEstimation.averageColorTemperature.Value;
			m_Light.colorTemperature = m_ColorTemperature.Value;
		}
		
		if (args.lightEstimation.colorCorrection.HasValue)
		{
			m_ColorCorrection = args.lightEstimation.colorCorrection.Value;
			m_Light.color = m_ColorCorrection.Value;
		}
	}
}









