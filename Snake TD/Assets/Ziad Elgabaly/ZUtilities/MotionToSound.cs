using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class MotionToSound : MonoBehaviour
    {
        #region Private Serialized Fields
        [Tooltip("The minimum change in motion at which a sound will be played.")]
        [SerializeField] private float minimumMotionThreshold = 0.01f;
        [Tooltip("The minimum time in seconds needed to pass since the last time a sound was played in order for it to stop.")]
        [SerializeField] private float minimumTimeToStop = 0.1f;
        [Tooltip("The pitch range between the minimum change in motion and maximum change in motion values.")]
        [SerializeField] private FloatRange pitchRange = new FloatRange(1f, 1.5f);
        [Tooltip("The volume range between the minimum change in motion and maximum change in motion values.")]
        [SerializeField] private FloatRange volumeRange = new FloatRange(0.3f, 1f);
        [Tooltip("The change in motion value that would result in the highest pitch and volume.")]
        [SerializeField] private float maximumChangeInMotion = 0.05f;
        [Tooltip("The minimum time in seconds required to start lowering down pitch and volume after the last time they were increased.")]
        [SerializeField] private float minimumTimeToLowerPitchVolume = 0.1f;

        [Header("End Clip")]
        [SerializeField] private AudioClip endClip;
        [SerializeField] private FloatRange endClipVolume = new FloatRange(1, 1);
        [SerializeField] private FloatRange endClipPitch = new FloatRange(1, 1);
        #endregion

        #region Private Fields
        private AudioSource myAS;
        private bool active;
        private float lastMotionValue;
        private float lastMotionDelta;
        private float lastHighestMotionDelta;
        private float sfxLastPlayTime;
        private float sfxLastIncreaseTime;
        #endregion

        #region MonoBehaviour Messages
        void Start()
        {
            myAS = GetComponent<AudioSource>();
        }
        #endregion

        #region Public Functions
        public void Activate()
        {
            active = true;
        }

        public void RecordMotion(float motion, bool suddenChange = false)
        {
            if (!active) return;

            float motionDelta = motion - lastMotionValue;

            if (motionDelta > minimumMotionThreshold)
            {
                if (!suddenChange && !myAS.isPlaying)
                {
                    myAS.Play();
                }
                else if (suddenChange)
                {
                    myAS.Stop();
                    myAS.Play();
                }

                float t = Mathf.Clamp01(motionDelta / maximumChangeInMotion);

                float pitch = pitchRange.Evaluate(t);
                float volume = volumeRange.Evaluate(t);

                if (motionDelta > lastHighestMotionDelta)
                {
                    myAS.pitch = pitchRange.Evaluate(t);
                    myAS.volume = volumeRange.Evaluate(t);

                    sfxLastIncreaseTime = Time.time;

                    lastHighestMotionDelta = motionDelta;
                }
                else if (Time.time > sfxLastIncreaseTime + minimumTimeToLowerPitchVolume)
                {
                    myAS.pitch = pitchRange.Evaluate(t);
                    myAS.volume = volumeRange.Evaluate(t);
                    lastHighestMotionDelta = 0;
                }

                sfxLastPlayTime = Time.time;
            }
            else
            {
                if (Time.time > sfxLastPlayTime + minimumTimeToStop && myAS.isPlaying)
                    myAS.Stop();
            }

            lastMotionValue = motion;
            lastMotionDelta = motionDelta;
        }

        public void End(bool silent = false)
        {
            if (myAS.isPlaying)
                myAS.Stop();

            if (!silent && endClip != null)
            {
                SFXPlayer.Play(endClip, endClipVolume.GetRandomValue(), endClipPitch.GetRandomValue());
            }

            lastMotionValue = 0;
            lastMotionDelta = 0;
            lastHighestMotionDelta = 0;

            active = false;
        }
        #endregion
    }
}