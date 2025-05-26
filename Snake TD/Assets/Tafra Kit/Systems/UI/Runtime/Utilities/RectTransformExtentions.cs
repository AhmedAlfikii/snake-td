using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public static class RectTransformExtentions
    {
        /// <summary>
        /// Change the anchors of the rect transform without affecting its size and position.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="anchorMin"></param>
        /// <param name="anchorMax"></param>
        public static void SetAnchorsSoft(this RectTransform rt, Vector2 anchorMin, Vector2 anchorMax)
        {
            Vector2 originalSize = new Vector2(rt.rect.width, rt.rect.height);
            Vector3 originalPosition = rt.position;

            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;

            rt.sizeDelta = originalSize;
            rt.position = originalPosition;
        }

        /// <summary>
        /// Make this rect have the same properties as another one.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="target"></param>
        public static void AdaptRect(this RectTransform rt, RectTransform target)
        {
            rt.anchorMin = target.anchorMin;
            rt.anchorMax = target.anchorMax;

            rt.sizeDelta = target.sizeDelta;

            rt.position = target.position;

            rt.localScale = target.localScale;

            rt.rotation = target.rotation;
        }

        /// <summary>
        /// Make this rect have the same properties as another one.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="target"></param>
        public static void AnchorsAndRectFillParent(this RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;

            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }
    }
}