#if TAFRA_IAP
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace TafraKit.IAP
{
    public abstract class IAPValidator
    {
        public abstract Task<IAPValidationResults> ValidateReceipt(PurchaseEventArgs purchaseEventArgs, IAPProduct productInfo, CancellationToken ct);
    }
}
#endif