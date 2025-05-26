#if LK_HAS_LION_ANALYTICS
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Purchasing;
using LionStudios.Suite.Purchasing;
using LionStudios.Suite.Analytics;
using UnityEngine;

namespace TafraKit.IAP
{
    public class LionIAPValidator : IAPValidator
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            TafraIAP.SetPurchaseValidator(new LionIAPValidator());
        }

        public async override Task<IAPValidationResults> ValidateReceipt(PurchaseEventArgs purchaseEventArgs, IAPProduct productInfo, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                //#if UNITY_EDITOR
                //return IAPValidationResults.Passed;
                //#endif

                Dictionary<string, object> additionalData = null;
                
                if (AnalyticsManager.Instance != null)
                    additionalData = AnalyticsManager.Instance.GetIAPAdditionalData();

                List<VirtualCurrency> receivedCurrencies = new List<VirtualCurrency>();

                if (productInfo.GameplayData.Consumable.consumable != null)
                {
                    VirtualCurrency virtualCurrency = new VirtualCurrency(
                                name: productInfo.GameplayData.Consumable.consumable.ID,
                                type: productInfo.GameplayData.ConsumableType,
                                amount: (int)productInfo.GameplayData.Consumable.changeAmount);

                    receivedCurrencies.Add(virtualCurrency);
                }

                var product = purchaseEventArgs.purchasedProduct;
                IAPGameplayInfo gameplayInfo = new IAPGameplayInfo(
                    receivedItems: new List<Item>(),
                    receivedCurrencies: receivedCurrencies,
                    purchaseLocation: productInfo.GameplayData.LastPlacement);

                IAPValidationResults state = IAPValidationResults.Unkown;

                await IAPValidation.ValidateAndLog(product, gameplayInfo,
                    onSuccess: () =>
                    {
                        Debug.Log($"Lion IAP Validation - Success!");
                        state = IAPValidationResults.Passed;
                    },
                    onFailure: (ValidationStatus vs) =>
                    {
                        Debug.Log($"Lion IAP Validation - Failed: {vs}");
                        state = IAPValidationResults.Failed;
                    }, additionalData);

                return state;
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Lion IAP Validation - Canceled!");

                return IAPValidationResults.Failed;
            }
            catch (Exception ex)
            {
                Debug.Log($"Lion IAP Validation - Error: {ex.Message}");

                return IAPValidationResults.Failed;
            }
        }
    }
}
#endif