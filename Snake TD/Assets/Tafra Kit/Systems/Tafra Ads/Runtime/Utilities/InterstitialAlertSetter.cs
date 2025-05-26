using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Ads
{
    public class InterstitialAlertSetter : MonoBehaviour
    {
        [SerializeField] private InterstitialAlert alert;
        [SerializeField] private bool assignOnEnable = true;
        [SerializeField] private bool removeOnDisable = true;

        private void OnEnable()
        {
            if (assignOnEnable)
                Assign();
        }
        private void OnDisable()
        {
            if (removeOnDisable)
                Clear();
        }

        public void Assign()
        {
            TafraAds.SetInterstitialAlertOverride(alert);
        }
        public void Clear()
        {
            TafraAds.ClearInterstitialAlertOverride();
        }
    }
}