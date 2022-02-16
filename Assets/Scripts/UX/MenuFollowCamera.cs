using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
public class MenuFollowCamera : MonoBehaviour
{
    [SerializeField]
    float m_SmoothFactor;

    public float smoothFactor
    {
        get => m_SmoothFactor;
        set => m_SmoothFactor = value;
    }

    [SerializeField]
    float m_Distance;

    public float distance
    {
        get => m_Distance;
        set => m_Distance = value;
    }

    [SerializeField]
    Camera m_CameraAR;

    public Camera cameraAR
    {
        get => m_CameraAR;
        set => m_CameraAR = value;
    }

    void Start()
    {
        Canvas menu = GetComponent<Canvas>();

#if UNITY_IOS || UNITY_ANDROID
        menu.renderMode = RenderMode.ScreenSpaceOverlay;
#else
        m_CameraFollow = true;
        menu.renderMode = RenderMode.WorldSpace;
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 575);
#endif
    }

    void Update()
    {
        if(m_CameraFollow)
        {
            Vector3 targetPosition = m_CameraAR.transform.position + m_CameraAR.transform.forward * distance;
            Vector3 currentPosition = transform.position;

            transform.position = Vector3.Lerp(currentPosition, targetPosition, m_SmoothFactor);
            transform.rotation = m_CameraAR.transform.rotation;

            float height = 0;
            if (m_CameraAR.orthographic)
                height = m_CameraAR.orthographicSize * 2;
            else
                height = distance * Mathf.Tan(Mathf.Deg2Rad * (m_CameraAR.fieldOfView * 0.5f));

            float heightScale = height / m_CameraAR.scaledPixelHeight;
            transform.localScale = new Vector3(heightScale, heightScale, 1);
        }
    }

    bool m_CameraFollow;
}
