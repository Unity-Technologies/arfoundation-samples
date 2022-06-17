using System;
using UnityEngine;

public class TranslateGeometry : MonoBehaviour
{
    [SerializeField]
    float m_MinDistance = 1f;
    
    [SerializeField]
    float m_MaxDistance = 10f;

    void Awake()
    {
        if (m_MinDistance > m_MaxDistance)
        {
            (m_MaxDistance, m_MinDistance) = (m_MinDistance, m_MaxDistance);
        }
    }

    public void OnValueChanged(float tScalar)
    {
        var distance = Mathf.Lerp(m_MinDistance, m_MaxDistance, tScalar);
        var localPosition = transform.localPosition;
        localPosition = new Vector3(localPosition.x, localPosition.y, distance);
        transform.localPosition = localPosition;
    }
}
