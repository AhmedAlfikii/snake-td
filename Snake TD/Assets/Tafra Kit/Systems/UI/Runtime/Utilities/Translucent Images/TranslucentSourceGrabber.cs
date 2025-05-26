#if TAFRA_TRANSLUCENT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using LeTai.Asset.TranslucentImage;

public static class TranslucentSourceGrabber
{
    private static TranslucentImageSource blurSource;

    public static TranslucentImageSource BlurSource
    {
        private set
        {
            blurSource = value;
        }
        get 
        {
            return blurSource;
        }
    }

    public static UnityEvent OnSourceUpdated = new UnityEvent();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        UpdateSource();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        UpdateSource();
    }

    private static void UpdateSource()
    {
        BlurSource = GameObject.FindAnyObjectByType<TranslucentImageSource>();
        OnSourceUpdated?.Invoke();
    }
}
#endif