using System.Collections;
using System.Collections.Generic;
using TafraKit.ModularSystem;
using TafraKit.Weaponry;
using TafraKit.RPG;
using UnityEngine;
using UnityEngine.Events;
using TafraKit.Healthies;
using System;

namespace TafraKit.CharacterControls
{
    public class CharacterCombat : InternallyModularComponent<CharacterCombatModule>
    {
        #region Classes, Structs & Enums
        [Serializable]
        public class WeaponSlot
        {
            [Tooltip("Optional. Useful if you want to specify where a weapon should be equipped if you have multiple slots.")]
            public TafraString slotID;
            public Transform slotTransform;
            public List<Weapon> equippedWeapons;

            public WeaponSlot()
            {
                equippedWeapons = new List<Weapon>();
            }
        }
        #endregion

        #region Fields
        [Header("Slots")]
        [SerializeField] private WeaponSlot[] weaponSlots;
        [Tooltip("Can a single slot contain multiple weapons? If not, a weapon in a slot will be replaced by the newly equipped one.")]
        [SerializeField] private bool allowMultipleWeaponsInSlot;

        [Tooltip("The point of which the actions should use as target point.")]
        [SerializeField] private Transform pointOfInterest;
        [SerializeField] private RuntimeAnimatorController unarmedAnimatorController;
        [SerializeReferenceListContainer("modules", false, "Module", "Modules")]
        [SerializeField] private CharacterCombatModuleContainer modulesContainer;

        private TafraActor myActor;
        private CharacterAnimator characterAnimator;
        private IStatsContainerProvider statsContainerProvider;
        private StatsContainer statsContainer;
        private Dictionary<string, WeaponSlot> weaponSlotsById = new Dictionary<string, WeaponSlot>();
        private List<Weapon> equippedWeapons = new List<Weapon>();
        private bool grantWeaponAttack;
        private bool canWeaponAim = true;
        private bool canWeaponAttack = true;
        private Weapon mainWeapon;
        private ICharacterController characterController;
        private ControlReceiver weaponAttackDisablers;
        private ControlReceiver aggressiveStateContributers;
        private ControlReceiver potentialPoIAppliers;
        private Vector3 potentialPointOfInterest;
        private bool autoApplyPotentialPoI;
        private bool isAggressive;
        private UnityEvent<Weapon> onWeaponEquip = new UnityEvent<Weapon>();
        private UnityEvent<Weapon> onWeaponUnequip = new UnityEvent<Weapon>();
        private UnityEvent onWeaponRequestAttack = new UnityEvent();
        private UnityEvent<bool> onAggressiveStateChanged = new UnityEvent<bool>();
        #endregion

        #region Properties
        public ICharacterController CharacterController => characterController;
        public Weapon MainWeapon => mainWeapon;
        public GameObject GameObject => gameObject;
        public CharacterAnimator CharacterAnimator => characterAnimator;
        public WeaponSlot[] WeaponSlots => weaponSlots;
        public Vector3 PotentialPointOfInterest 
        {
            get => potentialPointOfInterest;
            set => potentialPointOfInterest = value;
        }
        /// <summary>
        /// The point of which the actions should use as target point.
        /// </summary>
        public Transform PointOfInterest => pointOfInterest;
        public bool IsAggressive => isAggressive;
        public UnityEvent<Weapon> OnWeaponEquip => onWeaponEquip;
        public UnityEvent<Weapon> OnWeaponUnequip => onWeaponUnequip;
        public UnityEvent OnWeaponRequestAttack => onWeaponRequestAttack;
        /// <summary>
        /// Fires whenever the object changes its aggressive state. Contains a bool parameter indicating whether it is aggressive or not.
        /// </summary>
        public UnityEvent<bool> OnAggressiveStateChanged => onAggressiveStateChanged;
        /// <summary>
        /// The player is being targeted by an enemy.
        /// </summary>
        public bool CanWeaponAim { get => canWeaponAim; set => canWeaponAim = value; }
        public bool CanWeaponAttack => canWeaponAttack;
        protected override List<CharacterCombatModule> InternalModules => modulesContainer.Modules;
        #endregion

