namespace UnityEngine.XR.ARFoundation.Samples
{
    public class DropdownFitter : MonoBehaviour
    {
        const float k_MinWindowHeight = 50f;

        [Tooltip("The dropdown's template.")]
        [SerializeField]
        RectTransform m_TemplateRect;

        [Tooltip("Drag the main parent Canvas here")]
        [SerializeField]
        Canvas m_Canvas;

        [Tooltip("Optional padding in canvas space units to keep the menu slightly above the safe area line")]
        [SerializeField]
        float m_BottomPadding = 10f;

        void Start()
        {
            var corners = new Vector3[4];
            m_TemplateRect.GetWorldCorners(corners);

            var topEdgeWorld = corners[1];

            var canvasRect = m_Canvas.GetComponent<RectTransform>();
            var topEdgeCanvasSpace = canvasRect.InverseTransformPoint(topEdgeWorld);

            var safeAreaBottomScreenPos = new Vector2(Screen.width / 2f, Screen.safeArea.yMin);

            var camera = m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : m_Canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                safeAreaBottomScreenPos,
                camera,
                out var safeAreaBottomCanvasSpace);

            var maxAvailableHeight = topEdgeCanvasSpace.y - safeAreaBottomCanvasSpace.y;

            maxAvailableHeight -= m_BottomPadding;

            // clamp max height to be a minimum of k_MinWindowHeight (canvas space units) in case the dropdown windows
            // top edge is below the bottom of the screen.
            maxAvailableHeight = Mathf.Max(maxAvailableHeight, k_MinWindowHeight);

            m_TemplateRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxAvailableHeight);
        }
    }
}
