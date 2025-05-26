using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TafraKit
{
    [Serializable]
    public class SFXClip : ISFX
    {
        [FormerlySerializedAs("Clip")]
        [SerializeField] private AudioClip clip;
        [SerializeField] private SFXClipContainerSO clipSO;
        public FloatRange VolumeRange;
        public FloatRange PitchRange;
        public FloatRange FadeInDurationRange;
        public FloatRange DelayRange;

        public AudioClip Clip
        {
            set
            {
                clip = value;
            }
            get
            {
                if (clip != null) return clip;
                
                if (clipSO != null)
                {
                    return clipSO.GetClip();
                }

                return null;
            }
        }
        
        public SFXClip()
        {
            VolumeRange = new FloatRange(1, 1);
            PitchRange = new FloatRange(1, 1);
        }
        public SFXClip(AudioClip clip, FloatRange volumeRange, FloatRange pitchRange)
        {
            Clip = clip;
            VolumeRange = volumeRange;
            PitchRange = pitchRange;
        }
        public AudioClip GetClip()
        {
            if (clip != null) return clip;
                
            if (clipSO != null)
            {
                return clipSO.GetClip();
            }

            return null;
        }
        public float GetVolume()
        {
            return VolumeRange.GetRandomValue();
        }
        public float GetPitch()
        {
            return PitchRange.GetRandomValue();
        }
        public float GetFadeInDuration()
        {
            return FadeInDurationRange.GetRandomValue();
        }
        public float GetDelay()
        {
            return DelayRange.GetRandomValue();
        }

    }
}