        #region MonoBehaviour Messages
        protected override void Awake()
        {
            myActor = GetComponent<TafraActor>();
            characterAnimator = myActor.GetCachedComponent<CharacterAnimator>();
            statsContainerProvider = GetComponent<IStatsContainerProvider>();
            characterController = GetComponent<ICharacterController>();

            if(statsContainerProvider != null)
                statsContainer = statsContainerProvider.StatsContainer;
            
            pointOfInterest.SetParent(null);

            for (int i = 0; i < weaponSlots.Length; i++)
            {
                var slot = weaponSlots[i];
                weaponSlotsById.Add(slot.slotID.Value, slot);
            }

            base.Awake();

            for (int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];
               
                if (module == null)
                    continue;

                module.Initialize(this);
            }

            weaponAttackDisablers = new ControlReceiver(OnFirstAttackDisablerAdded, null, OnAllAttackDisablersCleared);
            aggressiveStateContributers = new ControlReceiver(OnFirstAggresssiveContributerAdded, null, OnAllAggresssiveContributersCleared);
            potentialPoIAppliers = new ControlReceiver(OnFirstPoIApplierAdded, null, OnAllPoIAppliersCleared);
        }
        protected override void Start()
        {
            base.Start();

            StartCoroutine(LateStart());
        }
        /// <summary>
        /// To give the equipments a chance to equip the weapon (it happens on Start).
        /// </summary>
        /// <returns></returns>
        IEnumerator LateStart()
        {
            yield return Yielders.EndOfFrame;

            //Apply the unarmed animator controller in case no weapon is equipped.
            if (mainWeapon == null && unarmedAnimatorController != null)
                characterAnimator.Animator.runtimeAnimatorController = unarmedAnimatorController;
        }
        protected override void Update()
        {
            base.Update();

            if (autoApplyPotentialPoI)
                pointOfInterest.position = potentialPointOfInterest;
        }
        #endregion

        #region Callbacks
        private void OnFirstAttackAllowerAdded()
        {
            for (int i = 0; i < equippedWeapons.Count; i++)
            {
                equippedWeapons[i].RemoveDeactivator("COMBAT_SUPRESSOR");
            }
        }
        private void OnAllAttackAllowersCleared()
        {
            for (int i = 0; i < equippedWeapons.Count; i++)
            {
                equippedWeapons[i].AddDeactivator("COMBAT_SUPRESSOR");
            }
        }
        private void OnFirstAggresssiveContributerAdded()
        {
            isAggressive = true;

            onAggressiveStateChanged?.Invoke(true);
        }
        private void OnAllAggresssiveContributersCleared()
        {
            isAggressive = false;

            onAggressiveStateChanged?.Invoke(false);
        }
        protected void OnFirstAttackDisablerAdded()
        { 
            canWeaponAttack = false;
        }
        protected void OnAllAttackDisablersCleared()
        {
            canWeaponAttack = true;
        }
        private void OnFirstPoIApplierAdded()
        {
            autoApplyPotentialPoI = true;
            pointOfInterest.position = potentialPointOfInterest;
        }
        private void OnAllPoIAppliersCleared()
        {
            autoApplyPotentialPoI = false;
        }
        #endregion

