using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [Serializable]
    public class AbilitiesContainer
    {
        [SerializeField] private bool enableSaveAndLoad;
        [SerializeField] private string id;
        [SerializeField] private string resourcesPath = "Abilities";
        [Tooltip("Slots that abilities can be added to for reference. Abilities don't need to be added to slots.")]
        [SerializeField] private List<TafraString> slots;

        [Header("Defaults")]
        [Tooltip("Abilities that will be equipped once this container is initialized.")]
        [SerializeField] private List<Ability> defaultAbilities;

        [NonSerialized] private CharacterAbilities characterAbilities;
        [NonSerialized] private TafraActor actor;
        [SerializeField] private List<Ability> equippedAbilities = new List<Ability>();
        [NonSerialized] private List<Ability> saveableEquippedAbilities = new List<Ability>();
        [NonSerialized] private Dictionary<string, List<Ability>> equippedAbilitiesByOriginalId = new Dictionary<string, List<Ability>>();
        [NonSerialized] private StringBuilder saveSB = new StringBuilder();
        [NonSerialized] private bool isInitialized;
        [NonSerialized] private Dictionary<string, Ability> abilityBySlot = new Dictionary<string, Ability>();
        [NonSerialized] private Dictionary<Ability, string> slotByAbility = new Dictionary<Ability, string>();

        public List<Ability> EquippedAbilities => equippedAbilities;

        public void Initialize(CharacterAbilities characterAbilities, TafraActor actor)
        {
            this.characterAbilities = characterAbilities;
            this.actor = actor;

            //If it's already initialized, this means that this is most likely in an external container and there's no need to re-equip the abilities, we just need to activate them.
            if (isInitialized)
            {
                InitializeEquippedAbilities();
                ActivateEquippedAbilities();
                return;
            }

            isInitialized = true;

            //Build the slots dictionary.
            for (int i = 0; i < slots.Count; i++)
            {
                abilityBySlot.Add(slots[i].Value, null);
            }

            bool appliedSavedData = false;
            if (enableSaveAndLoad)
                appliedSavedData = Load();

            if (appliedSavedData)
                return;

            for (int i = 0; i < defaultAbilities.Count; i++)
            {
                var ability = defaultAbilities[i];

                if (ability == null)
                    continue;

                Equip(ability);
            }
        }
        public Ability Equip(Ability ability, string slot = null, bool save = true, bool isFromLoad = true, Action<Ability> onBeforeActivate = null)
        {
            Ability instance = ability.GetOrCreateInstance();

            if (!string.IsNullOrEmpty(slot) && abilityBySlot.TryGetValue(slot, out var abilityInSlot))
            {
                //If an ability is already in the target slot, then unequip it.
                if (abilityInSlot != null)
                    Unequip(abilityInSlot);

                abilityBySlot[slot] = instance;

                slotByAbility.Add(instance, slot);
            }

            equippedAbilities.Add(instance);
            
            if(equippedAbilitiesByOriginalId.TryGetValue(instance.OriginalID, out var abilities))
                abilities.Add(instance);
            else
                equippedAbilitiesByOriginalId.Add(instance.OriginalID, new List<Ability>() { instance });

            instance.InitializeInstance(characterAbilities, actor);

            if (!isFromLoad)
                instance.EquippedFirstTime();
            else
                instance.EquippedAfterLoad();

            onBeforeActivate?.Invoke(instance);

            instance.ApplySystemObjectInitializers();

            instance.Activate();

            if (save && enableSaveAndLoad)
            {
                saveSB.Append(instance.OriginalScriptableObject.name).Append("_").Append(instance.InstanceNumber).Append('-').Append(string.IsNullOrEmpty(slot) ? '0' : slot).Append(',');
                Save(false);
                saveableEquippedAbilities.Add(instance);
            }

            return instance;
        }
        public bool Unequip(Ability ability)
        {
            if(equippedAbilitiesByOriginalId.TryGetValue(ability.OriginalID, out var equippedAbilities))
            {
                int abilityIndex = equippedAbilities.IndexOf(ability);
                
                if(abilityIndex == -1)
                    return false;

                Ability equippedAbility = equippedAbilities[abilityIndex];

                equippedAbility.Deactivate();
                equippedAbility.Unequipped();

                //If the unequipped ability is in a slot, remove it from it.
                if (slotByAbility.TryGetValue(equippedAbility, out var slot))
                {
                    abilityBySlot[slot] = null;
                    slotByAbility.Remove(equippedAbility);
                }

                equippedAbilities.Remove(equippedAbility);
                equippedAbilitiesByOriginalId.Remove(equippedAbility.OriginalID);
                this.equippedAbilities.Remove(equippedAbility);

                if (saveableEquippedAbilities.Remove(equippedAbility))
                    Save(true);

                UnityEngine.Object.Destroy(equippedAbility);

                return true;
            }

            return false;
        }
        public bool IsEquipped(Ability ability)
        {
            return equippedAbilitiesByOriginalId.ContainsKey(ability.OriginalID);
        }
        public void InitializeEquippedAbilities()
        {
            for (int i = 0; i < equippedAbilities.Count; i++)
            {
                var ability = equippedAbilities[i];
                ability.InitializeInstance(characterAbilities, actor);
            }
        }
        public void ActivateEquippedAbilities()
        {
            for (int i = 0; i < equippedAbilities.Count; i++)
            {
                var ability = equippedAbilities[i];
                ability.Activate();
            }
        }
        public void DeactivateEquippedAbilities()
        {
            for (int i = 0; i < equippedAbilities.Count; i++)
            {
                var ability = equippedAbilities[i];
                ability.Deactivate();
            }
        }
        public void PauseEquippedAbilities()
        {
            for (int i = 0; i < equippedAbilities.Count; i++)
            {
                var ability = equippedAbilities[i];
                ability.Pause();
            }
        }
        public void ResumeEquippedAbilities()
        {
            for (int i = 0; i < equippedAbilities.Count; i++)
            {
                var ability = equippedAbilities[i];
                ability.Resume();
            }
        }
        public bool TryGetEquippedAbility(Ability ability, out Ability equippedInstance)
        {
            if(equippedAbilitiesByOriginalId.TryGetValue(ability.OriginalID, out var abilities))
            {
                int abilityIndex = abilities.IndexOf(ability);

                if(abilityIndex == -1)
                {
                    equippedInstance = null;
                    return false;
                }

                equippedInstance = abilities[abilityIndex];
                return true;
            }
            else
            {
                equippedInstance = null;
                return false;
            }
        }
        public bool TryGetAbilityInSlot(string slot, out Ability ability)
        { 
            abilityBySlot.TryGetValue(slot, out ability);

            return ability != null;
        }

        public void Tick()
        {
            for(int i = 0; i < equippedAbilities.Count; i++)
            {
                var ability = equippedAbilities[i];

                if (ability.IsActive && !ability.IsPaused)
                    ability.Tick();
            }
        }

        /// <summary>
        /// Load the and equip the abilities, returns true if saved data was found.
        /// </summary>
        /// <returns>Whether or not saved data exist.</returns>
        private bool Load()
        {
            saveSB.Clear();

            string savedData = PlayerPrefs.GetString($"{id}_EQUIPPED_ABILITIES");

            if (string.IsNullOrEmpty(savedData))
                return false;

            saveSB.Append(savedData);

            string[] abilities = savedData.Split(',', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < abilities.Length; i++)
            {
                var abilityNameID = abilities[i];

                int indexOfIdSeparator = abilityNameID.LastIndexOf('_');
                int indexOfSlotSeparator = abilityNameID.LastIndexOf('-');

                string abilityName = abilityNameID.Substring(0, indexOfIdSeparator);

                int instanceNumberStartIndex = indexOfIdSeparator + 1;
                string instanceNumber = abilityNameID.Substring(instanceNumberStartIndex, indexOfSlotSeparator - instanceNumberStartIndex);

                string slot = abilityNameID.Substring(indexOfSlotSeparator + 1);

                Ability ability = Resources.Load<Ability>(resourcesPath + "/" + abilityName);

                Ability instance = ability.InstanceableSO.CreateInstance(int.Parse(instanceNumber)) as Ability;

                Equip(instance, string.IsNullOrEmpty(slot) ? null : slot, false, true);
                saveableEquippedAbilities.Add(instance);
            }

            return true;
        }
        private void Save(bool rebuildString)
        {
            if (rebuildString)
            {
                saveSB.Clear();

                for (int i = 0; i < saveableEquippedAbilities.Count; i++)
                {
                    var ability = saveableEquippedAbilities[i];
                    string slot;
                    slotByAbility.TryGetValue(ability, out slot);

                    saveSB.Append(ability.OriginalScriptableObject.name).Append("_").Append(ability.InstanceNumber).Append('-').Append(string.IsNullOrEmpty(slot) ? '0' : slot).Append(',');
                }
            }

            PlayerPrefs.SetString($"{id}_EQUIPPED_ABILITIES", saveSB.ToString());
        }
    }
}