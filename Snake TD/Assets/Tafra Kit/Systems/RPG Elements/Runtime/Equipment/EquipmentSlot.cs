using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.RPG
{
    [Serializable]
    public class EquipmentSlot 
    {
        [SerializeField] private TafraString category;
        [SerializeField] private int slotsCount = 1;

        [NonSerialized] private List<Equipment> currentEquipment = new List<Equipment>();
        [NonSerialized] private List<Equipment> currentSaveableEquipment = new List<Equipment>();
        [NonSerialized] private int equippedItemsCount;

        public string Category => category.Value;
        public int SlotsCount => slotsCount;
        public List<Equipment> CurrentEquipment => currentEquipment;
        public List<Equipment> CurrentSaveableEquipment => currentSaveableEquipment;
        public bool HasAnyEquipment => equippedItemsCount > 0;

        public EquipmentSlot()
        {
            currentEquipment = new List<Equipment>();
            slotsCount = 1;
        }

        public void AddEquipmentInSlot(int slotIndex, Equipment equipment, bool save)
        {
            if (currentEquipment[slotIndex] == null)
                equippedItemsCount++;

            currentEquipment[slotIndex] = equipment;
            currentSaveableEquipment[slotIndex] = save ? equipment : null;
        }
        public void RemoveEquipmentFromSlot(int slotIndex)
        {
            if (currentEquipment[slotIndex] == null)
                return;

            currentEquipment[slotIndex] = null;
            currentSaveableEquipment[slotIndex] = null;

            equippedItemsCount--;
        }
    }
}