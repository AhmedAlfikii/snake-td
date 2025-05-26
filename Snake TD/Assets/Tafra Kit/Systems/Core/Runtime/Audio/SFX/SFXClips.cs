using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [Serializable]
    public class SFXClips : ISFX
    {
        public AudioClip[] Clips;
        public FloatRange VolumeRange;
        public FloatRange PitchRange;
        public FloatRange FadeInDurationRange;
        public FloatRange DelayRange;

        public SFXClips()
        {
            Clips = new AudioClip[0];
            VolumeRange = new FloatRange(1, 1);
            PitchRange = new FloatRange(1, 1);
        }
        public SFXClips(AudioClip[] clip, FloatRange volumeRange, FloatRange pitchRange)
        {
            Clips = clip;
            VolumeRange = volumeRange;
            PitchRange = pitchRange;
        }

        public AudioClip GetClip()
        {
            if (Clips.Length == 0)
                return null;

            return Clips[UnityEngine.Random.Range(0, Clips.Length)];
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