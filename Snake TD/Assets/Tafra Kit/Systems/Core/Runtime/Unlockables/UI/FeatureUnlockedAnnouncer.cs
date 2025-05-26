using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Queueables;
using TafraKit.ZTweeners;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using ZUI;

namespace TafraKit.Internal
{
    public class FeatureUnlockedAnnouncer : MonoBehaviour
    {
        [Serializable]
        public class FeatureData : ITafraQueueable
        {
            public string name;
            public Lock featureLock;
            public int priority;
            [Tooltip("The icon that will be displayed in the announce screen.")]
            public Sprite icon;
            [Tooltip("Will be used during unlock animation")]
            public Sprite lockedIcon;
            [Tooltip("Will be used during unlock animation")]
            public Sprite lockedFlatWhiteIcon;
            [Tooltip("The images that will be disabled as long as the announce icon hasn't reached its destination yet.")]
            public Image[] targetImages;
            [Tooltip("The destination the announce icon will fly off to.")]
            public RectTransform targetRectTransform;

            [NonSerialized] public bool isAnnounced;
            [NonSerialized] public bool isQueuedToAnnounce;
            [NonSerialized] public string isAnnouncedSaveKey;

            private Action<FeatureData, bool> onUnlockedStateChange;
            private Action<FeatureData> onAnnounce;
            private Action onQueueableConcludedPerformance;

            public int Priority => priority;
            public Action OnQueueableConcludedPerformance { get => onQueueableConcludedPerformance; set => onQueueableConcludedPerformance = value; }

            private void OnUnlockedStateChange(bool isUnlocked)
            {
                onUnlockedStateChange?.Invoke(this, isUnlocked);
            }

            public void Initialize(Action<FeatureData, bool> onUnlockedStateChange, Action<FeatureData> onAnnounce)
            {
                this.onUnlockedStateChange = onUnlockedStateChange;
                this.onAnnounce = onAnnounce;

                isAnnouncedSaveKey = $"FEATURE_{featureLock.ID}_IS_ANNOUNCED";

                isAnnounced = TafraSaveSystem.LoadBool(isAnnouncedSaveKey);
            }
            public void Enable()
            {
                featureLock.OnUnlockedStateChange.AddListener(OnUnlockedStateChange);
            }
            public void Disable()
            {
                featureLock.OnUnlockedStateChange.RemoveListener(OnUnlockedStateChange);
            }
            public void SetAsAnnounced()
            {
                isAnnounced = true;
                TafraSaveSystem.SaveBool(isAnnouncedSaveKey, true);
            }
            public void AttemptAnnouncement()
            {
                if(isQueuedToAnnounce)
                    return;

                isQueuedToAnnounce = true;
                TafraQueues.Enqueue(this);
            }
            public void QueueablePerform()
            {
                isQueuedToAnnounce = false;
                onAnnounce?.Invoke(this);
            }
        }

        [Header("Features")]
        [SerializeField] private List<FeatureData> features;

        [Header("References")]
        [SerializeField] private UIElementsGroup uieg;
        [SerializeField] private PlayableDirector timelinePlayer;
        [SerializeField] private Image icon;
        [SerializeField] private Image lockedIcon;
        [SerializeField] private Image lockedWhiteIcon;
        [SerializeField] private Image lockImage;
        [SerializeField] private TextMeshProUGUI featureName;
        [SerializeField] private Image mimicIcon;

        [Header("Animation")]
        [SerializeField] private float iconFlyDuration = 0.75f;
        [SerializeField] private EasingType iconFlyEasing;

        private RectTransform mimicIconRT;
        private ZTweenRect rectTween = new ZTweenRect();

        private void Awake()
        {
            for(int i = 0; i < features.Count; i++)
            {
                var feature = features[i];

                feature.Initialize(OnUnlockedStateChange, OnAnnounce);
            }

            mimicIconRT = mimicIcon.transform as RectTransform;
        }

        private void OnEnable()
        {
            for(int i = 0; i < features.Count; i++)
            {
                var feature = features[i];

                feature.Enable();
                OnUnlockedStateChange(feature, feature.featureLock.IsUnlocked());
            }
        }
        private void OnDisable()
        {
            for(int i = 0; i < features.Count; i++)
            {
                var feature = features[i];

                feature.Disable();
            }
        }

        private void OnUnlockedStateChange(FeatureData featureData, bool isUnlocked)
        {
            if(featureData.isAnnounced)
                return;

            if(!isUnlocked)
                return;

            featureData.AttemptAnnouncement();

            for(int i = 0; i < featureData.targetImages.Length; i++)
            {
                featureData.targetImages[i].enabled = false;
            }
        }

        private void OnAnnounce(FeatureData featureData)
        {
            featureData.SetAsAnnounced();

            StartCoroutine(AnnounceFeature(featureData));
        }

        private IEnumerator AnnounceFeature(FeatureData featureData)
        {
            icon.sprite = featureData.icon;
            mimicIcon.sprite = featureData.icon;
            lockedIcon.sprite = featureData.lockedIcon;
            lockedWhiteIcon.sprite = featureData.lockedFlatWhiteIcon;
            featureName.text = featureData.name;

            for(int i = 0; i < featureData.targetImages.Length; i++)
            {
                featureData.targetImages[i].enabled = false;
            }

            mimicIcon.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(true);
            lockedIcon.gameObject.SetActive(true);
            lockImage.color = new Color(lockImage.color.r, lockImage.color.g, lockImage.color.b, 1);

            uieg.ChangeVisibility(true);

            yield return Yielders.GetWaitForSecondsRealtime(1.5f);

            timelinePlayer.Play();

            yield return Yielders.GetWaitForSecondsRealtime((float)timelinePlayer.duration);

            uieg.ChangeVisibility(false);

            mimicIconRT.AdaptRect(icon.transform as RectTransform);
            mimicIconRT.position = icon.transform.position;
            mimicIcon.gameObject.SetActive(true);

            yield return Yielders.GetWaitForSecondsRealtime(uieg.GetAllHidingTime());

            bool completed = false;

            mimicIconRT.ZTweenAdaptRect(rectTween, featureData.targetRectTransform, iconFlyDuration).SetEasingType(iconFlyEasing).SetOnCompleted(() => { completed = true; });

            while(!completed)
            {
                yield return null;
            }

            mimicIconRT.gameObject.SetActive(false);

            for(int i = 0; i < featureData.targetImages.Length; i++)
            {
                featureData.targetImages[i].enabled = true;
            }

            featureData.OnQueueableConcludedPerformance?.Invoke();
        }
    }
}