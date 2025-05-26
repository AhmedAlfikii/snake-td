using System.Collections.Generic;
using TafraKit.RPG;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZUI;

namespace TafraKit.Consumables
{
    public class RewardsScreen : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private DynamicPool<ItemSlot> slotsPool = new DynamicPool<ItemSlot>();
        [SerializeField] private DynamicPool<ConsumableUIBar> barsPool = new DynamicPool<ConsumableUIBar>();
        [SerializeField] private Transform barsHolder;
        [SerializeField] private UIElementsGroup screenUIEG;
        [SerializeField] private Button claimBtn;
        [SerializeField] private UIElement claimBtnUIE;
        #endregion

        #region Events
        private readonly UnityEvent onShow = new UnityEvent();
        private readonly UnityEvent onHide = new UnityEvent();
        #endregion

        #region Private Fields
        private ItemRewarderSettings settings;
        private readonly List<ItemSlot> spawnedSlots = new List<ItemSlot>();
        private Dictionary<Consumable, ConsumableUIBar> barByConsumableDic = new Dictionary<Consumable, ConsumableUIBar>();
        private Dictionary<ConsumableUIBar, UIElement> barUIEByBarDic = new Dictionary<ConsumableUIBar, UIElement>();
        private List<ConsumableChange> rewards = new List<ConsumableChange>();
        #endregion

        #region Public Properties
        public bool IsVisible => screenUIEG.Visible;
        public UnityEvent OnShow => onShow;
        public UnityEvent OnHide => onHide;
        #endregion

        #region Monobehaviour Messages
        private void Awake()
        {
            settings = TafraSettings.GetSettings<ItemRewarderSettings>();

            slotsPool.Initialize();
            barsPool.Initialize();
        }
        #endregion

        #region Public Functions
        public void Populate(List<ConsumableChange> addedRewards,List<StorableScriptableObject> storables)
        {
            slotsPool.ReleaseUnits(spawnedSlots);
            
            rewards.Clear();
            
            rewards.AddRange(addedRewards);
            
            spawnedSlots.Clear();

            for(int i = 0; i < rewards.Count; i++)
            {
                ItemSlot slot = slotsPool.RequestUnit();

                slot.transform.SetAsLastSibling();

                ConsumableChange reward = rewards[i];

                slot.Populate(reward.consumable, Mathf.RoundToInt(reward.changeAmount));

                spawnedSlots.Add(slot);

                //if(settings.PlayConsumablesAnimation)
                //    RequestConsumableBar(reward.consumable);
            }

            for(int i = 0; i < storables.Count; i++)
            {
                ItemSlot slot = slotsPool.RequestUnit();
                
                slot.transform.SetAsLastSibling();

                StorableScriptableObject reward = storables[i];
            
                slot.Populate(reward);
                
                spawnedSlots.Add(slot);
            } 
        }
        public void Show()
        {
            screenUIEG.ChangeVisibility(true);
            
            screenUIEG.OnShowComplete.AddListener(OnScreenShowComplete);
       
            claimBtn.onClick.AddListener(ClaimRewards);

            onShow?.Invoke();
        }
        private void OnScreenShowComplete()
        {
            screenUIEG.OnShowComplete.RemoveListener(OnScreenShowComplete);
            
            claimBtnUIE.ChangeVisibility(true);
        }

        public void ClaimRewards()
        {
            claimBtn.onClick.RemoveListener(ClaimRewards);
            
            claimBtnUIE.ChangeVisibility(false);

            if (settings.PlayConsumablesAnimation)
            {
                for (int i = 0; i < rewards.Count; i++)
                {
                    ConsumableChange reward = rewards[i];
                
                    ShowConsumableBar(reward.consumable);

                    //TODO
                    Debug.LogError("Handle this!");
                    //ConsumableAnimationHandler.Play(reward.consumable,barByConsumableDic[reward.consumable],(int) reward.changeAmount,null,
                    //    () =>
                    //    {
                    //        ReleaseConsumableBar(reward.consumable,barByConsumableDic[reward.consumable].DisplayedValue + (int) reward.changeAmount);
                    //    });
                }
            }
            
            Hide();
        }
        public void Hide()
        {
            screenUIEG.ChangeVisibility(false);
            
            onHide?.Invoke();
        }
        public void RequestConsumableBar(Consumable consumable)
        {
            if (barByConsumableDic.ContainsKey(consumable))
                return;
            
            ConsumableUIBar bar = ConsumablesBarFetcher.Fetch(consumable,nameof(RewardsScreen));

            if (bar == null)
            { 
                bar = barsPool.RequestUnit(barsHolder,false);
                
                bar.transform.localPosition = Vector3.zero;
            }
            
            bar.Populate(consumable);

            if (!barUIEByBarDic.ContainsKey(bar) && bar.TryGetComponent(out UIElement barUIE))
                barUIEByBarDic.Add(bar,barUIE);

            bar.AutoUpdate = false;

            barByConsumableDic.TryAdd(consumable,bar);
        }
        public void ShowConsumableBar(Consumable consumable)
        {
            if (!barByConsumableDic.ContainsKey(consumable))
                RequestConsumableBar(consumable);
            
            ConsumableUIBar bar = barByConsumableDic[consumable];
            
            if(barUIEByBarDic.TryGetValue(bar,out UIElement barUIE) && !barUIE.Visible)
                barUIE.ChangeVisibility(true);
        }
        public void ReleaseConsumableBar(Consumable consumable,int finalValue)
        {
            ConsumablesBarFetcher.Abandon(consumable, nameof(RewardsScreen));
                
            ConsumableUIBar bar = barByConsumableDic[consumable];
            
            bar.AutoUpdate = true;
            //bar.DisplayedValue = finalValue;

            if (barUIEByBarDic.TryGetValue(bar, out UIElement barUIE))
            {
                barUIE.OnHideComplete.AddListener(OnUieHide);
            
                barUIE.ChangeVisibility(false); 
                
                barUIEByBarDic.Remove(bar);
            }

            barByConsumableDic.Remove(consumable);

            void OnUieHide()
            {
                barUIE.OnHideComplete.RemoveListener(OnUieHide);
                
                barsPool.ReleaseUnit(bar);
            }
        }
        #endregion
    }
}
