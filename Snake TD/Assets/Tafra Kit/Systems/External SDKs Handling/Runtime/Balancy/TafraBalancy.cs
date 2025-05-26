#if TAFRA_BALANCY
using Balancy;
using Balancy.Interfaces;
using Balancy.Models.SmartObjects;
using Balancy.SmartObjects;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TafraKit.Consumables;
using TafraKit.Mathematics;
using TafraKit.RPGElements;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.ExternalSDKs.BalancySDK
{
    #if TAFRA_BALANCY
    public static class TafraBalancy
    {
        private static TafraBalancySettings settings;
        private static bool isInitialized;
        private static bool isWaitingLogin = true;
        private static InitializationState initializationState;
        private static BalancyEvents events = new BalancyEvents();
        private static Dictionary<int, Consumable> consumableByUnnyID = new Dictionary<int, Consumable>();
        private static Dictionary<Consumable, int> unnyIDByConsumable = new Dictionary<Consumable, int>();
        private static Dictionary<int, StorableScriptableObject> storableByUnnyID = new Dictionary<int, StorableScriptableObject>();
        private static List<ConsumableChange> tempConsumableChangeRewards = new List<ConsumableChange>();
        private static List<StorableScriptableObject> tempStorableRewards = new List<StorableScriptableObject>();

        private static UnityEvent onInitializationStarted = new UnityEvent();
        private static UnityEvent<bool> onInitializationFinished = new UnityEvent<bool>();
        private static UnityEvent onWaitingLoginFinished = new UnityEvent();
        
        private static bool debugLogs = false;

        public static bool IsInitialized => isInitialized;
        public static bool IsWaitingLogin => isWaitingLogin;
        public static bool IsAuthenticated => Auth.IsAuthorized();
        public static string UserID => Auth.GetUserId();
        public static InitializationState InitializationState => initializationState;
        public static BalancyEvents Events => events;
        public static UnityEvent OnInitializationStarted => onInitializationStarted;
        public static UnityEvent<bool> OnInitializationFinished => onInitializationFinished;
        public static UnityEvent OnWaitingLoginFinished => onWaitingLoginFinished;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void PreInitialize()
        {
            settings = TafraSettings.GetSettings<TafraBalancySettings>();

            if(settings == null || !settings.Enabled)
                return;

            for(int i = 0; i < settings.BalancyConsumables.Length; i++)
            {
                var balancyConsumable = settings.BalancyConsumables[i];

                if(balancyConsumable.consumable == null)
                    continue;

                consumableByUnnyID.Add(balancyConsumable.unnyID, balancyConsumable.consumable);
                unnyIDByConsumable.Add(balancyConsumable.consumable, balancyConsumable.unnyID);
            }

            for(int i = 0; i < settings.BalancyStorables.Length; i++)
            {
                var balancyStorable = settings.BalancyStorables[i];

                if(balancyStorable.storable == null)
                    continue;

                storableByUnnyID.Add(balancyStorable.unnyID, balancyStorable.storable);
            }

        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (settings == null || !settings.Enabled)
                return;

            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Attempting to initialize...</b></color>");

            initializationState = InitializationState.InProgress;

            RegisterSmartObjectsListener(events);

            onInitializationStarted?.Invoke();

            int updatePeriod = 300;

            #if UNITY_EDITOR
            updatePeriod = 10;
            #endif

            AppConfig appConfig = new AppConfig()
            {
                ApiGameId = settings.APIGameID,
                PublicKey = settings.PublicKey,
                Environment = settings.Environment,
                AutoLogin = settings.AutoLoginWithDeviceID,
                PreInit = settings.PreInitType,
                UpdatePeriod = updatePeriod,

                #if UNITY_EDITOR
                Platform = Constants.Platform.Unknown,
                #endif

                OnInitProgress = OnInitProgress,
                OnContentUpdateCallback = OnContentUpdate,
                OnReadyCallback = OnReady
            };

            Main.Init(appConfig);
        }

        #region Callbacks
        private static void OnInitProgress(InitProgress progress)
        {
            if(debugLogs)
                Debug.Log($"<color=green><b>Balancy - OnInitProgress - Status: {progress.Status}...</b></color>");

            switch(progress.Status)
            {
                case BalancyInitStatus.PreInitFromResourcesOrCache:
                    break;
                case BalancyInitStatus.PreInitLocalProfile:
                    break;
                case BalancyInitStatus.DictionariesReady:
                    break;
                case BalancyInitStatus.Finished:
                    break;
            }
        }
        private static void OnContentUpdate(LoaderResponseData data)
        {
            if(debugLogs)
                Debug.Log($"<color=green><b>Balancy - OnContentUpdate - Affected dictionaries: {data.AffectedDictionaries.Length}.</b></color>");
        }
        private static void OnReady(InitializedResponseData data)
        {
            if(debugLogs)
                Debug.Log($"<color=green><b>Balancy - Initialization complete: success = {data.Success}, deploy version = {data.DeployVersion}. Is authenticated? {Auth.IsAuthorized()}.</b></color>");

            initializationState = data.Success? InitializationState.Succeeded : InitializationState.Failed;
            isInitialized = true;
            isWaitingLogin = data.Success? !Auth.IsAuthorized() : false;

            onInitializationFinished?.Invoke(data.Success);
        }
        #endregion

        #region Public Functions
        public static void AuthWithName(string username, string password)
        {
            Auth.WithName(username, password, (response) =>
            {
                if(response.Success)
                {
                    if(debugLogs)
                        Debug.Log($"<color=green><b>Balancy - Sign in with name successfull. User ID: {Auth.GetUserId()}.</b></color>");
                }
                else
                {
                    if(debugLogs)
                        Debug.Log($"<color=green><b>Balancy - Sign in with name failed..</b></color>");
                }

                if(isWaitingLogin)
                {
                    isWaitingLogin = false;
                    onWaitingLoginFinished?.Invoke();
                }
            });
        }
        public static void RegisterSmartObjectsListener(ISmartObjectsEvents listener)
        {
            ExternalEvents.RegisterSmartObjectsListener(listener);
        }
        public static void RegisterLiveOpsListener(IStoreEvents listener)
        {
            ExternalEvents.RegisterLiveOpsListener(listener);
        }
        public static void RegisterLiveOpsListener(ILiveOpsEvents listener)
        {
            ExternalEvents.RegisterLiveOpsListener(listener);
        }
        public static bool TryGetConsumableByUnnyID(int unnyID, out Consumable consumable)
        {
            return consumableByUnnyID.TryGetValue(unnyID, out consumable);
        }
        public static bool TryGetUnnyIDByConsumable(Consumable consumable, out int unnyID)
        {

            return unnyIDByConsumable.TryGetValue(consumable, out unnyID);
        }
        public static bool TryGetStorableByUnnyID(int unnyID, out StorableScriptableObject storable)
        {
            return storableByUnnyID.TryGetValue(unnyID, out storable);
        }
        public static void ExtractConsumablesAndStorables(ItemWithAmount[] balancyItemsWithAmount, List<ConsumableChange> consumableChangesToFill, List<StorableScriptableObject> storablesToFill)
        {
            if(balancyItemsWithAmount == null)
                return;

            for(int i = 0; i < balancyItemsWithAmount.Length; i++)
            {
                var reward = balancyItemsWithAmount[i];

                if(consumableChangesToFill != null)
                {
                    Consumable consumable;

                    TryGetConsumableByUnnyID(reward.Item.IntUnnyId, out consumable);

                    if(consumable != null)
                    {
                        ConsumableChange change = new ConsumableChange(consumable, reward.Count);

                        consumableChangesToFill.Add(change);
                    }
                }

                if(storablesToFill != null)
                {
                    StorableScriptableObject storable;

                    TryGetStorableByUnnyID(reward.Item.IntUnnyId, out storable);

                    if(storable != null)
                    {
                        StorableScriptableObject storableInstance = storable.InstancableSO.GetOrCreateInstance() as StorableScriptableObject;

                        storableInstance.Quantity = reward.Count;

                        storablesToFill.Add(storableInstance);
                    }
                }
            }
        }
        public static void ShowRewardScreen(ItemWithAmount[] items)
        {
            tempConsumableChangeRewards.Clear();
            tempStorableRewards.Clear();

            ExtractConsumablesAndStorables(items, tempConsumableChangeRewards, tempStorableRewards);

            ItemRewarder.AddConsumables(tempConsumableChangeRewards);
            ItemRewarder.AddStorables(tempStorableRewards);

            ItemRewarder.ShowScreen();
        }
        public static bool IsSoftPriceLocallyAvailable(Balancy.Models.SmartObjects.Price price)
        {
            if(!price.IsSoft())
                return true;

            for (int i = 0; i < price.Items.Length; i++)
            {
                var itemWithAmount = price.Items[i];
                var item = itemWithAmount.Item;

                if(item == null)
                    continue;

                if(TryGetConsumableByUnnyID(item.IntUnnyId, out Consumable consumable) && consumable.Value < itemWithAmount.Count)
                {
                    return false;
                }
                else if(TryGetStorableByUnnyID(item.IntUnnyId, out StorableScriptableObject storable) 
                    && SceneReferences.PlayerInventory != null && SceneReferences.PlayerInventory.TryGetItemByOriginalID(storable.OriginalID, out var inInventoryStorable)
                    && inInventoryStorable.Quantity < itemWithAmount.Count)
                {
                    return false;
                }
            }

            return true;
        }
        public static void ExtractFormulas(Balancy.Models.Formula[] formulas, List<TafraKit.Mathematics.EquationBase> equationsToFill)
        {
            for (int i = 0; i < formulas.Length; i++)
            {
                var formula = formulas[i];

                if(formula == null)
                    continue;

                if(formula is Balancy.Models.ExactFormula bExactFormula)
                {
                    ExactEquation exactEquation = new ExactEquation(bExactFormula.MinInput, bExactFormula.MaxInput);

                    equationsToFill.Add(exactEquation);
                }
                else if(formula is Balancy.Models.ExponentialFormula bExponentialFormula)
                {
                    ExponentialEquation exponentialEquation = new ExponentialEquation(bExponentialFormula.MinInput, bExponentialFormula.MaxInput, bExponentialFormula.A, bExponentialFormula.B, bExponentialFormula.C);
                    equationsToFill.Add(exponentialEquation);
                }
                else if(formula is Balancy.Models.FixedNumberFormula bFixedNumberFormula)
                {
                    FixedNumberEquation fixedNumberEquation = new FixedNumberEquation(bFixedNumberFormula.MinInput, bFixedNumberFormula.MaxInput, bFixedNumberFormula.Value);
                    equationsToFill.Add(fixedNumberEquation);
                }
                else if(formula is Balancy.Models.LinearFormula bLinearFormula)
                {
                    LinearEquation linearEquation = new LinearEquation(bLinearFormula.MinInput, bLinearFormula.MaxInput, bLinearFormula.A, bLinearFormula.B);
                    equationsToFill.Add(linearEquation);
                }
                else if(formula is Balancy.Models.LinearIncrementalFormula bLinearIncrementalFormula)
                {
                    LinearIncrementalEquation linearIncrementalEquation = new LinearIncrementalEquation(bLinearIncrementalFormula.MinInput, bLinearIncrementalFormula.MaxInput, bLinearIncrementalFormula.PivotX,
                        bLinearIncrementalFormula.StartValue, bLinearIncrementalFormula.Increments, bLinearIncrementalFormula.IsLimited, bLinearIncrementalFormula.Limit);
                    equationsToFill.Add(linearIncrementalEquation);
                }
                else if(formula is Balancy.Models.LogarithmicFormula bLogarithmicFormula)
                {
                    LogarithmicEquation logarithmicEquation = new LogarithmicEquation(bLogarithmicFormula.MinInput, bLogarithmicFormula.MaxInput, bLogarithmicFormula.A, bLogarithmicFormula.B);
                    equationsToFill.Add(logarithmicEquation);
                }
                else if(formula is Balancy.Models.QuadraticFormula bQuadraticFormula)
                {
                    QuadraticEquation quadraticEquation = new QuadraticEquation(bQuadraticFormula.MinInput, bQuadraticFormula.MaxInput, bQuadraticFormula.A, bQuadraticFormula.B, bQuadraticFormula.C);
                    equationsToFill.Add(quadraticEquation);
                }
                else if(formula is Balancy.Models.ManualValuesFormula bManualValuesFormula)
                {
                    ManualValuesEquation manualValuesEquation = new ManualValuesEquation(bManualValuesFormula.MinInput, bManualValuesFormula.MaxInput, bManualValuesFormula.ValueStartInput, bManualValuesFormula.Values);
                    equationsToFill.Add(manualValuesEquation);
                }
            }
        }
        #endregion
    }
    #endif
}
