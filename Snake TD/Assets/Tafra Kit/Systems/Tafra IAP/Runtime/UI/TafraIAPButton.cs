#if TAFRA_IAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using TMPro;
using TafraKit.UI;
using UnityEngine.Purchasing.Extension;

namespace TafraKit.IAP
{
    public class TafraIAPButton : MonoBehaviour, ITafraIAPListener
    {
        [SerializeField] private string productID;
        [SerializeField] private TextMeshProUGUI priceTXT;
        [SerializeField] private ZButton button;
        [SerializeField] private bool disableIfUnavailableToPurchase = true;
        [SerializeField] private string placement = "iapshop";

        public UnityEvent<Product> OnPurchaseSuccess = new UnityEvent<Product>();
        public UnityEvent<Product, PurchaseFailureDescription> OnPurchaseFailure = new UnityEvent<Product, PurchaseFailureDescription>();

        private bool dataFetched;

        public string ProductID => productID;

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
            priceTXT.text = "Loading...";
        }
        private void OnEnable()
        {
            if(!dataFetched)
                StartCoroutine(FetchingData());
        }

        private IEnumerator FetchingData()
        {
            while(!TafraIAP.IsInitialized)
            {
                yield return null;
            }

            dataFetched = true;

            Product product = TafraIAP.GetProduct(productID);

            if (disableIfUnavailableToPurchase && !product.availableToPurchase)
            {
                gameObject.SetActive(false);
                yield break;
            }

            if(product == null)
                yield break;

            priceTXT.text = product.metadata.localizedPriceString;
        }

        private void OnClick()
        {
            if(!dataFetched)
                return;

            TafraIAP.Purchase(productID, name, placement, this);
        }

        public void OnPurchaseSuccessful(Product product)
        {
            Debug.Log($"Button purchase successful. Button:{gameObject.name} Product ID: {product.definition.id}.");
            OnPurchaseSuccess?.Invoke(product);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"Button purchase failed. Button:{gameObject.name} Product ID: {product.definition.id}.");
            OnPurchaseFailure?.Invoke(product, failureDescription);
        }
    }
}
#endif