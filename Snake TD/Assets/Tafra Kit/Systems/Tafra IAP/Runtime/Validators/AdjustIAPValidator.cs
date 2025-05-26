#if TAFRA_ADJUST && TAFRA_ADJUST_PURCHASING
using com.adjust.sdk;
using com.adjust.sdk.purchase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

namespace TafraKit.IAP
{
    public class AdjustIAPValidator : IAPValidator
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            //TafraIAP.SetPurchaseValidator(new AdjustIAPValidator());
        }

        public override async Task<IAPValidationResults> ValidateReceipt(PurchaseEventArgs purchaseEventArgs, IAPProduct productInfo, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                #if UNITY_EDITOR
                return IAPValidationResults.Passed;
                #endif

                decimal localPrice = purchaseEventArgs.purchasedProduct.metadata.localizedPrice;
                double price = decimal.ToDouble(localPrice);
                string currencyCode = purchaseEventArgs.purchasedProduct.metadata.isoCurrencyCode;
                string transactionID = purchaseEventArgs.purchasedProduct.transactionID;
                string productID = purchaseEventArgs.purchasedProduct.definition.id;
                Dictionary<string, object> receiptDict = (Dictionary<string, object>)MiniJson.JsonDecode(purchaseEventArgs.purchasedProduct.receipt);
                string payload = (null != receiptDict && receiptDict.ContainsKey("Payload")) ? (string)receiptDict["Payload"] : "";

                ADJPVerificationInfo verificationInfo = null;

                #if UNITY_IOS
                AdjustPurchase.VerifyPurchaseiOS(payload, transactionID, productID, OnVerificationInfoFetched);
                #elif UNITY_ANDROID
                var jsonDetailsDict = (!string.IsNullOrEmpty(payload)) ? (Dictionary<string, object>)MiniJson.JsonDecode(payload) : null;
                var json = (jsonDetailsDict != null && jsonDetailsDict.ContainsKey("json")) ? (string)jsonDetailsDict["json"] : "";
                var gpDetailsDict = (!string.IsNullOrEmpty(json)) ? (Dictionary<string, object>)MiniJson.JsonDecode(json) : null;
                var purchaseToken = (null != gpDetailsDict && gpDetailsDict.ContainsKey("purchaseToken")) ?
                (string)gpDetailsDict["purchaseToken"] : "";
                AdjustPurchase.VerifyPurchaseAndroid(productID, purchaseToken, "", OnVerificationInfoFetched);
                #endif

                void OnVerificationInfoFetched(ADJPVerificationInfo vInfo)
                {
                    verificationInfo = vInfo;
                }

                while(verificationInfo == null)
                {
                    ct.ThrowIfCancellationRequested();

                    await Task.Yield();
                }

                ct.ThrowIfCancellationRequested();

                IAPValidationResults state = IAPValidationResults.Unkown;
                switch(verificationInfo.VerificationState)
                {
                    case ADJPVerificationState.ADJPVerificationStatePassed:
                        state = IAPValidationResults.Passed;
                        break;
                    case ADJPVerificationState.ADJPVerificationStateFailed:
                    case ADJPVerificationState.ADJPVerificationStateNotVerified:
                        state = IAPValidationResults.Failed;
                        break;
                    case ADJPVerificationState.ADJPVerificationStateUnknown:
                        state = IAPValidationResults.Unkown;
                        break;
                }

                return state;
            }
            catch(OperationCanceledException) 
            {
                return IAPValidationResults.Failed;
            }
            catch
            {
                return IAPValidationResults.Failed;
            }
        }
    }
}
#endif