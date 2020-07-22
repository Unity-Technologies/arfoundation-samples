using UnityEngine;
using UnityEngine.UI;
using System.Text;

using UnityEngine.Rendering;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// A simple UI controller to display light estimation information.
    /// </summary>
    [RequireComponent(typeof(LightEstimation))]
    public class HDRLightEstimationUI : MonoBehaviour
    {
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
        StringBuilder m_SphericalHarmonicsStringBuilder = new StringBuilder("");

        void Awake()
        {
            m_LightEstimation = GetComponent<LightEstimation>();
        }

        void Update()
        {
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
                    m_SphericalHarmonicsStringBuilder.Clear();
                    for (int i = 0; i < 3; ++i)
                    {
                        if (i == 0)
                            m_SphericalHarmonicsStringBuilder.Append("R:[");
                        else if (i == 1)
                            m_SphericalHarmonicsStringBuilder.Append("G:[");
                        else
                            m_SphericalHarmonicsStringBuilder.Append("B:[");

                        for (int j = 0; j < 9; ++j)
                        {
                            m_SphericalHarmonicsStringBuilder.Append(j != 8 ? $"{maybeAmbientSphericalHarmonics.Value[i, j]}, " : $"{maybeAmbientSphericalHarmonics.Value[i, j]}]\n");
                        }
                    }
                    text.text = m_SphericalHarmonicsStringBuilder.ToString();
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
}
