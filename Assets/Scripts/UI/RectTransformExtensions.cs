namespace UnityEngine.XR.ARFoundation.Samples
{
    public static class RectTransformExtensions
    {
        static readonly Vector2 s_TopCenterAnchorPosition = new Vector2(.5f, 1);

        public static void SetSize(this RectTransform rt, Vector2 size)
        {
            var oldSize = rt.rect.size;
            var deltaSize = size - oldSize;
            rt.offsetMin -= new Vector2(deltaSize.x * rt.pivot.x, deltaSize.y * rt.pivot.y);
            rt.offsetMax += new Vector2(deltaSize.x * (1f - rt.pivot.x), deltaSize.y * (1f - rt.pivot.y));
        }

        public static void SetWidth(this RectTransform rt, float width)
        {
            SetSize(rt, new Vector2(width, rt.rect.size.y));
        }

        public static void SetHeight(this RectTransform rt, float height)
        {
            SetSize(rt, new Vector2(rt.rect.size.x, height));
        }

        public static void SetTopLeftPosition(this RectTransform rt, Vector2 pos)
        {
            rt.localPosition = new Vector3(
                pos.x + (rt.pivot.x * rt.rect.width),
                pos.y - ((1f - rt.pivot.y) * rt.rect.height),
                rt.localPosition.z);
        }

        public static void SetTopCenterPosition(this RectTransform rt, Vector2 pos)
        {
            rt.localPosition = new Vector3(
                pos.x + (rt.pivot.x * rt.rect.width / 2),
                pos.y - ((1f - rt.pivot.y) * rt.rect.height),
                rt.localPosition.z);
        }

        public static void AnchorToTopCenter(this RectTransform rt)
        {
            rt.anchorMin = s_TopCenterAnchorPosition;
            rt.anchorMax = s_TopCenterAnchorPosition;
        }
    }
}
