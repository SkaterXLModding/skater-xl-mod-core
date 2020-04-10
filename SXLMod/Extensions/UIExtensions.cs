using UnityEngine;
using UnityEngine.UI;

public static class UIExtensions
{
    public static void SetAnchor(this RectTransform rt, float xMin, float yMin, float xMax, float yMax)
    {
        rt.anchorMin = new Vector2(xMin, yMin);
        rt.anchorMax = new Vector2(xMax, yMax);
    }

    public static void SetOffset(this RectTransform rt, float left, float top, float right, float bottom)
    {
        rt.offsetMin = new Vector2(left, top);
        rt.offsetMax = new Vector2(-right, -bottom);
    }
}
