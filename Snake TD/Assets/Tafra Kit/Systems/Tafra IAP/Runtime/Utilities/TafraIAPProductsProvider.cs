#if TAFRA_IAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using TafraKit.IAP;
using TafraKit.Consumables;
using System;

public class TafraIAPProductsProvider : MonoBehaviour
{
    [System.Serializable]
    public class ProductPurchase
    {
        public string ProductID;

        public ConsumablesReward reward;

        public UnityEvent<Product> OnPurchaseSuccess = new UnityEvent<Product>();
        public UnityEvent<Product, PurchaseFailureDescription> OnPurchaseFailure = new UnityEvent<Product, PurchaseFailureDescription>();
    }

    [SerializeField] private ProductPurchase[] products;

    private Dictionary<string, ProductPurchase> productPurchases = new Dictionary<string, ProductPurchase>();

    private void Awake()
    {
        for (int i = 0; i < products.Length; i++)
        {
            ProductPurchase product = products[i];
            productPurchases.Add(product.ProductID, product);
        }
    }

    private void OnEnable()
    {
        TafraIAP.OnPurchaseSuccessful.AddListener(OnPurchaseSuccessful);
        TafraIAP.OnPurchaseFailure.AddListener(OnPurchaseFailure);
    }

    private void OnDisable()
    {
        TafraIAP.OnPurchaseSuccessful.RemoveListener(OnPurchaseSuccessful);
        TafraIAP.OnPurchaseFailure.RemoveListener(OnPurchaseFailure);
    }

    private void OnPurchaseSuccessful(Product product)
    {
        ProductPurchase productPurchase;
        productPurchases.TryGetValue(product.definition.id, out productPurchase);

        if (productPurchase != null)
        {
            Debug.LogError("Handle This");
            //ConsumableRewarder.Instance.AddRewards(productPurchase.reward.Rewards);
            //ConsumableRewarder.Instance.ShowScreen();
            productPurchase.OnPurchaseSuccess?.Invoke(product);
        }
    }
    private void OnPurchaseFailure(Product product, PurchaseFailureDescription failureDescription)
    {
        ProductPurchase productPurchase;
        productPurchases.TryGetValue(product.definition.id, out productPurchase);

        if (productPurchase != null)
            productPurchase.OnPurchaseFailure?.Invoke(product, failureDescription);
    }
}
#endif