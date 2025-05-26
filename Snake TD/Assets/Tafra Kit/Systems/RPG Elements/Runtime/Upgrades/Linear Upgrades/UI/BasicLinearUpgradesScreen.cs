using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Consumables;
using TafraKit.MotionFactory;
using TafraKit.RPG;
using TafraKit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZUI;

namespace TafraKit
{
    public class BasicLinearUpgradesScreen : MonoBehaviour, IUIAlertContainer
    {
        [SerializeField] private DynamicPool<UpgradeSlot> mainSlotsPool = new DynamicPool<UpgradeSlot>();

        [Header("UI")]
        [SerializeField] private UIElementsGroup myUIEG;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform activeBG;
        [SerializeField] private RectTransform inactiveBG;
        [SerializeField] private RectTransform separator;
        [SerializeField] private RectTransform mainUpgradesBarBG;
        [SerializeField] private RectTransform mainUpgradesBar;

        [Header("Main Purchase Bubble")]
        [SerializeField] private VisibilityMotionController mainPurchaseBubbleMotionController;
        [SerializeField] private RectTransform mainPurchaseBubble;
        [SerializeField] private TextMeshProUGUI mainPurchaseBubbleTitle;
        [SerializeField] private TextMeshProUGUI mainPurchaseBubbleDescrption;
        [SerializeField] private TextMeshProUGUI mainPurchaseBubblePrice;
        [SerializeField] private ZButton mainPurchaseBubbleButton;

        [Header("Layout")]
        [SerializeField] private Vector3 mainSlotStartPosition = new Vector3(270, 480, 0);
        [SerializeField] private float stepLength = 350;

        [Header("Animation")]
        [SerializeField] private float mainUpgradeAnimationDuration = 1f;
        [SerializeField] private EasingType mainUpgradeAnimationEasing = new EasingType(MotionType.EaseOut, new EasingEquationsParameters());

        [Header("SFX")]
        [SerializeField] private SFXClips upgradeAudio;

        private MainLinearUpgradePath mainUpgradePath;
        private List<UpgradeSlot> spawnedMainUpgradeSlots = new List<UpgradeSlot>();
        private int targetMainUpgradeIndex;
        private float targetMainUpgradeCost;
        private UpgradeModule targetMainUpgrade;
        private UpgradeSlot targetMainUpgradeSlot;
        private int mainUpgradesCount;
        private float contentPadding;
        private float totalLength;
        private bool mainPurchaseBubbleShown;
        private IEnumerator goingToNextMainUpgradeEnum;
        private bool isAutoScrolling;
        private Consumable mainUpgradesCostCurrency;
        private UIAlertState currentAlertState;
        private UnityEvent<UIAlertState> onAlertStateChange = new UnityEvent<UIAlertState>();
        protected InfluenceReceiver<UIAlertState> activeAlerts;

        public UIAlertState CurrentAlertState => currentAlertState;
        public UnityEvent<UIAlertState> OnAlertStateChange => onAlertStateChange;

        private void Awake()
        {
            Initialize();
        }
        private void OnEnable()
        {
            mainPurchaseBubbleButton.onClick.AddListener(OnMainPurchaseButtonClicked);

            myUIEG.OnShow.AddListener(OnShow);
        }
        private void OnDisable()
        {
            mainPurchaseBubbleButton.onClick.RemoveListener(OnMainPurchaseButtonClicked);
         
            myUIEG.OnShow.RemoveListener(OnShow);
        }
        private void Update()
        {
            //if(mainPurchaseBubbleShown && Input.GetMouseButtonUp(0))
            //{
            //    mainPurchaseBubbleMotionController.Hide();
            //    mainPurchaseBubbleShown = false;
            //}

            if(isAutoScrolling && Input.GetMouseButton(0))
            {
                isAutoScrolling = false;
            }
        }

