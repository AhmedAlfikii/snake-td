#if TAFRA_IAP
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace TafraKit.IAP
{
    [Serializable]
    public class IAPProduct
    {
        public string ProductID;
        public ProductType ProductType;
        public IAPProductGameplayIData GameplayData;
    }
}
#endif