using UnityEngine.UI;
using System.Text;

using UnityEngine.Rendering;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// A simple UI controller to display HDR light estimation information.
    /// </summary>
    [RequireComponent(typeof(HDRLightEstimation))]
    public class HDRLightEstimationUI : MonoBehaviour
    {
        [Tooltip("The UI Text element used to display the estimated ambient intensity in the physical environment.")]
        [SerializeField]
        Text m_AmbientIntensityText;

        /// <summary>
        /// The UI Text element used to display the estimated ambient intensity value.
        /// </summary>
        public Text ambientIntensityText
        {
            get => m_AmbientIntensityText;
            set => m_AmbientIntensityText = ambientIntensityText;
        }

        [Tooltip("The UI Text element used to display the estimated ambient color in the physical environment.")]
        [SerializeField]
        Text m_AmbientColorText;

        /// <summary>
        /// The UI Text element used to display the estimated ambient color in the scene.
        /// </summary>
        public Text ambientColorText
        {
            get => m_AmbientColorText;
            set => m_AmbientColorText = value;
        }
        
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
            m_HDRLightEstimation = GetComponent<HDRLightEstimation>();
        }

        void Update()
        {
            //Display basic light estimation info
            SetUIValue(m_HDRLightEstimation.brightness, ambientIntensityText);
            //Display color temperature or color correction if supported
            if (m_HDRLightEstimation.colorTemperature != null)
                SetUIValue(m_HDRLightEstimation.colorTemperature, ambientColorText);
            else if (m_HDRLightEstimation.colorCorrection != null)
                SetUIValue(m_HDRLightEstimation.colorCorrection, ambientColorText);
            else
                SetUIValue<float>(null, ambientColorText);
            
            //Display HDR only light estimation info
            SetUIValue(m_HDRLightEstimation.mainLightDirection, mainLightDirectionText);
            SetUIValue(m_HDRLightEstimation.mainLightColor, mainLightColorText);
            SetUIValue(m_HDRLightEstimation.mainLightIntensityLumens, mainLightIntensityLumens);
            SetSphericalHarmonicsUIValue(m_HDRLightEstimation.sphericalHarmonics, ambientSphericalHarmonicsText);
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

        HDRLightEstimation m_HDRLightEstimation;
    }
}