        private void OnShow()
        {
            if (targetMainUpgradeSlot != null)
                scrollRect.GoToElement(targetMainUpgradeSlot.transform);

            if(mainPurchaseBubbleShown)
            {
                mainPurchaseBubbleMotionController.Hide(true);
                mainPurchaseBubbleShown = false;
            }
        }
        private void OnMainPurchaseButtonClicked()
        {
            if(targetMainUpgrade == null)
                return;

            if(ConsumableMerchant.Consume(mainUpgradePath.CostCurrency, mainUpgradePath.GetUpgradeCost(targetMainUpgradeIndex), null, false))
            {
                mainUpgradePath.ApplyUpgrade(targetMainUpgradeIndex);

                targetMainUpgradeSlot.ChangeUnlockdState(true, false);
                targetMainUpgradeSlot.ChangeTargetSlotState(false);
                targetMainUpgradeSlot.ChangeAlertState(false);

                SFXPlayer.Play(upgradeAudio);

                GoToNextMainUpgrade();
            }
            else
            {
                Consumable costCurrency = mainUpgradePath.CostCurrency;
                FleetingMessages.Show($"You don't have enough <sprite=\"{costCurrency.ID}\", index=0>{costCurrency.GetDisplayName(1)}");
            }
        }
        private bool ShouldReplaceAlertState(UIAlertState newInfluence, UIAlertState oldInfluence)
        {
            return true;
        }
        private void OnActiveAlertUpdated(UIAlertState state)
        {
            if(state == currentAlertState)
                return;

            currentAlertState = state;
            onAlertStateChange?.Invoke(currentAlertState);
        }
        private void OnAllAlertsCleared()
        {
            if(currentAlertState == UIAlertState.None)
                return;

            currentAlertState = UIAlertState.None;
            onAlertStateChange?.Invoke(currentAlertState);
        }

        private void Initialize()
        {
            activeAlerts = new InfluenceReceiver<UIAlertState>(ShouldReplaceAlertState, OnActiveAlertUpdated, null, OnAllAlertsCleared);

            mainSlotsPool.Initialize();

            mainUpgradePath = LinearUpgrades.MainUpgradePath;
            mainUpgradesCostCurrency = mainUpgradePath.CostCurrency;

            mainUpgradesCostCurrency.OnValueChange.AddListener(OnMainUpgradesCurrencyValueChange);

            int unlockedMainUpgrades = mainUpgradePath.AppliedUpgradesCount;
           
            targetMainUpgradeIndex = unlockedMainUpgrades;

            var mainUpgrades = mainUpgradePath.Upgrades;
            mainUpgradesCount = mainUpgrades.Count;
            for (int i = 0; i < mainUpgradesCount; i++)
            {
                var slot = mainSlotsPool.RequestUnit(activateUnit:false);

                bool unlocked = i < unlockedMainUpgrades;
                bool isTarget = i == targetMainUpgradeIndex;

                slot.Populate(mainUpgrades[i], unlocked, isTarget, () => { OnMainSlotClicked(slot); });
                slot.RT.anchoredPosition = mainSlotStartPosition + new Vector3(0, i * stepLength, 0);
                slot.gameObject.SetActive(true);
                spawnedMainUpgradeSlots.Add(slot);
            }

            contentPadding = mainSlotStartPosition.y * 2; //Multiplied by 2 since we want to add padding to the top and bottom.
            totalLength = contentPadding + ((mainUpgradesCount - 1) * stepLength);

            float activeLength = mainSlotStartPosition.y + (unlockedMainUpgrades - 1) * stepLength;
            float inactiveLength = totalLength - activeLength;
            float latestUnlockedUpgradeY = mainSlotStartPosition.y + ((unlockedMainUpgrades - 1) * stepLength);
        
            content.sizeDelta = new Vector2(content.sizeDelta.x, totalLength);
            activeBG.sizeDelta = new Vector2(activeBG.sizeDelta.x, activeLength);
            inactiveBG.sizeDelta = new Vector2(inactiveBG.sizeDelta.x, inactiveLength);

            inactiveBG.anchoredPosition = new Vector3(inactiveBG.anchoredPosition.x, latestUnlockedUpgradeY, 0);
            separator.anchoredPosition = new Vector3(separator.anchoredPosition.x, latestUnlockedUpgradeY, 0);

            mainUpgradesBarBG.anchoredPosition = mainSlotStartPosition;
            mainUpgradesBarBG.sizeDelta = new Vector2(mainUpgradesBarBG.sizeDelta.x, (mainUpgradesCount - 1) * stepLength);

            mainUpgradesBar.anchoredPosition = mainSlotStartPosition;
            mainUpgradesBar.sizeDelta = new Vector2(mainUpgradesBar.sizeDelta.x, Mathf.Max(unlockedMainUpgrades - 1, 0) * stepLength);

            if(mainUpgradesCount > targetMainUpgradeIndex)
            {
                targetMainUpgradeSlot = spawnedMainUpgradeSlots[targetMainUpgradeIndex];
                targetMainUpgrade = targetMainUpgradeSlot.UpgradeModule;
                targetMainUpgradeCost = mainUpgradePath.GetUpgradeCost(targetMainUpgradeIndex);

            }
            else
            {
                targetMainUpgrade = null;
                targetMainUpgradeSlot = null;
            }

            mainPurchaseBubble.SetAsLastSibling();

            PlaceMainBubble();

            CheckTargetUpgradePurchasableState();
        }

