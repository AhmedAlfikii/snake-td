#if TAFRA_TRANSLUCENT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeTai.Asset.TranslucentImage;
using ZUI;
using System;

[RequireComponent(typeof(TranslucentImage))]
public class BlurredImage : MonoBehaviour
{
    [SerializeField] private UIElement myUIE;

    private TranslucentImage blurImage;

    private void Awake()
    {
        blurImage = GetComponent<TranslucentImage>();
    }

    private void OnEnable()
    {
        var blurSource = TranslucentSourceGrabber.BlurSource;

        TranslucentSourceGrabber.OnSourceUpdated.AddListener(UpdateTranslucentSource);
        Debug.Log(blurSource);

        if(!blurSource)
            return;
        
        blurImage.SetSource(blurSource);

        if(myUIE == null)
            blurSource.UpdateBlur();
        else
            myUIE.OnShow.AddListener(OnUIEShow);
    }
    private void OnDisable()
    {
        TranslucentSourceGrabber.OnSourceUpdated.RemoveListener(UpdateTranslucentSource);

        if(myUIE != null)
            myUIE.OnShow.RemoveListener(OnUIEShow);
    }

    private void UpdateTranslucentSource()
    {
        blurImage.SetSource(TranslucentSourceGrabber.BlurSource);
    }
    private void OnUIEShow()
    {
        var source = TranslucentSourceGrabber.BlurSource;

        if(!source)
            return;

        source.UpdateBlur();
    }
}
#endif