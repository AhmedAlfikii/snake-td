using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Consumables;
using TafraKit.RPG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit
{
    public class ItemSlotSkinner : MonoBehaviour
    {
        [Serializable]
        public class ConsumableBackground
        {
            public Consumable consumable;
            public Sprite background;
        }

        [Space()]

        [SerializeField] private Image[] icons;
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI quantity;
        [SerializeField] private bool compactQuantity = true;

        [Header("Equipment Specific")]
        [SerializeField] private GameObject equipmentTypeHolder;
        [SerializeField] private Image equipmentTypeIcon;
        [SerializeField] private GameObject equipmentLevelHolder;
        [SerializeField] private TextMeshProUGUI equipmentLevel;
        [SerializeField] private GameObject intermediaryRarityHolder;
        [SerializeField] private Image intermediaryRarityBG;
        [SerializeField] private TextMeshProUGUI intermediaryRarityNumber;
        [SerializeField] private Color[] equimpentTypeRarityColors;
        [SerializeField] private GameObject sTierBanner;

        [Header("Sprites")]
        [SerializeField] private Sprite lockedBackground;
        [SerializeField] private Sprite[] rarityBackgrounds;
        [SerializeField] private Sprite[] intermediaryRarityBackgrounds;

        [Header("Consumables")]
        [SerializeField] private ConsumableBackground[] consumableBackgrounds;
        [SerializeField] private Sprite defaultConsumableBackground;

        [Header("Other Storables")]
        [SerializeField] private Sprite defaultStorableBackground;

        private ScriptableObject curItem;
        private bool isInitialized;
        private Dictionary<Consumable, Sprite> backgroundByConsumable = new Dictionary<Consumable, Sprite>();

        private void Awake()
        {
            if(!isInitialized)
                Initialize();
        }
        private void Initialize()
        {
            if(!isInitialized)
                return;

            for (int i = 0; i < consumableBackgrounds.Length; i++)
            {
                var cb = consumableBackgrounds[i];

                backgroundByConsumable.Add(cb.consumable, cb.background);
            }

            isInitialized = true;
        }

        public void Populate(ScriptableObject item, int quantityOverride = -1, bool isAvailable = true)
        {
            if(!isInitialized)
                Initialize();

            curItem = item;

            if(item is Consumable consumable)
                PopulateConsumable(consumable, quantityOverride, isAvailable);
            else if(item is Equipment equipment)
                PopulateEquipment(equipment, isAvailable);
            else if(item is StorableScriptableObject storable)
                PopulateStorable(storable, quantityOverride, isAvailable);
        }
        public void Refresh()
        {
            Populate(curItem);
        }

        private void PopulateConsumable(Consumable consumable, int quantityOverride = -1, bool isAvailable = true)
        {
            //Icons
            for(int i = 0; i < icons.Length; i++)
            {
                if(!isAvailable && consumable.Icons.Count >= 3)
                    icons[i].sprite = consumable.GetIcon(2);
                else
                    icons[i].sprite = consumable.Icon;
            }

            //Background
            if(isAvailable)
            {
                Sprite targetSprite;
                    
                backgroundByConsumable.TryGetValue(consumable, out targetSprite);

                if(targetSprite == null)
                    targetSprite = defaultConsumableBackground;

                background.sprite = targetSprite;
            }
            else
                background.sprite = lockedBackground;

            //Quantity
            int usableQuantity = quantityOverride > -1 ? quantityOverride : consumable.ValueInt;
            quantity.text = "<size=80%>x</size>" + (compactQuantity ? ZHelper.CompactNumberString(usableQuantity) : consumable.ValueInt.ToString());

            if(!quantity.gameObject.activeSelf)
                quantity.gameObject.SetActive(true);

            //Disable equipment related game objects
            if(equipmentLevelHolder != null && equipmentLevelHolder.gameObject.activeSelf)
            {
                equipmentLevelHolder.gameObject.SetActive(false);
                equipmentTypeHolder.SetActive(false);
                intermediaryRarityHolder.gameObject.SetActive(false);
            }
        }
        private void PopulateEquipment(Equipment equipment, bool isAvailable = true)
        {
            /*
            //Icons
            for(int i = 0; i < icons.Length; i++)
            {
                if(!isAvailable && equipment.LockedIcon != null)
                    icons[i].sprite = equipment.LockedIcon;
                else
                    icons[i].sprite = equipment.Icon;
            }

            int rarity = equipment.Rarity;

            //Background
            if(isAvailable)
                background.sprite = rarityBackgrounds[rarity];
            else
                background.sprite = lockedBackground;

            //Quantity
            if(quantity.gameObject.activeSelf)
                quantity.gameObject.SetActive(false);

            //Equipment level
            equipmentLevel.text = $"Lv. {equipment.Level}";

            //Equipment S-Tier
            if(sTierBanner != null)
                sTierBanner.SetActive(equipment.Tier == 1);

            if(!equipmentLevelHolder.gameObject.activeSelf)
                equipmentLevelHolder.gameObject.SetActive(true);

            equipmentTypeIcon.sprite = EquipmentProperties.Settings.GetTypeIcon(equipment.Category);
            equipmentTypeIcon.color = equimpentTypeRarityColors[rarity];

            if(!equipmentTypeHolder.activeSelf)
                equipmentTypeHolder.SetActive(true);

            string rarityName = EquipmentProperties.Settings.GetRarityName(equipment.Rarity);

            int plusCharIndex = rarityName.IndexOf('+');
            bool isIntermediary = plusCharIndex != -1;

            if(isIntermediary)
            {
                int intermediaryNumber = int.Parse(rarityName.Substring(plusCharIndex));

                intermediaryRarityNumber.text = intermediaryNumber.ToString();
                intermediaryRarityBG.sprite = intermediaryRarityBackgrounds[rarity];

                if(!intermediaryRarityHolder.gameObject.activeSelf)
                    intermediaryRarityHolder.gameObject.SetActive(true);
            }
            else if(intermediaryRarityHolder.gameObject.activeSelf)
                intermediaryRarityHolder.gameObject.SetActive(false);
            */
        }
        private void PopulateStorable(StorableScriptableObject storable, int quantityOverride = -1, bool isAvailable = true)
        {
            /*
            //Icons
            for(int i = 0; i < icons.Length; i++)
            {
                if(!isAvailable && storable.LockedIcon != null)
                    icons[i].sprite = storable.LockedIcon;
                else
                    icons[i].sprite = storable.Icon;
            }

            //Background
            if(isAvailable)
                background.sprite = defaultStorableBackground;
            else
                background.sprite = lockedBackground;

            //Quantity
            int usableQuantity = quantityOverride > -1 ? quantityOverride : storable.Quantity;
            quantity.text = "<size=80%>x </size>" + (compactQuantity ? ZHelper.CompactNumberString(usableQuantity) : storable.Quantity.ToString());

            if(!quantity.gameObject.activeSelf)
                quantity.gameObject.SetActive(true);

            //Disable equipment related game objects
            if(equipmentLevelHolder.gameObject.activeSelf)
            {
                equipmentLevelHolder.gameObject.SetActive(false);
                equipmentTypeHolder.SetActive(false);
                intermediaryRarityHolder.gameObject.SetActive(false);
            }
            */
        }
    }
}