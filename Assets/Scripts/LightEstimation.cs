using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Light))]
public class LightEstimation : MonoBehaviour
{
	Light m_Light;

	public float? brightness { get; private set; }

	public float? colorTemperature { get; private set; }

	public Color? colorCorrection { get; private set; }

	void Awake ()
	{
		m_Light = GetComponent<Light>();
		ARSubsystemManager.cameraFrameReceived += FrameChanged;
	}

	void FrameChanged(ARCameraFrameEventArgs args)
	{
		if (args.lightEstimation.averageBrightness.HasValue)
		{
			brightness = args.lightEstimation.averageBrightness.Value;
			m_Light.intensity = brightness.Value;
		}

		if (args.lightEstimation.averageColorTemperature.HasValue)
		{
			colorTemperature = args.lightEstimation.averageColorTemperature.Value;
			m_Light.colorTemperature = colorTemperature.Value;
		}
		
		if (args.lightEstimation.colorCorrection.HasValue)
		{
			colorCorrection = args.lightEstimation.colorCorrection.Value;
			m_Light.color = colorCorrection.Value;
		}
	}
}









