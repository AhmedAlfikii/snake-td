using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TafraKit.UI;

namespace TafraKit.RPG
{
    public class InventoryEquipmentItemInfoPanel : InventoryItemInfoPanel
    {
        [SerializeField] private TextMeshProUGUI titleTXT;
        [SerializeField] private Image iconIMG;
        [SerializeField] private TextMeshProUGUI descriptionTXT;
        [SerializeField] private ZButton equipBTN;
        [SerializeField] private ZButton UnequipBTN;

        public override Type ItemType => typeof(Equipment);

        private Equipment equipment;

        public override void SetData(StorableScriptableObject item)
        {
            equipment = item as Equipment;

            titleTXT.text = item.name;
            iconIMG.sprite = item.GetIconIfLoaded();
            descriptionTXT.text = item.Description;

            equipBTN.onClick.AddListener(OnEquipButtonClick);
        }

        private void OnEquipButtonClick()
        {
            if (equipment == null || inventory == null || inventory.EquipmentsHolder == null)
                return;

            inventory.EquipmentsHolder.Equip(equipment);
            inventory.RemoveItem(equipment);
        }
    }
}