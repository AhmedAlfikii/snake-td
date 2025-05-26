using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Ads
{
    public class BannerAdScene : MonoBehaviour
    {
        [SerializeField] private string sourceName = "level";
        private void OnEnable()
        {
            //To make sure banners got a chance to initialize.
            StartCoroutine(ShowAfterDelay());
        }
        private void OnDisable()
        {
            TafraAds.HideBannerAd();
        }

        private IEnumerator ShowAfterDelay()
        {
            yield return null;
            TafraAds.ShowBannerAd(sourceName);
        }
    }
}