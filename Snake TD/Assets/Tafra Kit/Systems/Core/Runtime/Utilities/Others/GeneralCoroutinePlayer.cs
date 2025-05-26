using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal
{
    public class GeneralCoroutinePlayer
    {
        private static GameObject go;
        private static MonoBehaviour player;
        private static bool isInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if(isInitialized)
                return;
            
            go = new GameObject("GeneralCoroutinePlayer", typeof(EmptyMonoBehaviour));
            
            GameObject.DontDestroyOnLoad(go);
            
            player = go.GetComponent<EmptyMonoBehaviour>();

            isInitialized = true;
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            if(!isInitialized)
                Initialize();

            return player.StartCoroutine(routine);
        }
        public static void StopCoroutine(IEnumerator routine)
        {
            if(!isInitialized)
                Initialize();

            player.StopCoroutine(routine);
        }
        public static void StopAllCoroutines()
        {
            if(!isInitialized)
                Initialize();

            player.StopAllCoroutines();
        }
    }
}