using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TafraKit.MotionFactory;

namespace TafraKit.RPG
{
    public class InventoryItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image iconIMG;
        [SerializeField] private TextMeshProUGUI quantityTXT;
        [SerializeField] private InOutMotionController selectedMC;

        private StorableScriptableObject item;
        private RectTransform myRT;
        private Action<InventoryItemSlot> onClick;

        public RectTransform MyRT => myRT;
        public StorableScriptableObject Item => item;
        public Action<InventoryItemSlot> OnClick { get { return onClick; } set { onClick = value; } }

        private void OnEnable()
        {
            WakeUp();
        }
        private void OnDisable()
        {
            Sleep();
        }

        public void Initialize(StorableScriptableObject storableItem)
        {
            item = storableItem;
            
            if (myRT == null)
                myRT = GetComponent<RectTransform>();

            if(gameObject.activeInHierarchy)
                WakeUp();
        }
        public void RefreshData()
        {
            OnQuantityChange();
        }
        public void WakeUp()
        {
            if (!item)
                return;

            iconIMG.sprite = item.RequestIcon();
            quantityTXT.text = item.Quantity.ToString();

            item.OnQuantityChange.AddListener(OnQuantityChange);
        }
        public void Sleep()
        {
            if (!item)
                return;

            iconIMG.sprite = null;

            item.ReleaseIcon();

            item.OnQuantityChange.RemoveListener(OnQuantityChange);
        }

        private void OnQuantityChange()
        {
            quantityTXT.text = item.Quantity.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(this);
        }

        public void SetSelectedState(bool selected)
        {
            if (selected)
                selectedMC.Show(false);
            else
                selectedMC.Hide(false);
        }
    }
}