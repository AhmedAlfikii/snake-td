using System;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class OpacityMotionSelfReferences : MotionSelfReferences
    {
        public Graphic graphic;
        public CanvasGroup canvasGroup;

        public float GetCurrentOpacity()
        {
            if(graphic)
                return graphic.color.a;
            else if(canvasGroup)
                return canvasGroup.alpha;
            else
                return 0;
        }

        public void SetOpacity(float opacity)
        {
            if (canvasGroup)
            {
                canvasGroup.alpha = opacity;
            }
            else if (graphic)
            {
                Color color = graphic.color;
                color.a = opacity;
                graphic.color = color;
            }
        }

        public bool IsAvailable()
        {
            return graphic != null || canvasGroup != null;
        }
    }
}
