#if TAFRA_IAP
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Unity.Services.Core;
using System;
using Unity.Services.Core.Environments;
using TafraKit.Tasks;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.IAP
{
    //TODO: Consider stopping the initialize async method when the editor stops playing.
    public class TafraIAP : IDetailedStoreListener
    {
        private static TafraIAPSettings settings;
        
        private static TafraIAP instance;
        private static IStoreController storeController;
        private static IExtensionProvider storeExtentionProvider;
        private static IAPValidator activeValidator;
        private static bool isInitialized;
        private static Dictionary<string, Product> productsByID = new Dictionary<string, Product>();
        private static Dictionary<string, IAPProduct> productsInfoByID = new Dictionary<string, IAPProduct>();
        private static Dictionary<string, ITafraIAPListener> productPurchaseListeners = new Dictionary<string, ITafraIAPListener>();
        private static Dictionary<string, string> productPurchasersIDs = new Dictionary<string, string>();
        private static ZUIScreen purchaseLoadingScreen;
        private static ZUIScreen restoreLoadingScreen;
        private static CancellationTokenSource validationCTS;

        public static UnityEvent<Product, PurchaseFailureDescription> OnPurchaseFailure = new UnityEvent<Product, PurchaseFailureDescription>();
        public static UnityEvent<Product> OnPurchaseSuccessful = new UnityEvent<Product>();
        public static UnityEvent<IAPValidationResults, Product> OnPurchaseValidationResult = new UnityEvent<IAPValidationResults, Product>();
        public static UnityEvent<Product> OnPurchaseRestoredOrContinued = new UnityEvent<Product>();
        public static UnityEvent OnIOSPurchasesRestorationSuccessful = new UnityEvent();
        public static UnityEvent OnIOSPurchasesRestorationFailure = new UnityEvent();

        public static bool IsInitialized => isInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void Initialize()
        {
            settings = TafraSettings.GetSettings<TafraIAPSettings>();

            if (settings == null || !settings.Enabled)
                return;

            if (settings.ProcessPurchaseLoadingScreen != null)
            { 
                GameObject loadingScreenGO = GameObject.Instantiate(settings.ProcessPurchaseLoadingScreen.gameObject);

                GameObject.DontDestroyOnLoad(loadingScreenGO);

                purchaseLoadingScreen = loadingScreenGO.GetComponent<ZUIScreen>();
            }
            if (settings.iOSRestorationLoadingScreen != null)
            { 
                GameObject loadingScreenGO = GameObject.Instantiate(settings.iOSRestorationLoadingScreen.gameObject);

                GameObject.DontDestroyOnLoad(loadingScreenGO);

                restoreLoadingScreen = loadingScreenGO.GetComponent<ZUIScreen>();
            }

            Task<BoolOperationResult> serviceInitTask = UnityServiesInitializer.Initialize();

            await serviceInitTask;

            if (serviceInitTask.Result != BoolOperationResult.Success)
            {
                TafraDebugger.Log("Tafra IAP", "Game service initialization failed, can't initialize IAP.", TafraDebugger.LogType.Error);
                return;
            }

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra IAP", "Game service initialization succeeded, proceeding to initialize IAP.", TafraDebugger.LogType.Info);

            StandardPurchasingModule.Instance().useFakeStoreUIMode = settings.FakeStoreUI;

            instance = new TafraIAP();

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            
            for (int i = 0; i < settings.Products.Length; i++)
            {
                IAPProduct product = settings.Products[i];

                builder.AddProduct(product.ProductID, product.ProductType);
                productsInfoByID.Add(product.ProductID, product);
            }

            UnityPurchasing.Initialize(instance, builder);
    
            #if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            #endif
        }

        #if UNITY_EDITOR
        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            //If we're exiting playmode, cancel the the active tasks.
            if (change == PlayModeStateChange.ExitingPlayMode)
            {
                if (validationCTS != null)
                {
                    validationCTS.Cancel();
                    validationCTS.Dispose();
                }
            }
        }
        #endif

        #region Callbacks
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra IAP", "Purchasing initialized successfully.", TafraDebugger.LogType.Info);

            storeController = controller;
            storeExtentionProvider = extensions;

            Product[] fetchedProducts = storeController.products.all;
            for (int i = 0; i < fetchedProducts.Length; i++)
            {
                Product product = fetchedProducts[i];

                productsByID.Add(product.definition.id, product);
            }

            isInitialized = true;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {

        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            TafraDebugger.Log("Tafra IAP", $"Purchasing initialized failed. Error message: {message}", TafraDebugger.LogType.Error);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            TafraDebugger.Log("Tafra IAP", $"Purchasing product ({failureDescription.productId}) failed due to: {failureDescription.reason}.\nFailure Message: {failureDescription.message}", TafraDebugger.LogType.Error);

            ITafraIAPListener listener;
            productPurchaseListeners.TryGetValue(product.definition.id, out listener);

            if (listener != null)
            {
                listener.OnPurchaseFailed(product, failureDescription);
                productPurchaseListeners.Remove(product.definition.id);
            }

            if (purchaseLoadingScreen)
                purchaseLoadingScreen.Hide();

            OnPurchaseFailure?.Invoke(product, failureDescription);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {

        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra IAP", $"Processing the following purchased product: \"{purchaseEvent.purchasedProduct.definition.id}\".", TafraDebugger.LogType.Info);

            bool isRestoredPurchase = !productPurchasersIDs.ContainsKey(purchaseEvent.purchasedProduct.definition.id);

            if (isRestoredPurchase)
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Tafra IAP", $"A produt is restored: \"{purchaseEvent.purchasedProduct.definition.id}\".", TafraDebugger.LogType.Info);

                productPurchaseListeners.Remove(purchaseEvent.purchasedProduct.definition.id);

                OnPurchaseSuccessful?.Invoke(purchaseEvent.purchasedProduct);
                OnPurchaseRestoredOrContinued?.Invoke(purchaseEvent.purchasedProduct);
                return PurchaseProcessingResult.Complete;
            }

            productsInfoByID.TryGetValue(purchaseEvent.purchasedProduct.definition.id, out var productInfo);

            if(validationCTS != null)
            {
                validationCTS.Cancel();
                validationCTS.Dispose();
            }

            validationCTS = new CancellationTokenSource();

            ValidatePurchase(purchaseEvent, productInfo, validationCTS.Token);

            return PurchaseProcessingResult.Pending;
        }
        #endregion

        #region Private Functions
        private async void ValidatePurchase(PurchaseEventArgs purchaseEvent, IAPProduct productInfo, CancellationToken ct)
        {
            try
            {
                if(settings.DebugLogs)
                    TafraDebugger.Log("Tafra IAP", $"Validating purchase: \"{purchaseEvent.purchasedProduct.definition.id}\"...", TafraDebugger.LogType.Info);

                ct.ThrowIfCancellationRequested();

                //Wait at least one frame to make sure that the purchase was marked as pending.
                await Task.Yield();

                ct.ThrowIfCancellationRequested();

                if(activeValidator == null)
                {
                    if(settings.DebugLogs)
                        TafraDebugger.Log("Tafra IAP", $"No active validator found. Passing the purchase as legitimate.", TafraDebugger.LogType.Info);

                    PurchaseValidated(purchaseEvent.purchasedProduct);
                    OnPurchaseValidationResult?.Invoke(IAPValidationResults.Passed, purchaseEvent.purchasedProduct);
                    return;
                }

                IAPValidationResults validationResults = await activeValidator.ValidateReceipt(purchaseEvent, productInfo, ct);

                ct.ThrowIfCancellationRequested();

                if(settings.DebugLogs)
                    TafraDebugger.Log("Tafra IAP", $"Purchase validator returned the following results: {validationResults}", TafraDebugger.LogType.Info);

                if(settings.BypassValidationFailure || validationResults == IAPValidationResults.Passed)
                {
                    PurchaseValidated(purchaseEvent.purchasedProduct);
                }
                else
                {
                    OnPurchaseFailed(purchaseEvent.purchasedProduct, new PurchaseFailureDescription(purchaseEvent.purchasedProduct.definition.id, PurchaseFailureReason.Unknown, "Couldn't verify purchase."));
                    storeController.ConfirmPendingPurchase(purchaseEvent.purchasedProduct);
                }

                OnPurchaseValidationResult?.Invoke(validationResults, purchaseEvent.purchasedProduct);

                if(productInfo != null)
                    productInfo.GameplayData.LastPlacement = "";
            }
            catch(OperationCanceledException)
            {
                if(productInfo != null)
                    productInfo.GameplayData.LastPlacement = "";
            }
            //catch(Exception e)
            //{
            //    TafraDebugger.Log("Tafra IAP", $"Purchase processing error: {e.Message}", TafraDebugger.LogType.Error);
            //}
        }
        private void PurchaseValidated(Product product)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra IAP", $"Purchase validated: \"{product.definition.id}\".", TafraDebugger.LogType.Info);

            ITafraIAPListener listener;
            productPurchaseListeners.TryGetValue(product.definition.id, out listener);

            if (listener != null)
            {
                listener.OnPurchaseSuccessful(product);
                productPurchaseListeners.Remove(product.definition.id);
            }

            if (purchaseLoadingScreen)
                purchaseLoadingScreen.Hide();

            OnPurchaseSuccessful?.Invoke(product);

            storeController.ConfirmPendingPurchase(product);
        }
        #endregion

        #region Public Functions
        public static void Purchase(string productID, string purchaserId, string placement, ITafraIAPListener listener = null)
        {
            if (!IsInitialized)
            {
                TafraDebugger.Log("Tafra IAP", "Purchasing is not initialized, can't make a purchase.", TafraDebugger.LogType.Error);

                if (listener != null)
                    listener.OnPurchaseFailed(null, null);

                return;
            }

            Product product;
            productsByID.TryGetValue(productID, out product);

            if (product == null)
            {
                TafraDebugger.Log("Tafra IAP", $"Couldn't find a product with the given id ({productID}).", TafraDebugger.LogType.Error);

                if (listener !=null)
                    listener.OnPurchaseFailed(null, null);

                return;
            }

            if (listener != null)
                productPurchaseListeners.TryAdd(productID, listener);

            productPurchasersIDs.TryAdd(productID, purchaserId);

            if (purchaseLoadingScreen)
                purchaseLoadingScreen.Show();

            if(productsInfoByID.TryGetValue(productID, out var productInfo))
                productInfo.GameplayData.LastPlacement = placement;

            storeController.InitiatePurchase(product);
        }
        public static void RestoreIOSPurchases()
        {
            if (!isInitialized)
            {
                TafraDebugger.Log("Tafra IAP", "Not initialized, can't restore iOS purchases. Make sure to enable this feature in Tafra Kit window.", TafraDebugger.LogType.Error);
                return;
            }

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra IAP", "Attempting to restore iOS purchases.", TafraDebugger.LogType.Info);

            if (restoreLoadingScreen)
                restoreLoadingScreen.Show();

            storeExtentionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((result, error) =>
            {
                if (result)
                {
                    if (settings.DebugLogs)
                        TafraDebugger.Log("Tafra IAP", $"Restoration process done.", TafraDebugger.LogType.Info);

                    OnIOSPurchasesRestorationSuccessful?.Invoke();
                }
                else
                {
                    if (settings.DebugLogs)
                        TafraDebugger.Log("Tafra IAP", $"Restoration process failed.", TafraDebugger.LogType.Info);

                    OnIOSPurchasesRestorationFailure?.Invoke();
                }

                if (restoreLoadingScreen)
                    restoreLoadingScreen.Hide();
            });
        }
        public static void SetPurchaseValidator(IAPValidator validator)
        {
            activeValidator = validator;

            if(settings.DebugLogs)
                TafraDebugger.Log("Tafra IAP", $"A purchase validator was assigned ({validator.GetType().Name}).", TafraDebugger.LogType.Info);
        }
        public static Product GetProduct(string productID)
        {
            Product product;
            productsByID.TryGetValue(productID, out product);
            return product;
        }
        public static IAPProduct GetProductInfo(string productID)
        {
            productsInfoByID.TryGetValue(productID, out var productInfo);
            return productInfo;
        }
        public static string GetProductID(Product product)
        {
            return productsByID.FirstOrDefault(x => x.Value == product).Key;
        }
        #endregion
    }
}
#endif