        /// <summary>
        /// Equip a weapon in one of the weapon slots.
        /// </summary>
        /// <param name="weapon">The weapon to equip (should already be spawned).</param>
        /// <param name="isMain">Whether this weapon should be considered the main one. Main weapons apply their animator override, among other things. If no main weapon exist, this will be considered the main anyway.</param>
        /// <param name="slotId">The ID of the slot to equip this weapon in. If empty or null, it will be equipped in the first slot with the least number of weapons.</param>
        /// <param name="equipment">If this weapon was spawned equipped through an equipment, it should be sent here.</param>
        public void EquipWeapon(Weapon weapon, bool isMain = false, string slotId = null, Equipment equipment = null)
        {
            WeaponSlot targetSlot;

            if (slotId != null && weaponSlotsById.TryGetValue(slotId, out var slot))
            {
                targetSlot = slot;
            }
            else
            {
                int lowestSlotWeaponsCount = 999999;
                int lowestSlotIndex = 999999;
                //If no specific slot was sent, or if that slot couldn't be found, then find a suitable slot.
                for (int i = 0; i < weaponSlots.Length; i++)
                {
                    var weaponSlot = weaponSlots[i];
                    int weaponsCount = weaponSlot.equippedWeapons.Count;

                    if(weaponsCount < lowestSlotWeaponsCount)
                    {
                        lowestSlotIndex = i;
                        lowestSlotWeaponsCount = weaponsCount;
                    }

                    //No need to look further, since it can't be lower than this.
                    if(weaponsCount == 0)
                        break;
                }

                targetSlot = weaponSlots[lowestSlotIndex];
            }

            if(targetSlot.equippedWeapons.Count > 0)
            {
                if(allowMultipleWeaponsInSlot)
                    targetSlot.equippedWeapons.Add(weapon);
                else
                {
                    //Unequip current weapon
                    UnequipWeapon(targetSlot.equippedWeapons[0]);

                    //Equip the new weapon.
                    targetSlot.equippedWeapons[0] = weapon;
                }
            }
            else
                targetSlot.equippedWeapons.Add(weapon);

            equippedWeapons.Add(weapon);

            weapon.transform.SetParent(targetSlot.slotTransform);
            weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            if(isMain || mainWeapon == null)
            {
                mainWeapon = weapon;
                isMain = true;
            }

            //Apply the weapon's animator controller
            if(isMain && weapon.AnimatorController != null)
            {
                characterAnimator.Animator.runtimeAnimatorController = weapon.AnimatorController;
                characterAnimator.Animator.Rebind();
            }

            weapon.Initialize(this, myActor, equipment);

            onWeaponEquip?.Invoke(weapon);
        }
        public void UnequipWeapon(Weapon weapon, string slotId = null)
        {
            WeaponSlot weaponSlot = null;

            //Look for the target weapon slot.
            if (!string.IsNullOrEmpty(slotId) && weaponSlotsById.TryGetValue(slotId, out var foundWeaponSlot))
            {
                weaponSlot = foundWeaponSlot;
            }
            else
            {
                for (int i = 0; i < weaponSlots.Length; i++)
                {
                    var ws = weaponSlots[i];
                    
                    if (ws.equippedWeapons.Contains(weapon))
                    {
                        weaponSlot = ws;
                        break;
                    }
                }
            }

            int weaponIndex = weaponSlot.equippedWeapons.IndexOf(weapon);

            if (weaponIndex != -1)
            {
                weaponSlot.equippedWeapons.RemoveAt(weaponIndex);

                weapon.AddDeactivator("unequipped");

                if (weapon == mainWeapon)
                {
                    equippedWeapons.Remove(weapon);

                    mainWeapon = null;

                    if (equippedWeapons.Count > 0)
                        mainWeapon = equippedWeapons[0];
                    else
                    {
                        //Apply the unarmed animator controller
                        if (unarmedAnimatorController != null)
                            characterAnimator.Animator.runtimeAnimatorController = unarmedAnimatorController;
                    }
                }

                onWeaponUnequip?.Invoke(weapon);
            }
        }
        public void AddAggressiveStateContributer(string contributerID)
        {
            aggressiveStateContributers.AddController(contributerID);
        }
        public void RemoveAggressiveStateContributer(string contributerID)
        {
            aggressiveStateContributers.RemoveController(contributerID);
        }
        public bool WeaponRequestAttack()
        {
            grantWeaponAttack = true;

            onWeaponRequestAttack?.Invoke();

            return grantWeaponAttack;
        }
        public void DenyWeaponAttackRequest()
        {
            grantWeaponAttack = false;
        }
        public void AddWeaponAttackDisabler(string disablerID)
        {
            weaponAttackDisablers.AddController(disablerID);
        }
        public void RemoveWeaponAttackDisabler(string disablerID)
        {
            weaponAttackDisablers.RemoveController(disablerID);
        }
        /// <summary>
        /// Take the value of the potential point of interest, and apply it on the actual point of interest.
        /// </summary>
        public void ApplyPotentialPointOfInterest()
        {
            pointOfInterest.position = potentialPointOfInterest;
        }
        /// <summary>
        /// As long as this applier is still present, the potential point of interest will keep getting applied as the actual point of interest.
        /// </summary>
        /// <param name="applierID"></param>
        public void AddPotentialPoIContinuousApplier(string applierID)
        {
            potentialPoIAppliers.AddController(applierID);
        }
        public void RemovePotentialPoIContinuousApplier(string applierID)
        {
            potentialPoIAppliers.RemoveController(applierID);
        }
        protected override void OnPause()
        {
            base.OnPause();

            for (int i = 0; i < equippedWeapons.Count; i++)
            {
                equippedWeapons[i].AddDeactivator("combat_paused");
            }
        }
        protected override void OnResume()
        {
            base.OnResume();

            for(int i = 0; i < equippedWeapons.Count; i++)
            {
                equippedWeapons[i].RemoveDeactivator("combat_paused");
            }
        }
    }
}