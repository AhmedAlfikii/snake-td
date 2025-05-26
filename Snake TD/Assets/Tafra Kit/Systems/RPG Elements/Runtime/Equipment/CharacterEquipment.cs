using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TafraKit;
using System.Text;
using TafraKit.CharacterCustomization;
using TafraKit.Weaponry;
using TafraKit.CharacterControls;
using TafraKit.ModularSystem;

namespace TafraKit.RPG
{
    public class CharacterEquipment : InternallyModularComponent<CharacterEquipmentModule>
    {
        [SerializeField] private string id;
        /// <summary>
        /// Only equipment with a category equal to one of the slots in this list can be equipped.
        /// </summary>
        [SerializeField] private EquipmentSlot[] slots;
        /// <summary>
        /// The lsit of equipment the character will be equipping when the game first launches.
        /// </summary>
        [SerializeField] private List<Equipment> defaultEquipment = new List<Equipment>();
        /// <summary>
        /// The list of equipment that will be equipped if no other equipment is equipped in its slot.
        /// </summary>
        [SerializeField] private List<Equipment> mandatoryEquipment = new List<Equipment>();
        /// <summary>
        /// The name of the slot in then slots list that will contain the weapons. Any equipment added to this slot will be sent to the character combat component as a weapon.
        /// </summary>
        [SerializeField] private TafraString weaponSlot;

        [Header("Loading")]
        [Tooltip("(RECOMMENDED: true) Should we load the items from the resources folder? otherwise you should provide a full list of all the equipment items in the game.")]
        [SerializeField] private bool loadFromResources = true;
        [Tooltip("The path of the folder inside the resources folder that will contain all the loadable equipment.")]
        [SerializeField] private string pathInResources = "Storables";
        [Tooltip("(OPTIONAL) only fill this if the \"Load From Resources\" property is set to false.")]
        [SerializeField] private Equipment[] allPossibleEquipment;

        [Header("LOD")]
        [Tooltip("The equipment's customizations level of details that will be used. If an equipment doens't contian this level, the closest one will be used.")]
        [SerializeField] private int levelOfDetails = 0;

        [Header("Modules")]
        [SerializeReferenceListContainer("modules", false, "Module", "Modules")]
        [SerializeField] private CharacterEquipmentModuleContainer modulesContainer;

        [HideInInspector] private UnityEvent onInitialized = new UnityEvent();
        [HideInInspector] private UnityEvent onEquipmentChange = new UnityEvent();
        [HideInInspector] private UnityEvent<Equipment> onEquip = new UnityEvent<Equipment>();
        [HideInInspector] private UnityEvent<Equipment> onUnequip = new UnityEvent<Equipment>();
        [HideInInspector] private UnityEvent<Weapon, Equipment> onWeaponEquipped = new UnityEvent<Weapon, Equipment>();
        [HideInInspector] private UnityEvent<Weapon> onWeaponUnequipped = new UnityEvent<Weapon>();

        private bool isInitialized;
        private Dictionary<string, EquipmentSlot> slotByCategory = new Dictionary<string, EquipmentSlot>();
        private Dictionary<string, Equipment> mandatoryEquipmentBySlot = new Dictionary<string, Equipment>();
        private StringBuilder equipmentSB = new StringBuilder();
        private List<Equipment> equippedEquipment = new List<Equipment>();
        private IStatsContainerProvider statsContainerProvider;
        private StatsContainer statsContainer;
        private CustomizableCharacter customizableCharacter;
        private CharacterCombat characterCombat;

        public UnityEvent OnInitialized => onInitialized;
        public UnityEvent OnEquipmentChange => onEquipmentChange;
        public UnityEvent<Equipment> OnEquip => onEquip;
        public UnityEvent<Equipment> OnUnequip => onUnequip;
        public UnityEvent<Weapon, Equipment> OnWeaponEquipped => onWeaponEquipped;
        public UnityEvent<Weapon> OnWeaponUnequipped => onWeaponUnequipped;

        public bool IsInitialized => isInitialized;
        public int LevelOfDetails => levelOfDetails;

