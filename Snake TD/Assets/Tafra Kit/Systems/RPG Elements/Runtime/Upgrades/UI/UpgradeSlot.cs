using System;
using TafraKit.Consumables;
using TafraKit.MotionFactory;
using TafraKit.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TafraKit.RPG
{
    public class UpgradeSlot : MonoBehaviour
    {
        [SerializeField] private Image icon;

        [Header("Skin")]
        [SerializeField] private Image[] bgs;
        [SerializeField] private Color bgsUnlockedColor;
        [SerializeField] private Color bgsLockedColor;
        [SerializeField] private GameObject highlight;
        [SerializeField] private VisibilityMotionController alertMotionController;

        [Space()]

        [SerializeField] private Color iconUnlockedColor;
        [SerializeField] private Color iconLockedColor;

        private RectTransform rt;
        private ZButton myButton;
        private UpgradeModule upgradeModule;
        private bool isTarget;
        private Consumable currency;
        private float cost;
        private Action onClick;

        public UpgradeModule UpgradeModule => upgradeModule;
        public RectTransform RT
        {
            get
            {
                if(rt == null)
                    rt = transform as RectTransform;

                return rt;
            }
        }

        private void Awake()
        {
            myButton = GetComponent<ZButton>();
        }
        private void OnEnable()
        {
            myButton.onClick.AddListener(OnClick);
        }
        private void OnDisable()
        {
            myButton.onClick.RemoveListener(OnClick);
        }
        private void OnDestroy()
        {
            if(upgradeModule != null)
            {
                upgradeModule.ReleaseIcon();
                upgradeModule = null;
            }
        }

        private void OnClick()
        {
            onClick?.Invoke();
        }

        public void Populate(UpgradeModule upgradeModule, bool isUnlocked, bool isTarget, Action onClick)
        {
            this.onClick = onClick;

            this.currency = currency;
            this.cost = cost;

            if(this.upgradeModule != null)
                this.upgradeModule.ReleaseIcon();

            this.upgradeModule = upgradeModule;

            icon.sprite = upgradeModule.LoadIcon();
            icon.color = isUnlocked ? iconUnlockedColor : iconLockedColor;

            for(int i = 0; i < bgs.Length; i++)
            {
                var bg = bgs[i];
                bg.color = isUnlocked ? bgsUnlockedColor : bgsLockedColor;
            }

            highlight.gameObject.SetActive(isTarget);

            this.isTarget = isTarget;
        }
        public void ChangeUnlockdState(bool isUnlocked, bool instant)
        {
            icon.color = isUnlocked ? iconUnlockedColor : iconLockedColor;
            for(int i = 0; i < bgs.Length; i++)
            {
                var bg = bgs[i];
                bg.color = isUnlocked ? bgsUnlockedColor : bgsLockedColor;
            }

            if(instant)
            {

            }
            else
            {

            }
        }
        public void ChangeTargetSlotState(bool isTarget)
        {
            if(this.isTarget == isTarget)
                return;

            highlight.gameObject.SetActive(isTarget);

            this.isTarget = isTarget;
        }
        public void ChangeAlertState(bool show)
        {
            if(show)
                alertMotionController.Show();
            else
                alertMotionController.Hide();
        }
    }
}