using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CornerVisualizerSizeController : MonoBehaviour
    {
        [FormerlySerializedAs("m_CornerAxis")]
        [SerializeField]
        Transform m_XAxis;

        [SerializeField]
        Transform m_YAxis;

        [SerializeField]
        Transform m_ZAxis;

        float m_DefaultLength;
        float m_Sign;

        void Reset()
        {
            if (m_XAxis != null)
                m_DefaultLength = m_XAxis.localScale.x;
        }

        void Awake()
        {
            if (m_XAxis == null)
                Debug.Log($"{nameof(m_XAxis)} is null.");

            if (m_YAxis == null)
                Debug.Log($"{nameof(m_YAxis)} is null.");

            if (m_ZAxis == null)
                Debug.Log($"{nameof(m_ZAxis)} is null.");

            if (m_XAxis != null)
                m_DefaultLength = m_XAxis.localScale.x;

            if (m_XAxis != null)
                m_Sign = m_XAxis.localPosition.x > 0 ? 1 : -1;
        }

        public void SetMaxLength(float length)
        {
            SetXAxisVisualizer(length);
            SetYAxisVisualizer(length);
            SetZAxisVisualizer(length);
        }

        void SetXAxisVisualizer(float length)
        {
            if (length < m_DefaultLength)
            {
                var size = m_XAxis.localScale;
                size.x = length;
                m_XAxis.localScale = size;

                var position = m_XAxis.localPosition;
                position.x = m_XAxis.localScale.x * 0.5f * m_Sign;
                m_XAxis.localPosition = position;
            }
            else
            {

                var size = m_XAxis.localScale;
                size.x = m_DefaultLength;
                m_XAxis.localScale = size;

                var position = m_XAxis.localPosition;
                position.x = m_XAxis.localScale.x * 0.5f * m_Sign;
                m_XAxis.localPosition = position;
            }
        }
        
        void SetYAxisVisualizer(float length)
        {
            if (length < m_DefaultLength)
            {
                var size = m_YAxis.localScale;
                size.y = length;
                m_YAxis.localScale = size;

                var position = m_YAxis.localPosition;
                position.y = m_YAxis.localScale.y * 0.5f * m_Sign;
                m_YAxis.localPosition = position;
            }
            else
            {
                var size = m_YAxis.localScale;
                size.y = m_DefaultLength;
                m_YAxis.localScale = size;

                var position = m_YAxis.localPosition;
                position.y = m_YAxis.localScale.y * 0.5f * m_Sign;
                m_YAxis.localPosition = position;
            }
        }

        void SetZAxisVisualizer(float length)
        {
            if (length < m_DefaultLength)
            {
                var size = m_ZAxis.localScale;
                size.z = length;
                m_ZAxis.localScale = size;

                var position = m_ZAxis.localPosition;
                position.z = m_ZAxis.localScale.z * 0.5f * m_Sign;
                m_ZAxis.localPosition = position;
            }
            else
            {
                var size = m_ZAxis.localScale;
                size.z = m_DefaultLength;
                m_ZAxis.localScale = size;

                var position = m_ZAxis.localPosition;
                position.z = m_ZAxis.localScale.z * 0.5f * m_Sign;
                m_ZAxis.localPosition = position;
            }
        }
    }
}
