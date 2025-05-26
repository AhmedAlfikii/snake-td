using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

namespace TafraKit.UI
{
    public static class TafraUIUtilities
    {
        public static void PlaceRectOnRectEdge(RectTransform rectToPlace, RectTransform rectToPlaceOn, Side side)
        {
            float lossyScale = rectToPlaceOn.lossyScale.x;

            Vector2 rectSize = new Vector2(rectToPlaceOn.rect.width * lossyScale, rectToPlaceOn.rect.height * lossyScale);

            Vector2 centerPosition = new Vector2(
                rectToPlaceOn.position.x - ((rectToPlaceOn.pivot.x - 0.5f) * rectSize.x),
                rectToPlaceOn.position.y - ((rectToPlaceOn.pivot.y - 0.5f) * rectSize.y));

            Vector2 finalPosition = centerPosition;

            switch (side)
            {
                case Side.Left:
                    finalPosition.x -= rectSize.x / 2f;
                    break;
                case Side.Right:
                    finalPosition.x += rectSize.x / 2f;
                    break;
                case Side.Top:
                    finalPosition.y += rectSize.y / 2f;
                    break;
                case Side.Bottom:
                    finalPosition.y -= rectSize.y / 2f;
                    break;
                default:
                    break;
            }

            rectToPlace.position = finalPosition;
        }
    }
}