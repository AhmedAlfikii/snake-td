using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

namespace TafraKit.RPG
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private RectTransform[] slotPlacements;
        [SerializeField] private DynamicPool<InventoryItemSlot> itemSlotsPool;
        [SerializeField] private DynamicPool<InventoryItemInfoPanel> generalItemInfoPanelPool;
        [SerializeField] private DynamicPool<InventoryItemInfoPanel>[] specialItemInfoPanelPools;

        private Dictionary<StorableScriptableObject, InventoryItemSlot> itemSlotsByStorable = new Dictionary<StorableScriptableObject, InventoryItemSlot>();
        private Dictionary<Type, DynamicPool<InventoryItemInfoPanel>> infoPanelsPerItemType = new Dictionary<Type, DynamicPool<InventoryItemInfoPanel>>();
        private List<InventoryItemSlot> activeItemSlots = new List<InventoryItemSlot>();
        private InventoryItemInfoPanel activeInfoPanel;
        private DynamicPool<InventoryItemInfoPanel> activeInfoPanelPool;
        private StorableScriptableObject activeInfoPanelItem;
        private InventoryItemSlot selectedSlot;

        private void Awake()
        {
            itemSlotsPool.Initialize();
            generalItemInfoPanelPool.Initialize();

            for (int i = 0; i < specialItemInfoPanelPools.Length; i++)
            {
                specialItemInfoPanelPools[i].Initialize();
                infoPanelsPerItemType.Add(specialItemInfoPanelPools[i].GetUnitInstance().ItemType, specialItemInfoPanelPools[i]);
            }
        }

        private void OnEnable()
        {
            if (inventory.IsInitialized)
                RefreshContent();
            else
                inventory.OnInitialized.AddListener(OnInventoryInitialized);

            inventory.OnItemAdded.AddListener(OnItemAdded);
            inventory.OnItemRemoved.AddListener(OnItemRemoved);
        }
        private void OnDisable()
        {
            inventory.OnInitialized.RemoveListener(OnInventoryInitialized);
            
            inventory.OnItemAdded.RemoveListener(OnItemAdded);
            inventory.OnItemRemoved.RemoveListener(OnItemRemoved);
        }

        private void OnInventoryInitialized()
        {
            inventory.OnInitialized.RemoveListener(OnInventoryInitialized);
            RefreshContent();
        }
        private void OnItemAdded(StorableScriptableObject item)
        {
            InventoryItemSlot slot = itemSlotsPool.RequestUnit(null, false);
            
            slot.Initialize(item);
            
            slot.OnClick += OnSlotClicked;

            activeItemSlots.Add(slot);
            
            slot.MyRT.AdaptRect(slotPlacements[activeItemSlots.Count - 1]);

            slot.gameObject.SetActive(true);

            itemSlotsByStorable.Add(item, slot);
        }
        private void OnItemRemoved(StorableScriptableObject item)
        {
            if (itemSlotsByStorable.TryGetValue(item, out InventoryItemSlot slot))
            {
                slot.OnClick += OnSlotClicked;

                itemSlotsPool.ReleaseUnit(slot);
                activeItemSlots.Remove(slot);

                if (item == activeInfoPanelItem)
                    CloseActiveInfoPanel();
            }
        }

        private void OnSlotClicked(InventoryItemSlot slot)
        {
            if (selectedSlot != null)
                selectedSlot.SetSelectedState(false);
            
            CloseActiveInfoPanel();

            StorableScriptableObject item = slot.Item;

            if (item == null)
                return;

            selectedSlot = slot;

            slot.SetSelectedState(true);

            DynamicPool<InventoryItemInfoPanel> infoPanelPool = generalItemInfoPanelPool;

            if (infoPanelsPerItemType.TryGetValue(item.GetType(), out var specialPool))
                infoPanelPool = specialPool;

            InventoryItemInfoPanel infoPanel = infoPanelPool.RequestUnit(null, false);
            
            activeInfoPanel = infoPanel;
            activeInfoPanelPool = infoPanelPool;
            activeInfoPanelItem = item;

            infoPanel.Initialize(inventory);
            infoPanel.SetData(item);

            infoPanel.gameObject.SetActive(true);
        }

        private void CloseActiveInfoPanel()
        {
            if (activeInfoPanel == null)
                return;

            activeInfoPanelPool.ReleaseUnit(activeInfoPanel);

            activeInfoPanel = null;
            activeInfoPanelPool = null;
            activeInfoPanelItem = null;
        }

        private void RefreshContent()
        {
            List<StorableScriptableObject> inventoryContent = inventory.Content;

            for (int i = 0; i < inventoryContent.Count; i++)
            {
                if (itemSlotsByStorable.TryGetValue(inventoryContent[i], out var slot))
                {
                    //This item already has a slot, we should update it in case the item was changed while the inventory was closed.
                    slot.RefreshData();
                }
                else
                {
                    //This item doesn't have a slot, we should add one.
                    OnItemAdded(inventoryContent[i]);
                }
            }
        }
    }
}