        private void OnMainUpgradesCurrencyValueChange(float arg0)
        {
            CheckTargetUpgradePurchasableState();
        }

        private void PlaceMainBubble()
        {
            if(targetMainUpgrade == null)
            {
                mainPurchaseBubble.gameObject.SetActive(false);
                return;
            }

            mainPurchaseBubbleTitle.text = targetMainUpgrade.DisplayName;
            mainPurchaseBubbleDescrption.text = targetMainUpgrade.Description;
            mainPurchaseBubblePrice.text = $"<sprite=\"{mainUpgradePath.CostCurrency.ID}\", index=0> {mainUpgradePath.GetUpgradeCost(targetMainUpgradeIndex)}";

            mainPurchaseBubble.anchoredPosition = targetMainUpgradeSlot.RT.anchoredPosition + new Vector2(0, targetMainUpgradeSlot.RT.rect.height / 2f);

            mainPurchaseBubble.gameObject.SetActive(true);
        }
        private void GoToNextMainUpgrade()
        {
            targetMainUpgradeIndex++;

            if(mainUpgradesCount > targetMainUpgradeIndex)
            {
                targetMainUpgradeSlot = spawnedMainUpgradeSlots[targetMainUpgradeIndex];
                targetMainUpgrade = targetMainUpgradeSlot.UpgradeModule;
                targetMainUpgradeCost = mainUpgradePath.GetUpgradeCost(targetMainUpgradeIndex);
            }
            else
            {
                targetMainUpgrade = null;
                targetMainUpgradeSlot = null;
            }

            if(goingToNextMainUpgradeEnum != null)
                StopCoroutine(goingToNextMainUpgradeEnum);

            goingToNextMainUpgradeEnum = GoingToNextMainUpgrade();

            CheckTargetUpgradePurchasableState();

            StartCoroutine(goingToNextMainUpgradeEnum);
        }
        private void OnMainSlotClicked(UpgradeSlot slot)
        {
            //If this is the slot that the player can purchase, then show the purchase bubble.
            if(spawnedMainUpgradeSlots.Count > targetMainUpgradeIndex && slot == spawnedMainUpgradeSlots[targetMainUpgradeIndex])
            {
                if(mainPurchaseBubbleShown)
                {
                    mainPurchaseBubbleMotionController.Hide();
                    mainPurchaseBubbleShown = false;
                }
                else
                {
                    mainPurchaseBubbleMotionController.Show();
                    mainPurchaseBubbleShown = true;
                }

                return;
            }

            if(mainPurchaseBubbleShown)
            {
                mainPurchaseBubbleMotionController.Hide();
                mainPurchaseBubbleShown = false;
            }

            InfoBubbleHandler.Show(slot.transform as RectTransform, Side.Top, slot.UpgradeModule.Description, slot.UpgradeModule.DisplayName);
        }
        private void CheckTargetUpgradePurchasableState()
        {
            if(targetMainUpgrade == null && currentAlertState != UIAlertState.None)
            {
                currentAlertState = UIAlertState.None;
                onAlertStateChange?.Invoke(currentAlertState);
            }

            if(ConsumableMerchant.IsAffordable(mainUpgradesCostCurrency, targetMainUpgradeCost))
            {
                currentAlertState = UIAlertState.Upgrade;
                onAlertStateChange?.Invoke(currentAlertState);

                targetMainUpgradeSlot.ChangeAlertState(true);
            }
            else if(currentAlertState != UIAlertState.None)
            {
                currentAlertState = UIAlertState.None;
                onAlertStateChange?.Invoke(currentAlertState);

                targetMainUpgradeSlot.ChangeAlertState(false);
            }
        }

