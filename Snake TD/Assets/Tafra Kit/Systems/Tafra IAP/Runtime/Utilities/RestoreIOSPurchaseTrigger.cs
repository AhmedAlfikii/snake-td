#if TAFRA_IAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.IAP
{
    public class RestoreIOSPurchaseTrigger : MonoBehaviour
    {
        public void RestorePurchases()
        {
            TafraIAP.RestoreIOSPurchases();
        }
    }
}
#endif