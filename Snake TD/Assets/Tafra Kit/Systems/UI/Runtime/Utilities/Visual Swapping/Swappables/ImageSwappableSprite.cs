using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.UI
{
    [System.Serializable]
    [SearchMenuItem("Image Swappable Sprite")]
    public class ImageSwappableSprite : UIGenericSwappableValues<Image, Sprite>
    {
        protected override void OnStateChange(int stateIndex)
        {
            target.sprite = stateValues[stateIndex];
        }
    }
}