        protected override List<CharacterEquipmentModule> InternalModules => modulesContainer.Modules;

        protected override void Awake()
        {
            base.Awake();

            statsContainerProvider = GetComponent<IStatsContainerProvider>();
            customizableCharacter = GetComponent<CustomizableCharacter>();
            characterCombat = GetComponent<CharacterCombat>();

            if(statsContainerProvider != null)
                statsContainer = statsContainerProvider.StatsContainer;

            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];

                for (int j = 0; j < slots[i].SlotsCount; j++)
                {
                    slot.CurrentEquipment.Add(null);
                    slot.CurrentSaveableEquipment.Add(null);
                }

                slotByCategory.Add(slots[i].Category, slot);
                mandatoryEquipmentBySlot.Add(slots[i].Category, null);
            }

            //Cache mandatory equipment
            for(int i = 0; i < mandatoryEquipment.Count; i++)
            {
                Equipment mandatoryEquip = mandatoryEquipment[i];
                mandatoryEquipmentBySlot[mandatoryEquip.Category] = mandatoryEquip;
            }

            for (int i = 0; i < modulesCount; i++)
            {
                allModules[i].Initialize(this);
            }
        }
        protected override void Start()
        {
            base.Start();

            Load();
            EquipApplicableMandatoryEquipment();

            isInitialized = true;
            onInitialized?.Invoke();
        }

        private void Save()
        {
            if(string.IsNullOrEmpty(id)) 
                return;

            equipmentSB.Clear();

            for(int slotGroupIndex = 0; slotGroupIndex < slots.Length; slotGroupIndex++)
            {
                var slot = slots[slotGroupIndex];
                int lastSlotIndex = slot.SlotsCount - 1;

                for(int slotIndex = 0; slotIndex < slot.CurrentSaveableEquipment.Count; slotIndex++)
                {
                    Equipment eq = slot.CurrentSaveableEquipment[slotIndex];

                    if(eq != null)
                        equipmentSB.Append(eq.InstancableSO.GetSOInstanceID());

                    //Add a column only if this isn't the last slot of the last slot group.
                    if(slotIndex != lastSlotIndex || slotGroupIndex < slots.Length - 1)
                        equipmentSB.Append(',');
                }
            }

            PlayerPrefs.SetString($"EQUIPMENT_{id}_CONTENT", equipmentSB.ToString());
        }
        private void Load()
        {
            if(String.IsNullOrEmpty(id)) 
                return;
            
            StringBuilder defaultEquipmentSB = new StringBuilder();

            for (int i = 0; i < defaultEquipment.Count; i++)
            {
                defaultEquipmentSB.Append(defaultEquipment[i].ID);
                defaultEquipmentSB.Append("_0");   //To mimic an instance, since extracting elements expects them to be instances.

                if (i != defaultEquipment.Count - 1)
                    defaultEquipmentSB.Append(',');
            }

            string savedEquipmentString = PlayerPrefs.GetString($"EQUIPMENT_{id}_CONTENT", defaultEquipmentSB.ToString());

            if (string.IsNullOrEmpty(savedEquipmentString))
                return;

            string[] contentInstanceIDs = savedEquipmentString.Split(',');

            for (int i = 0; i < contentInstanceIDs.Length; i++)
            {
                string instanceID = contentInstanceIDs[i];

                if(string.IsNullOrEmpty(instanceID))
                    continue;

                int lastUnderscore = instanceID.LastIndexOf('_');
                string originalID = instanceID.Substring(0, lastUnderscore);
                int instanceNumber = int.Parse(instanceID.Substring(lastUnderscore + 1));

                Equipment originalEquipment = GetOriginalEquipment(originalID);

                if (originalEquipment == null)
                {
                    if (loadFromResources)
                        TafraDebugger.Log($"Character Equipment ({id})", $"Couldn't find the equipment ({originalID}) in the Resources/{pathInResources} folder.", TafraDebugger.LogType.Error);
                    else
                        TafraDebugger.Log($"Character Equipment ({id})", $"Couldn't find the equipment ({originalID}) in the provided possible equipment list.", TafraDebugger.LogType.Error);
                    continue;
                }

                Equipment instance = originalEquipment.InstancableSO.CreateInstance(instanceNumber) as Equipment;

                instance.Load();

                Equip(instance, -1, true);
            }
        }
        private void EquipApplicableMandatoryEquipment()
        {
            for(int i = 0; i < mandatoryEquipment.Count; i++)
            {
                Equipment mandatoryEquip = mandatoryEquipment[i];

                //If no equipment is already equipped in this mandatory equipment's slot, then equip it.
                if(!slotByCategory[mandatoryEquip.Category].HasAnyEquipment)
                    Equip(mandatoryEquip, -1, false);
            }
        }
        private Equipment GetOriginalEquipment(string originalID)
        {
            if (loadFromResources)
                return Resources.Load<Equipment>($"{pathInResources}/{originalID}");
            else
            {
                for (int i = 0; i < allPossibleEquipment.Length; i++)
                {
                    if (allPossibleEquipment[i].name == originalID)
                        return allPossibleEquipment[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equipmentPiece"></param>
        /// <param name="slotIndex">If this category contains more than 1 slot, then which slot should this item be placed in? If left with a negative value, it will be placed in the first empty slot, or will replace the last slot if no empty slots were found.</param>
        /// <param name="save"></param>
        public Equipment Equip(Equipment equipmentPiece, int slotIndex = -1, bool save = true)
        {
            Equipment equipmentInstance = equipmentPiece.InstancableSO.GetOrCreateInstance() as Equipment;

            //If there's no slot for this item's category, then don't do anything.
            if(!slotByCategory.TryGetValue(equipmentInstance.Category, out var slot))
                return null;

            //If that instance of the item is already equipped, then return it.
            if(equippedEquipment.Contains(equipmentInstance))
            {
                return equipmentInstance;
            }

            int slotsCount = slot.SlotsCount;
            int occupiedSlots = 0;
            int firstUnoccupiedSlot = -1;

            for (int i = 0; i < slotsCount; i++)
            {
                if (slot.CurrentEquipment[i] != null)
                {
                    occupiedSlots++;
                }
                else if (firstUnoccupiedSlot < 0)
                    firstUnoccupiedSlot = i;
            }

            int targetSlotIndex;

            //If all the slots are occupied, then replace the item in the last slot.
            if (firstUnoccupiedSlot < 0)
            {
                targetSlotIndex = slot.SlotsCount - 1;
                Unequip(equipmentInstance.Category, slot.CurrentEquipment[targetSlotIndex], false, true);
            }
            else
                targetSlotIndex = firstUnoccupiedSlot;

            equippedEquipment.Add(equipmentInstance);

            slotByCategory[equipmentInstance.Category].AddEquipmentInSlot(targetSlotIndex, equipmentInstance, save);

            if(save)
                Save();

            //Apply equipment stats
            if(statsContainer != null)
                statsContainer.AddStatValues(equipmentInstance.StatValues);

            //Skin and spawnable game object game object.
            if(customizableCharacter != null && equipmentInstance.HasSkin(levelOfDetails))
            {
                GameObject equipmentInstanceGO = customizableCharacter.ApplyMeshCustomization(equipmentInstance.LoadSkin(levelOfDetails));
                equipmentInstance.EquipmentInstanceGO = equipmentInstanceGO;
            }
            else if(equipmentInstance.HasGO(levelOfDetails))
            {
                GameObject equipmentInstanceGO = Instantiate(equipmentInstance.LoadGO(levelOfDetails), transform);

                //Remove the clone text.
                string eqName = equipmentInstanceGO.name;
                equipmentInstanceGO.name = eqName.Substring(0, eqName.Length - 7);

                equipmentInstance.EquipmentInstanceGO = equipmentInstanceGO;
            }

            //If this is a weapon, spawn it
            if(equipmentInstance.EquipmentInstanceGO && equipmentInstance.Category == weaponSlot.Value)
            {
                Weapon weapon = equipmentInstance.EquipmentInstanceGO.GetComponent<Weapon>();

                if(weapon != null)
                {
                    characterCombat.EquipWeapon(weapon, equipment: equipmentInstance);

                    onWeaponEquipped?.Invoke(weapon, equipmentInstance);
                }
            }

            onEquipmentChange.Invoke();
            onEquip.Invoke(equipmentInstance);

            return equipmentInstance;
        }
        /// <summary>
        /// Will unequip the desired item from the slot. If no item is given, the last item in the slots group will be unequipped.
        /// </summary>
        /// <param name="slotCategory"></param>
        /// <param name="equipmentToUnequip">Unequip this item from the slots, if null, the last item in the slots will be unequipped.</param>
        /// <param name="save"></param>
        /// <param name="supressMandatoryEquipment"></param>
        public void Unequip(string slotCategory, Equipment equipmentToUnequip, bool save = true, bool supressMandatoryEquipment = false)
        {
            //If there's no slot with that category, or if there is but it has no item, then don't do anything.
            if(!slotByCategory.TryGetValue(slotCategory, out var slot) || !slot.HasAnyEquipment)
                return;

            Equipment equippedItem = null;
            int slotIndex = -1;
            int lastEquippedSlotIndex = -1;
            for (int i = 0; i < slot.CurrentEquipment.Count; i++)
            {
                var eq = slot.CurrentEquipment[i];

                if (eq != null)
                    lastEquippedSlotIndex = i;

                if (eq == equipmentToUnequip)
                {
                    equippedItem = eq;
                    slotIndex = i;
                    break;
                }
            }

            //Item is not equipped.
            if (equippedItem == null && lastEquippedSlotIndex < 0)
                return;

            if(equippedItem == null)
            {
                equippedItem = slot.CurrentEquipment[lastEquippedSlotIndex];
                slotIndex = lastEquippedSlotIndex;
            }

            equippedEquipment.Remove(equippedItem);

            slot.RemoveEquipmentFromSlot(slotIndex);

            if(save)
                Save();

            //Remove stat values
            if(statsContainer != null)
                statsContainer.RemoveStatValues(equippedItem.StatValues);

            //If this is a weapon, remove it from the Character Combat component
            if(equippedItem.EquipmentInstanceGO && equippedItem.Category == weaponSlot.Value)
            {
                Weapon weapon = equippedItem.EquipmentInstanceGO.GetComponent<Weapon>();

                if(weapon != null)
                {
                    characterCombat.UnequipWeapon(weapon);

                    onWeaponUnequipped?.Invoke(weapon);
                }
            }

            //Skin and spawnable game object.
            if(customizableCharacter != null && equippedItem.HasSkin(levelOfDetails))
            {
                customizableCharacter.RemoveMeshCustomization(equippedItem.LoadSkin(levelOfDetails).Category);
                equippedItem.ReleaseLoadedObjects();
            }
            else if(equippedItem.HasGO(levelOfDetails) && equippedItem.EquipmentInstanceGO)
            {
                Destroy(equippedItem.EquipmentInstanceGO);
                equippedItem.ReleaseLoadedObjects();
            }

            //If there's a mandatory equipment for this slot, equip it if no equipment exist in this slot group.
            if(!slot.HasAnyEquipment && !supressMandatoryEquipment && mandatoryEquipmentBySlot.TryGetValue(slotCategory, out Equipment mandatoryEquip) && mandatoryEquip != null)
                Equip(mandatoryEquip);

            onEquipmentChange.Invoke();
            onUnequip.Invoke(equippedItem);
        }

        public List<Equipment> GetEquipmentInSlot(string slot)
        {
            if (slotByCategory.ContainsKey(slot))
                return slotByCategory[slot].CurrentEquipment;
            else
                return null;
        }
        public List<Equipment> GetEquipment()
        {
            return equippedEquipment;
        }
        public bool IsEquipmentMandatory(Equipment equipment)
        {
            if(mandatoryEquipmentBySlot.TryGetValue(equipment.Category, out Equipment equippedItem) && equippedItem != null)
                return equippedItem.OriginalID == equipment.OriginalID;

            return false;
        }
    }
}