        private IEnumerator GoingToNextMainUpgrade()
        {
            mainPurchaseBubbleMotionController.Hide();

            int unlockedMainUpgrades = mainUpgradePath.AppliedUpgradesCount;

            float activeLength = mainSlotStartPosition.y + (unlockedMainUpgrades - 1) * stepLength;
            float inactiveLength = totalLength - activeLength;
            float latestUnlockedUpgradeY = mainSlotStartPosition.y + ((unlockedMainUpgrades - 1) * stepLength);

            float activeBGStartLength = activeBG.sizeDelta.y;
            float inactiveBGStartLength = inactiveBG.sizeDelta.y;
            float inactiveBGStartY = inactiveBG.anchoredPosition.y;
            float separatorStartY = separator.anchoredPosition.y;
            float mainUpgradesBarStartLength = mainUpgradesBar.sizeDelta.y;
            float mainUpgradesBarTargetLength = Mathf.Max(unlockedMainUpgrades - 1, 0) * stepLength;

            float startTime = Time.time;
            float endTime = startTime + mainUpgradeAnimationDuration;

            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / mainUpgradeAnimationDuration;
                
                t = MotionEquations.GetEaseFloat(t, mainUpgradeAnimationEasing);

                activeBG.sizeDelta = new Vector2(activeBG.sizeDelta.x, Mathf.LerpUnclamped(activeBGStartLength, activeLength, t));
                inactiveBG.sizeDelta = new Vector2(inactiveBG.sizeDelta.x, Mathf.LerpUnclamped(inactiveBGStartLength, inactiveLength, t));

                inactiveBG.anchoredPosition = new Vector3(inactiveBG.anchoredPosition.x, Mathf.LerpUnclamped(inactiveBGStartY, latestUnlockedUpgradeY, t), 0);
                separator.anchoredPosition = new Vector3(separator.anchoredPosition.x, Mathf.LerpUnclamped(separatorStartY, latestUnlockedUpgradeY, t), 0);
                
                mainUpgradesBar.sizeDelta = new Vector2(mainUpgradesBar.sizeDelta.x, Mathf.LerpUnclamped(mainUpgradesBarStartLength, mainUpgradesBarTargetLength, t));

                yield return null;
            }

            activeBG.sizeDelta = new Vector2(activeBG.sizeDelta.x, activeLength);
            inactiveBG.sizeDelta = new Vector2(inactiveBG.sizeDelta.x, inactiveLength);

            inactiveBG.anchoredPosition = new Vector3(inactiveBG.anchoredPosition.x, latestUnlockedUpgradeY, 0);
            separator.anchoredPosition = new Vector3(separator.anchoredPosition.x, latestUnlockedUpgradeY, 0);

            mainUpgradesBar.sizeDelta = new Vector2(mainUpgradesBar.sizeDelta.x, mainUpgradesBarTargetLength);


            goingToNextMainUpgradeEnum = null;

            PlaceMainBubble();

            if(targetMainUpgrade != null)
            {
                mainPurchaseBubbleMotionController.Show();
                targetMainUpgradeSlot.ChangeTargetSlotState(true);

                Vector3 contentStartPosition = content.position;
                Vector3 contentTargetPosition = scrollRect.GetContentPositionToCenterElement(targetMainUpgradeSlot.transform);

                startTime = Time.time;
                float scrollDuration = 0.5f;
                endTime = startTime + scrollDuration;

                isAutoScrolling = true;

                while(Time.time < endTime)
                {
                    float t = (Time.time - startTime) / scrollDuration;
                    t = MotionEquations.EaseOut(t);

                    if(!isAutoScrolling)
                        break;

                    content.position = Vector3.LerpUnclamped(contentStartPosition, contentTargetPosition, t);

                    yield return null;
                }

                if (isAutoScrolling)
                    content.position = contentTargetPosition;
            }
        }
    }
}