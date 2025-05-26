using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public interface ISFX
    {
        public AudioClip GetClip();
        public float GetVolume();
        public float GetPitch();
        public float GetFadeInDuration();
        public float GetDelay();
    }
}