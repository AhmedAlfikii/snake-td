using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.RPG
{
    public class Inventory : MonoBehaviour
    {
        [Tooltip("The id of the inventory to be used for saving/loading operations.")]
        [SerializeField] private string id;
        [Tooltip("If Infinite Slots is turned off, how many slots should the inventory contain?")]
        [SerializeField] private int slotsCount;
        [SerializeField] private bool infiniteSlots = true;

        [Header("Loading")]
        [Tooltip("(RECOMMENDED: true) Should we load the items from the resources folder? otherwise you should provide a full list of all the storable items in the game.")]
        [SerializeField] private bool loadFromResources = true;
        [Tooltip("The path of the folder inside the resources folder that will contain all the loadable storables.")]
        [SerializeField] private string pathInResources = "Storables";
        [Tooltip("(OPTIONAL) only fill this if the \"Load From Resources\" property is set to false.")]
        [SerializeField] private StorableScriptableObject[] allPossibleStorables;

        [Header("Additional References")]
        [SerializeField] private CharacterEquipment equipmentsHolder;

        public UnityEvent OnInitialized = new UnityEvent();
        public UnityEvent<StorableScriptableObject> OnItemAdded = new  UnityEvent<StorableScriptableObject>();
        public UnityEvent<StorableScriptableObject> OnItemRemoved = new UnityEvent<StorableScriptableObject>();
        /// <summary>
        /// Fires whenever an item gets added to the inventory. Contains the original scriptable object of the added item, and the added quantity.
        /// </summary>
        public UnityEvent<StorableScriptableObject, int> OnItemQuantityAdded = new UnityEvent<StorableScriptableObject, int>();

        private List<StorableScriptableObject> content = new List<StorableScriptableObject>();
        private Dictionary<string, List<StorableScriptableObject>> contentByOriginalID = new Dictionary<string, List<StorableScriptableObject>>();
        private StringBuilder contentSB = new StringBuilder();
        private bool isInitialized;

        public bool IsInitialized => isInitialized;
        public List<StorableScriptableObject> Content => content;
        public CharacterEquipment EquipmentsHolder => equipmentsHolder;

        private void Awake()
        {
            if (loadFromResources)
            {
                LoadAllPossibleStorables();
            }
            Load();

            isInitialized = true;
            OnInitialized?.Invoke();
        }

        /// <summary>
        /// Saves the inventory content. Only needs to be saved when a slot's state is changed (occupant added/removed) no need to update it if the occupant storable's properties were changed (like quantity).
        /// </summary>
        private void Save()
        {
            contentSB.Clear();
            for(int i = 0; i < content.Count; i++)
            {
                contentSB.Append(content[i].InstancableSO.GetSOInstanceID());

                if (i != content.Count - 1)
                    contentSB.Append(',');
            }

            PlayerPrefs.SetString($"INVENTORY_{id}_CONTENT", contentSB.ToString());
        }
        private void Load()
        {
            string savedContentString = PlayerPrefs.GetString($"INVENTORY_{id}_CONTENT");
            
            if(string.IsNullOrEmpty(savedContentString))
                return;

            string[] contentInstanceIDs = savedContentString.Split(',');

            for(int i = 0; i < contentInstanceIDs.Length; i++)
            {
                string instanceID = contentInstanceIDs[i];
                int lastUnderscore = instanceID.LastIndexOf('_');
                string originalID = instanceID.Substring(0, lastUnderscore);
                int instanceNumber = int.Parse(instanceID.Substring(lastUnderscore + 1));

                StorableScriptableObject originalStorable = GetOriginalStorable(originalID);

                if(originalStorable == null)
                {
                    if (loadFromResources)
                        TafraDebugger.Log($"Inventory ({id})", $"Couldn't find the storable ({originalID}) in the Resources/{pathInResources} folder.", TafraDebugger.LogType.Error);
                    else
                        TafraDebugger.Log($"Inventory ({id})", $"Couldn't find the storable ({originalID}) in the provided possible storables list.", TafraDebugger.LogType.Error);
                    continue;
                }

                StorableScriptableObject instance = originalStorable.InstancableSO.CreateInstance(instanceNumber) as StorableScriptableObject;
                
                instance.Load();

                AddInstanceToContent(instance);
            }
        }

        public StorableScriptableObject GetOriginalStorable(string originalID)
        {
            if(loadFromResources)
                return Resources.Load<StorableScriptableObject>($"{pathInResources}/{originalID}");
            else
            {
                for(int i = 0; i < allPossibleStorables.Length; i++)
                {
                    if(allPossibleStorables[i].name == originalID)
                        return allPossibleStorables[i];
                }
            }

            return null;
        }

        private void LoadAllPossibleStorables()
        {
            allPossibleStorables = Resources.LoadAll<StorableScriptableObject>($"{pathInResources}");
        }

        private void AddInstanceToContent(StorableScriptableObject storableInstance)
        {
            content.Add(storableInstance);

            if(contentByOriginalID.TryGetValue(storableInstance.ID, out var instances))
                instances.Add(storableInstance);
            else
                contentByOriginalID.Add(storableInstance.ID, new List<StorableScriptableObject> { storableInstance });    //TODO: pool lists.
        }

        public void AddItem(StorableScriptableObject item)
        {
            bool hasACopyInInventory = contentByOriginalID.ContainsKey(item.ID);
            
            //If all the inventory slots are occupied, and the inventory doesn't contain this item, or it does contain it but it's not stackable, then the inventory can't store the item.
            if (!infiniteSlots && content.Count >= slotsCount && (!hasACopyInInventory || !item.IsStackable))
            {
                TafraDebugger.Log($"Inventory ({id})", $"Can't store item ({item.ID}) because the inventroy is full.", TafraDebugger.LogType.Info);
                return;
            }

            //Increase the stack of the item if it is in the inventory and is stackable.
            if(hasACopyInInventory && item.IsStackable)
            {
                StorableScriptableObject itemInInventory = contentByOriginalID[item.ID][0];
                itemInInventory.Quantity += item.Quantity;
                itemInInventory.Save();
            }
            //Add a new item instance in case the item isn't: already in the inventory and is stackable.
            else
            {
                //If the storable is already an instance, then we can simply take it in the inventory, otherwise we should create an instance out of it.
                StorableScriptableObject storableInstance = item.InstancableSO.GetOrCreateInstance() as StorableScriptableObject;

                //Save this instance in case it has any unsaved properties.
                storableInstance.Save();

                AddInstanceToContent(storableInstance);

                Save();

                OnItemAdded?.Invoke(storableInstance);
            }

            OnItemQuantityAdded?.Invoke(item.OriginalScriptableObject as StorableScriptableObject, item.Quantity);
        }
        public void RemoveItem(StorableScriptableObject item)
        {
            if (!content.Contains(item))
                TafraDebugger.Log($"Inventory ({id})", "Item is not in the inventory, can't remove it", TafraDebugger.LogType.Error);

            content.Remove(item);

            //Remove the item from the content by ID dictionary.
            if (contentByOriginalID.TryGetValue(item.ID, out var storables) && storables != null)
            { 
                int itemIndex = storables.IndexOf(item);
                if (itemIndex >= 0)
                    storables.RemoveAt(itemIndex);

                contentByOriginalID.Remove(item.ID);
            }
            
            Save();
         
            OnItemRemoved?.Invoke(item);
        }

        public bool TryGetItemByOriginalID(string id, out StorableScriptableObject storable)
        {
            storable = null;

            if(contentByOriginalID.TryGetValue(id, out var storables) && storables != null)
            {
                //TODO: this shouldn't just output the first storable. Handle the other case.
                storable = storables[0];
            }

            return storable != null;
        }
        public StorableScriptableObject[] GetAllPossibleStorables()
        {
            return allPossibleStorables;
        }

        public void ResetPlayerInventoryContent()
        {
            PlayerPrefs.SetString($"INVENTORY_{id}_CONTENT", string.Empty);
        }

        public string GetPlayerInventoryContent()
        {
            return PlayerPrefs.GetString($"INVENTORY_{id}_CONTENT");
        }
    }
}