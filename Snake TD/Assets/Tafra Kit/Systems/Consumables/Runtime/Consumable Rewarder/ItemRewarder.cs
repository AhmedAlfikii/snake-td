using System;
using System.Collections.Generic;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit.Consumables
{
    public static class ItemRewarder
    {
        #region Private Fields
        private static ItemRewarderSettings settings;
        private static bool isEnabled;
        private static RewardsScreen screen;
        private static readonly List<ConsumableChange> addedConsumableRewards = new List<ConsumableChange>();
        private static readonly List<StorableScriptableObject> addedStorableRewards = new List<StorableScriptableObject>();
        private static  List<StorableScriptableObject> tempAddedStorables = new List<StorableScriptableObject>();
        private static Action endAction;
        private static Inventory playerInventory;
        private static Dictionary<string, List<StorableScriptableObject>> expandableStorableByStorable;
        private static Dictionary<Consumable, Consumable> consumableByIdleIncomeToken;
        #endregion

        #region Public Properties
        public static bool IsScreenVisible => screen.IsVisible;
        public static List<ConsumableChange> AddedConsumableRewards => addedConsumableRewards;
        public static List<StorableScriptableObject> AddedStorablesRewards => addedStorableRewards;
        #endregion

        #region Private Functions
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<ItemRewarderSettings>();
            
            if(!settings.Enabled)
                return;

            if(settings.ScreenPrefab == null)
            {
                TafraDebugger.Log("Consumable Rewarder", "There is no screen prefab assigned.", TafraDebugger.LogType.Error);
                return;
            }

            GameObject screenGO = GameObject.Instantiate(settings.ScreenPrefab.gameObject);

            MonoBehaviour.DontDestroyOnLoad(screenGO);

            screen = screenGO.GetComponent<RewardsScreen>();

            if(settings.ExpandableStorables.Count > 0)
            {
                expandableStorableByStorable = new Dictionary<string, List<StorableScriptableObject>>();

                for(int i = 0; i < settings.ExpandableStorables.Count; i++)
                {
                    var expandableStorable = settings.ExpandableStorables[i];
                    expandableStorableByStorable.Add(expandableStorable.storable.OriginalID, expandableStorable.expantionList);
                }
            }

            if(settings.IdleIncomeConsumables.Count > 0)
            {
                consumableByIdleIncomeToken = new Dictionary<Consumable, Consumable>();

                for(int i = 0; i < settings.IdleIncomeConsumables.Count; i++)
                {
                    var idleIncomeConsumable = settings.IdleIncomeConsumables[i];
                    consumableByIdleIncomeToken.Add(idleIncomeConsumable.idleIncomeToken, idleIncomeConsumable.mainConsumable);
                }
            }
            
            isEnabled = true;
        }
        #endregion
    
        #region Public Functions
        public static void AddConsumables(List<ConsumableChange> rewards, bool dontReward = false)
        {
            if (!isEnabled)
            {
                TafraDebugger.Log("Consumable Rewarder", "Consumable rewarder is not initialized properly.", TafraDebugger.LogType.Error);
                return;
            }
            
            for (int i = 0; i < rewards.Count; i++)
            {
                var reward = rewards[i];

                if(reward.changeAmount > 0)
                {
                    bool createNewSlot = true;

                    for (int j = 0; j < addedConsumableRewards.Count; j++)
                    {
                        if (reward.consumable.ID == addedConsumableRewards[j].consumable.ID)
                        {
                            ConsumableChange combinedChange = new ConsumableChange(reward.consumable, addedConsumableRewards[j].changeAmount + reward.changeAmount);

                            addedConsumableRewards[j] = combinedChange;

                            createNewSlot = false;

                            break;
                        }
                    }

                    if (createNewSlot)
                        addedConsumableRewards.Add(reward);

                    if (settings.PlayConsumablesAnimation)
                        screen.RequestConsumableBar(reward.consumable);

                    if (!dontReward)
                        reward.AddChange();
                }
            }
        }
        public static void AddConsumable(ConsumableChange reward, bool dontReward = false)
        {
            if(!isEnabled)
            {
                TafraDebugger.Log("Consumable Rewarder", "Consumable rewarder is not initialized properly.", TafraDebugger.LogType.Error);
                return;
            }

            bool createNewSlot = true;

            for (int j = 0; j < addedConsumableRewards.Count; j++)
            {
                if (reward.consumable.ID == addedConsumableRewards[j].consumable.ID)
                {
                    addedConsumableRewards[j].changeAmount += reward.changeAmount;

                    createNewSlot = false;

                    break;
                }
            }

            if (createNewSlot)
                addedConsumableRewards.Add(reward);

            if (settings.PlayConsumablesAnimation)
                screen.RequestConsumableBar(reward.consumable);

            if (!dontReward)
                reward.AddChange();
        }
        public static void AddStorables(List<StorableScriptableObject> storables, bool dontReward = false)
        {
            if (!isEnabled)
            {
                TafraDebugger.Log("Consumable Rewarder", "Consumable rewarder is not initialized properly.", TafraDebugger.LogType.Error);
                return;
            }

            if(playerInventory == null)
                playerInventory = SceneReferences.PlayerHealthy.GetComponent<Inventory>();

            tempAddedStorables.Clear();

            for(int i = 0; i < storables.Count; i++)
            {
                StorableScriptableObject storable = storables[i];

                if(!storable.IsInstance)
                {
                    Debug.LogError($"Storable with the ID ({storable.OriginalID}) is not an instance.");

                    break;
                }

                if(storable.Quantity > 0)
                {
                    if(expandableStorableByStorable.TryGetValue(storable.OriginalID, out var expansionList))
                    {
                        int expansionListCount = expansionList.Count;
                        for(int j = 0; j < storable.Quantity; j++)
                        {
                            StorableScriptableObject sso = expansionList[UnityEngine.Random.Range(0, expansionListCount)];
                            StorableScriptableObject ssoInstance = sso.InstancableSO.GetOrCreateInstance() as StorableScriptableObject;
                            
                            ssoInstance.Quantity = 1;

                            AddToStorableList(ssoInstance, dontReward);
                        }
                    }
                    else
                        AddToStorableList(storable, dontReward);
                }
            }
        }
        public static void AddStorable(StorableScriptableObject storable, bool dontReward = false)
        {
            if (!isEnabled)
            {
                TafraDebugger.Log("Consumable Rewarder", "Consumable rewarder is not initialized properly.", TafraDebugger.LogType.Error);
                return;
            }

            if(!storable.IsInstance)
            {
                Debug.LogError($"Storable with the ID ({storable.OriginalID}) is not an instance.");
                return;
            }

            if (playerInventory == null)
                playerInventory = SceneReferences.PlayerHealthy.GetComponent<Inventory>();

            if(storable.Quantity > 0)
            {
                if(expandableStorableByStorable.TryGetValue(storable.OriginalID, out var expansionList))
                {
                    int expansionListCount = expansionList.Count;
                    for(int j = 0; j < storable.Quantity; j++)
                    {
                        StorableScriptableObject sso = expansionList[UnityEngine.Random.Range(0, expansionListCount)];
                        StorableScriptableObject ssoInstance = sso.InstancableSO.GetOrCreateInstance() as StorableScriptableObject;

                        ssoInstance.Quantity = 1;

                        AddToStorableList(ssoInstance, dontReward);
                    }
                }
                else
                    AddToStorableList(storable, dontReward);
            }
        }
        public static void ClearAddedRewards()
        {
            addedConsumableRewards.Clear();
            addedStorableRewards.Clear();
        }
        public static void ShowScreen(Action onHideAction = null)
        {
            if (!isEnabled)
            {
                TafraDebugger.Log("Consumable Rewarder", "Consumable rewarder is not initialized properly.", TafraDebugger.LogType.Error);
                return;
            }

            endAction = onHideAction;

            screen.OnHide.AddListener(OnScreenHide);
            
            screen.Populate(addedConsumableRewards,addedStorableRewards);
            
            screen.Show();

            if(settings.FreezeTimeScale)
                TimeScaler.SetTimeScale(nameof(ItemRewarder),0);

            if (settings.ShowScreenSFX.Clip != null)
                SFXPlayer.Play(settings.ShowScreenSFX);
        }
        #endregion

        private static void AddToStorableList(StorableScriptableObject storable, bool dontReward = false)
        {
            if(storable.IsStackable)
            {
                bool foundAlreadyAddedStorable = false;
                for(int i = 0; i < addedStorableRewards.Count; i++)
                {
                    var addedStorable = addedStorableRewards[i];

                    if(storable.OriginalID == addedStorable.OriginalID)
                    {
                        foundAlreadyAddedStorable = true;
                        addedStorable.Quantity += storable.Quantity;
                        break;
                    }
                }

                if(foundAlreadyAddedStorable && !dontReward)
                { 
                    playerInventory.AddItem(storable);
                }
                else if (!foundAlreadyAddedStorable)
                {
                    addedStorableRewards.Add(storable);

                    if(!dontReward)
                    {
                        //Send a new instance to the players inventory so that our version's quantity doesn't get changed when the inventory version does.
                        StorableScriptableObject sso = storable.InstancableSO.CreateInstance() as StorableScriptableObject;
                        sso.Quantity = storable.Quantity;
                        playerInventory.AddItem(sso);
                    }
                }
            }
            else
            {
                addedStorableRewards.Add(storable);
                if (!dontReward)
                    playerInventory.AddItem(storable);
            }
        }

        #region Callbacks
        private static void OnScreenHide()
        {
            screen.OnHide.RemoveListener(OnScreenHide);
            
            addedConsumableRewards.Clear();
            addedStorableRewards.Clear();
            
            endAction?.Invoke();
            
            if(settings.FreezeTimeScale)
                TimeScaler.RemoveTimeScaleControl(nameof(ItemRewarder));
            
            if (settings.ClaimSFX.Clip != null)
                SFXPlayer.Play(settings.ClaimSFX);
        }
        #endregion
    }
}
