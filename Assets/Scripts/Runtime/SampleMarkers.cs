using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class SampleMarkers : MonoBehaviour
    {
        [Serializable]
        public class MarkerData
        {
            [SerializeField]
            string m_Title;

            [SerializeField, TextArea(2, 5)]
            string m_Description;

            [SerializeField]
            Texture2D m_MarkerTexture;
        }

        [SerializeField]
        List<MarkerData> m_QRCodes = new();
    }
}
