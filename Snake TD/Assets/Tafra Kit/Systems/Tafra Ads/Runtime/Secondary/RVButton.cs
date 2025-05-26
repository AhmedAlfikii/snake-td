using System;
using TafraKit.MotionFactory;
using TafraKit.UI;
using UnityEngine;

namespace TafraKit.Ads
{
    public class RVButton : MonoBehaviour
    {
        [SerializeField] private VisibilityMotionController loadingIndicator;

        private ZButton myButton;
        private bool isRVAvailable;
        private string source;
        private Action onReward;

        public bool Interactable 
        { 
            get { return isRVAvailable; }
            set { myButton.interactable =  value; }
        }

        private void Awake()
        {
            myButton = GetComponent<ZButton>();
        }
        private void OnEnable()
        {
            UpdateAvailability();

            myButton.onClick.AddListener(OnButtonClick);

            TafraAds.OnRewardedVideoLoaded.AddListener(OnRewardedVideoLoaded);
            TafraAds.OnRewardedVideoEnd.AddListener(OnRewardedVideoEnd);
        }
        private void OnDisable()
        {
            myButton.onClick.RemoveListener(OnButtonClick);
        
            TafraAds.OnRewardedVideoLoaded.RemoveListener(OnRewardedVideoLoaded);
            TafraAds.OnRewardedVideoEnd.RemoveListener(OnRewardedVideoEnd);
        }

        private void OnRewardedVideoLoaded(TafraAdInfo adInfo)
        {
            UpdateAvailability();
        }
        private void OnRewardedVideoEnd(TafraAdInfo adInfo)
        {
            UpdateAvailability();
        }
        private void OnFail()
        {
            FleetingMessages.Show("Ad failed, please try again.");
        }
        private void OnReward()
        {
            onReward?.Invoke();
        }
        private void OnButtonClick()
        {
            if(isRVAvailable)
                TafraAds.ShowRewardedAd(source, null, OnReward, null, OnFail);
            else
                FleetingMessages.Show("Ad isn't ready yet, please try again later.");
        }

        private void UpdateAvailability()
        {
            isRVAvailable = TafraAds.IsRewardedVideoLoaded;

            if(isRVAvailable)
            {
                if(loadingIndicator.State != VisibilityControllerState.Hidden || loadingIndicator.State != VisibilityControllerState.Hiding)
                {
                    loadingIndicator.Hide();
                }
            }
            else
            {
                if(!loadingIndicator.IsVisible)
                {
                    loadingIndicator.Show();
                }
            }
        }

        public void Initialize(string source, Action onReward)
        { 
            this.source = source;
            this.onReward = onReward;
        }
    }
}