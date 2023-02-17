using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(RectTransform))]
    public class MenuFollowCamera : MonoBehaviour
    {
        bool m_CameraFollow;

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
            var menu = GetComponent<Canvas>();
            var scaler = GetComponent<CanvasScaler>();

#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
            menu.renderMode = RenderMode.ScreenSpaceOverlay;
            scaler.matchWidthOrHeight = .5f;
#else
            m_CameraFollow = true;
            menu.renderMode = RenderMode.WorldSpace;
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 575);
#endif
        }

        void Update()
        {
            if (!m_CameraFollow)
                return;

            var cameraTransform = m_CameraAR.transform;
            Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * distance;
            Vector3 currentPosition = transform.position;

            transform.SetPositionAndRotation(
                Vector3.Lerp(currentPosition, targetPosition, m_SmoothFactor),
                cameraTransform.rotation);

            float height;
            if (m_CameraAR.orthographic)
                height = m_CameraAR.orthographicSize * 2;
            else
                height = distance * Mathf.Tan(Mathf.Deg2Rad * (m_CameraAR.fieldOfView * 0.5f));

            float heightScale = height / m_CameraAR.scaledPixelHeight;
            transform.localScale = new Vector3(heightScale, heightScale, 1);
        }
    }
}
