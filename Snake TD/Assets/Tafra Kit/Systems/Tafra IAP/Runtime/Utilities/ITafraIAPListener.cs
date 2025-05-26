#if TAFRA_IAP
using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace TafraKit.IAP
{
    public interface ITafraIAPListener
    {
        public string ProductID { get;}
        public void OnPurchaseSuccessful(Product product);
        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription);
    }
}
#endif