using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    [RequireComponent(typeof(AudioSource))]
    public class TafraAudioSource : MonoBehaviour
    {
        private AudioSource myAS;
        private int curAudioId;
        private float curTargetVolume;
        private IEnumerator waitingUntilFinishedEnum;
        private IEnumerator fadingOutStopEnum;
        private IEnumerator volumeFadingEnum;
        private IEnumerator playingDelayedEnum;

        public IntUnityEvent OnCompleted = new IntUnityEvent();
        public IntUnityEvent OnStopped = new IntUnityEvent();
        public IntFloatUnityEvent OnStartedStopping = new IntFloatUnityEvent();

        public bool IsMuted
        {
            get
            {
                if (myAS == null)
                    myAS = GetComponent<AudioSource>();

                return myAS.mute;
            }
            set
            {
                if (myAS == null)
                    myAS = GetComponent<AudioSource>();

                myAS.mute = value;
            }
        }

        private void Awake()
        {
            if (myAS == null)
                myAS = GetComponent<AudioSource>();
        }

        IEnumerator WaitUntilFinished(AudioClip clip, float delay)
        {
            float fullDelay = delay;

            if (clip != null)
                fullDelay += clip.length;

            yield return Yielders.GetWaitForSecondsRealtime(fullDelay);

            while (myAS.isPlaying)
                yield return null;

            myAS.clip = null;

            OnCompleted.Invoke(curAudioId);
        }
        IEnumerator FadeOutStop(float duration)
        {
            float startTime = Time.unscaledTime;
            float endTime = startTime + duration;
            float startVolume = myAS.volume;

            while (Time.unscaledTime < endTime)
            {
                float t = (Time.unscaledTime - startTime) / duration;

                t = MotionEquations.EaseInOut(t, 2);

                myAS.volume = Mathf.Lerp(startVolume, 0, t);

                yield return null;
            }

            myAS.volume = 0;

            Stop();
        }
        IEnumerator FadingVolume(float startVolme, float finalVolume, float duration, float delay)
        {
            if (delay > 0.001f)
                yield return Yielders.GetWaitForSecondsRealtime(delay);

            float startTime = Time.unscaledTime;
            float endTime = startTime + duration;

            while (Time.unscaledTime < endTime)
            {
                float t = (Time.unscaledTime - startTime) / duration;

                t = MotionEquations.EaseInOut(t, 2);

                myAS.volume = Mathf.Lerp(startVolme, finalVolume, t);

                yield return null;
            }

            myAS.volume = finalVolume;
        }
        /// <summary>
        /// Does the same as AudioSource.PlayDelayed, except this one can be stopped.
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        IEnumerator PlayDelayed(float delay)
        {
            yield return Yielders.GetWaitForSecondsRealtime(delay);
           
            myAS.Play();
        }

        public void Play(AudioClip clip, int id, bool loop = false, float volume = 1, float pitch = 1, float fadeInDuration = 0, float delay = 0)
        {
            StopAllCoroutines();

            myAS.clip = clip;
            myAS.volume = volume;
            myAS.pitch = pitch;
            myAS.loop = loop;
            
            curTargetVolume = volume;

            curAudioId = id;

            if (delay < 0.001f)
                myAS.Play();
            else
            {
                playingDelayedEnum = PlayDelayed(delay);
                StartCoroutine(playingDelayedEnum);
            }

            if (fadeInDuration > 0.001f)
            {
                volumeFadingEnum = FadingVolume(0, volume, fadeInDuration, delay);
                StartCoroutine(volumeFadingEnum);
            }

            if (loop == false)
            {
                waitingUntilFinishedEnum = WaitUntilFinished(clip, delay);

                StartCoroutine(waitingUntilFinishedEnum);
            }
        }

        public void Stop(float fadeOutDuration = 0)
        {
            StopAllCoroutines();

            if (fadeOutDuration < 0.001f)
            {
                myAS.Stop();
                myAS.clip = null;

                OnStopped?.Invoke(curAudioId);
            }
            else
            {
                fadingOutStopEnum = FadeOutStop(fadeOutDuration);
                StartCoroutine(fadingOutStopEnum);

                OnStartedStopping?.Invoke(curAudioId, fadeOutDuration);
            }
        }

        public void FadeOutVolume(float duration, float delay = 0)
        {
            if (volumeFadingEnum != null)
                StopCoroutine(volumeFadingEnum);

            volumeFadingEnum = FadingVolume(myAS.volume, 0, duration, delay);
            StartCoroutine(volumeFadingEnum);
        }

        public void FadeInVolume(float duration, float delay = 0)
        {
            if (volumeFadingEnum != null)
                StopCoroutine(volumeFadingEnum);

            volumeFadingEnum = FadingVolume(myAS.volume, curTargetVolume, duration, delay);
            StartCoroutine(volumeFadingEnum);
        }

        public void FadeVolume(float volume, float duration, float delay = 0)
        {
            if (volumeFadingEnum != null)
                StopCoroutine(volumeFadingEnum);

            volumeFadingEnum = FadingVolume(myAS.volume, volume, duration, delay);
            StartCoroutine(volumeFadingEnum);
        }

        public int GetAudioID()
        {
            return curAudioId;
        }
        public AudioClip GetPlayingClip()
        {
            if (myAS == null)
                return null;

            return myAS.clip;
        }
    }
}