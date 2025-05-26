using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.UI
{
    [System.Serializable]
    [SearchMenuItem("Canvas Group Alpha")]
    public class CanvasGroupSwappableAlpha : UIGenericSwappableValues<CanvasGroup, float>
    {
        protected override void OnStateChange(int stateIndex)
        {
            target.alpha = stateValues[stateIndex];
        }
    }
}