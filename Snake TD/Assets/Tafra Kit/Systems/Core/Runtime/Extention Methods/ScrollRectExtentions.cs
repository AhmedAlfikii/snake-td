using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit
{
    public static class ScrollRectExtentions
    {
        public static void GoToElement(this ScrollRect scrollRect, Transform element)
        {
            RectTransform content = scrollRect.content;
            content.position = GetContentPositionToCenterElement(scrollRect, element);
        }
        //TODO: do this in a cleaner way.
        /// <summary>
        /// Returns the position the content rect should be at in order to center the target element in the scroll view.
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Vector3 GetContentPositionToCenterElement(this ScrollRect scrollRect, Transform element, bool clamp = true)
        {
            RectTransform viewPort = scrollRect.viewport;
            RectTransform content = scrollRect.content;

            float lossyScale = content.lossyScale.x;

            float contentWidth = content.rect.width * lossyScale;
            float contentHeight = content.rect.height * lossyScale;
            Vector3 contentPivotedPosition = content.position;
            Vector3 contentMin = new Vector3(contentPivotedPosition.x - content.pivot.x * contentWidth, contentPivotedPosition.y - content.pivot.y * contentHeight, 0);
            Vector3 contentMax = new Vector3(contentPivotedPosition.x + (1 - content.pivot.x) * contentWidth, contentPivotedPosition.y + (1 - content.pivot.y) * contentHeight, 0);
            Vector3 contentCenterPoint = new Vector3((contentMin.x + contentMax.x) / 2f, (contentMin.y + contentMax.y) / 2f);

            float viewportWidth = viewPort.rect.width * lossyScale;
            float viewportHeight = viewPort.rect.height * lossyScale;
            Vector3 viewportPivotPoint = viewPort.position;
            Vector3 viewportMin = new Vector3(viewportPivotPoint.x - viewPort.pivot.x * viewportWidth, viewportPivotPoint.y - viewPort.pivot.y * viewportHeight, 0);
            Vector3 viewportMax = new Vector3(viewportPivotPoint.x + (1 - viewPort.pivot.x) * viewportWidth, viewportPivotPoint.y + (1 - viewPort.pivot.y) * viewportHeight, 0);
            Vector3 viewportCenterPoint = new Vector3((viewportMin.x + viewportMax.x) / 2f, (viewportMin.y + viewportMax.y) / 2f);

            Vector3 elementPos = element.position;
            Vector3 diff = viewportCenterPoint - elementPos;

            if (!scrollRect.horizontal)
                diff.x = 0;
            if (!scrollRect.vertical)
                diff.y = 0;

            float contentCenterPosXMin = viewportCenterPoint.x - (contentWidth / 2f) + viewportWidth / 2f;
            float contentCenterPosXMax = viewportCenterPoint.x + (contentWidth / 2f) - viewportWidth / 2f;
            //TODO: support Y min max (do the same as the above but for Y).
            float contentCenterPosYMin = viewportCenterPoint.y - (contentHeight / 2f) + viewportHeight / 2f;
            float contentCenterPosYMax = viewportCenterPoint.y + (contentHeight / 2f) - viewportHeight / 2f;

            Vector3 newCenterPoint = contentCenterPoint + diff;

            if (clamp)
            {
                if (scrollRect.horizontal)
                    newCenterPoint.x = Mathf.Clamp(newCenterPoint.x, contentCenterPosXMin, contentCenterPosXMax);
                if (scrollRect.vertical)
                    newCenterPoint.y = Mathf.Clamp(newCenterPoint.y, contentCenterPosYMin, contentCenterPosYMax);
            }

            Vector3 newDiff = newCenterPoint - contentCenterPoint;

            return contentPivotedPosition + newDiff;
        }
    }
}