using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.UI
{
    [System.Serializable]
    [SearchMenuItem("Graphic Swappable Color")]
    public class GraphicSwappableColor : UIGenericSwappableValues<Graphic, Color>
    {
        protected override void OnStateChange(int stateIndex)
        {
            target.color = stateValues[stateIndex];
        }
    }
}