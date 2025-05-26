using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class CompactCouroutines
    {
        public delegate void Execution(float t);

        public static MonoBehaviour CoroutinePlayer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            GameObject coroutinePlayerGO = new GameObject("CompactCoroutinePlayer", typeof(EmptyMonoBehaviour));
            CoroutinePlayer = coroutinePlayerGO.GetComponent<EmptyMonoBehaviour>();
            GameObject.DontDestroyOnLoad(coroutinePlayerGO);
        }
        public static IEnumerator StartCompactCoroutine(MonoBehaviour player, float startAfter, float duration, bool unscaledTime, Execution execute, Action onStart = null, Action onEnd = null)
        {
            IEnumerator coroutine = CompactCoroutine(startAfter, duration, unscaledTime, execute, onStart, onEnd);

            if (player != null)
                player.StartCoroutine(coroutine);
            else
                CoroutinePlayer.StartCoroutine(coroutine);

            return coroutine;
        }
        public static IEnumerator StartCompactCoroutine(float startAfter, float duration, bool unscaledTime, Execution execute, Action onStart = null, Action onEnd = null)
        {
            IEnumerator coroutine = CompactCoroutine(startAfter, duration, unscaledTime, execute, onStart, onEnd);

            CoroutinePlayer.StartCoroutine(coroutine);

            return coroutine;
        }

        public static IEnumerator CompactCoroutine(float startAfter, float duration, bool unscaledTime, Execution execute, Action onStart = null, Action onEnd = null)
        {
            if(startAfter > 0.001f)
            { 
                if (unscaledTime)
                    yield return Yielders.GetWaitForSecondsRealtime(startAfter);
                else
                    yield return Yielders.GetWaitForSeconds(startAfter);
            }

            onStart?.Invoke();

            if (duration > 0.001f)
            {
                if(execute != null)
                {
                    float time = unscaledTime ? Time.unscaledTime : Time.time;
                    float startTime = time;

                    while(time < startTime + duration)
                    {
                        float t = (time - startTime) / duration;
                        execute(t);
                  
                        yield return null;
                        
                        time = unscaledTime ? Time.unscaledTime : Time.time;
                    }

                    execute(1);
                }
                else
                {
                    if(unscaledTime)
                        yield return Yielders.GetWaitForSecondsRealtime(duration);
                    else
                        yield return Yielders.GetWaitForSeconds(duration);
                }
            }
            else
                execute?.Invoke(1);

            onEnd?.Invoke();
        }
    }
}