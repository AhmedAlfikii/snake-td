using System;
using System.Collections.Generic;
using TafraKit.Consumables;
using TafraKit.RPG;
using TafraKit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit
{
    public class ItemSlotSkin : UISkinReceiver
    {
        [System.Serializable]
        public class ConsumableSkin
        {
            public Consumable consumable;
            public UISkin skin;
        }

        [Header("Item Slot")]
        [SerializeField] private Image[] icons;
        [SerializeField] private int iconIndex;
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI quantity;
        [SerializeField] private bool compactQuantity = true;

        [Header("Consumables")]
        [SerializeField] private List<ConsumableSkin> consumableSkins = new List<ConsumableSkin>();
        [SerializeField] private UISkin defaultConsumableSkin;

        private ScriptableObject curItem;
        private bool initialized;
        private Dictionary<Consumable, UISkin> skinByConsumable = new Dictionary<Consumable, UISkin>();

        private void Awake()
        {
            if(!initialized)
                Initialize();
        }

        private void Initialize()
        {
            if(initialized)
                return;

            for(int i = 0; i < consumableSkins.Count; i++)
            {
                var cs = consumableSkins[i];

                skinByConsumable.Add(cs.consumable, cs.skin);
            }

            initialized = true;
        }

        public void Populate(ScriptableObject item, int quantityOverride = -1, bool isAvailable = true)
        {
            if(!initialized)
                Initialize();

            curItem = item;

            if(item is Consumable consumable)
                PopulateConsumable(consumable, quantityOverride, isAvailable);
            else if(item is Equipment equipment)
                PopulateEquipment(equipment, isAvailable);
            else if(item is StorableScriptableObject storable)
                PopulateStorable(storable, quantityOverride, isAvailable);
        }

        private void PopulateConsumable(Consumable consumable, int quantityOverride, bool isAvailable)
        {
            //Icons
            for(int i = 0; i < icons.Length; i++)
            {
                if(isAvailable)
                    icons[i].sprite = consumable.GetIcon(iconIndex);
                else
                    icons[i].sprite = consumable.GetLockedIcon(iconIndex);
            }

            //Quantity
            int usableQuantity = quantityOverride > -1 ? quantityOverride : consumable.ValueInt;
            quantity.text = "<size=80%>x</size>" + (compactQuantity ? ZHelper.CompactNumberString(usableQuantity) : consumable.ValueInt.ToString());

            if(!quantity.gameObject.activeSelf)
                quantity.gameObject.SetActive(true);

            UISkin skin;

            skinByConsumable.TryGetValue(consumable, out skin);

            if(skin == null)
                skin = defaultConsumableSkin;

            ApplySkin(skin);
        }
        private void PopulateEquipment(Equipment equipment, bool isAvailable)
        {
            throw new NotImplementedException();
        }
        private void PopulateStorable(StorableScriptableObject storable, int quantityOverride, bool isAvailable)
        {
            throw new NotImplementedException();
        }
    }
}