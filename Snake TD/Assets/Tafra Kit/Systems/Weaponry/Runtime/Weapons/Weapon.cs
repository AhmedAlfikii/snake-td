using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.ContentManagement;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit.Weaponry
{
    public abstract class Weapon : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] protected List<WeaponActionCategory> actionCategories;

        [Header("Animation")]
        [Tooltip("The animator controller that will be applied to the holder whenever this weapon is equipped.")]
        [SerializeField] protected RuntimeAnimatorController animatorController;
        [SerializeField] protected string aggressiveStateAnimatorBool = "Is Aggressive";

        [Header("Inputs")]
        [SerializeField] protected TafraAsset<ScriptableFloat> damage;
        [SerializeField] protected TafraAsset<ScriptableFloat> attackSpeed;
        [SerializeField] protected TafraAsset<ScriptableFloat> critChance;
        [SerializeField] protected TafraAsset<ScriptableFloat> critDamage;
        #endregion

        #region Private Fields
        /// <summary>
        /// Can the weapon act or react to player input?
        /// </summary>
        protected bool isActive = true;
        /// <summary>
        /// The actor that is holding this weapon.
        /// </summary>
        protected TafraActor holderActor;
        protected CharacterCombat characterCombat;
        protected CharacterMotor characterMotor;
        protected BodyRotationModule bodyRotationModule;
        protected CharacterAnimator characterAnimator;
        protected Animator animator;
        /// <summary>
        /// If at least one controller is in this list, then the weapon will be deactivated.
        /// </summary>
        protected ControlReceiver deactivators;
        /// <summary>
        /// If this weapon was spawned through an equipment, it'll be stored here.
        /// </summary>
        protected Equipment equipment;
        protected int aggresiveStateAnimatorBoolHash = -1;
        protected ScriptableFloat damageSF;
        protected ScriptableFloat attackSpeedSF;
        protected ScriptableFloat critChanceSF;
        protected ScriptableFloat critDamageSF;
        protected int curDamage;
        protected float curAttackCooldown;
        protected float curCritChance;
        protected float curCritDamage;
        #endregion

        #region Public Properties
        /// <summary>
        /// Whoever holds the weapon and is in control of.
        /// </summary>
        public CharacterCombat CharacterCombat => characterCombat;
        public CharacterMotor CharacterMotor => characterMotor;
        public CharacterAnimator CharacterAnimator => characterAnimator;
        public BodyRotationModule BodyRotationModule => bodyRotationModule;
        public RuntimeAnimatorController AnimatorController => animatorController;
        /// <summary>
        /// If this weapon was spawned through an equipment, it'll be stored here.
        /// </summary>
        public Equipment Equipment => equipment;
        #endregion

        #region MonoBehaviour Messages
        protected virtual void Awake()
        {
            deactivators = new ControlReceiver(OnFirstDeactivatorAdded, null, OnAllDeactivatorsCleared);

            if (!string.IsNullOrEmpty(aggressiveStateAnimatorBool))
                aggresiveStateAnimatorBoolHash = Animator.StringToHash(aggressiveStateAnimatorBool);
        }
        protected virtual void OnEnable()
        {
            damageSF = damage.Load();
            attackSpeedSF = attackSpeed.Load();
            critChanceSF = critChance.Load();
            critDamageSF = critDamage.Load();

            if(damageSF)
            {
                OnDamageValueChange(damageSF.Value);
                damageSF.OnValueChange.AddListener(OnDamageValueChange);
            }
            if(attackSpeedSF)
            {
                OnAttackSpeedValueChange(attackSpeedSF.Value);
                attackSpeedSF.OnValueChange.AddListener(OnAttackSpeedValueChange);
            }
            if(critChanceSF)
            {
                OnCritChanceValueChange(critChanceSF.Value);
                critChanceSF.OnValueChange.AddListener(OnCritChanceValueChange);
            }
            if(critDamageSF)
            {
                OnCritDamageValueChange(critDamageSF.Value);
                critDamageSF.OnValueChange.AddListener(OnCritDamageValueChange);
            }
        }
        protected virtual void OnDisable()
        {
            if(damageSF)
            {
                damageSF.OnValueChange.RemoveListener(OnDamageValueChange);
                damage.Release();
            }
            if(attackSpeedSF)
            {
                attackSpeedSF.OnValueChange.RemoveListener(OnAttackSpeedValueChange);
                attackSpeed.Release();
            }
            if(critChanceSF)
            {
                critChanceSF.OnValueChange.RemoveListener(OnCritChanceValueChange);
                critChance.Release();
            }
            if(critDamageSF)
            {
                critDamageSF.OnValueChange.RemoveListener(OnCritDamageValueChange);
                critDamage.Release();
            }
        }
        protected virtual void Start()
        {

        }
        protected virtual void OnDestroy()
        {
            if (characterCombat != null)
                characterCombat.OnAggressiveStateChanged.RemoveListener(SetAggressiveState);

            for (int i = 0; i < actionCategories.Count; i++)
            {
                var category = actionCategories[i];

                WeaponAction action = actionCategories[i].Action;

                if(action != null)
                    action.OnDestroy();
            }
        }
        protected virtual void OnDrawGizmos()
        { 

        }
        protected virtual void OnDrawGizmosSelected()
        { 

        }
        #endregion

        #region Callbacks
        private void OnFirstDeactivatorAdded()
        {
            Deactivate();
        }
        private void OnAllDeactivatorsCleared()
        {
            Activate();
        }
        protected virtual void OnDamageValueChange(float newValue)
        {
            curDamage = Mathf.RoundToInt(newValue);
        }
        protected virtual void OnAttackSpeedValueChange(float newValue)
        {
            curAttackCooldown = 1f / newValue;
        }
        protected virtual void OnCritChanceValueChange(float newValue)
        {
            curCritChance = newValue;
        }
        protected virtual void OnCritDamageValueChange(float newValue)
        {
            curCritDamage = newValue;
        }
        #endregion

        #region Private Functions
        private void Activate()
        {
            if(isActive)
                return;

            isActive = true;

            OnActivate();
        }
        private void Deactivate()
        {
            if(!isActive)
                return;

            isActive = false;

            OnDeactivate();
        }
        protected bool CanAttack()
        {
            return characterCombat.CanWeaponAttack && characterCombat.WeaponRequestAttack();
        }
        #endregion

        #region Public Functions
        public void Initialize(CharacterCombat characterCombat, TafraActor holderActor, Equipment equipment = null)
        {
            this.characterCombat = characterCombat;
            this.holderActor = holderActor;
            this.equipment = equipment;

            characterAnimator = holderActor.GetCachedComponent<CharacterAnimator>();
            characterMotor = holderActor.GetCachedComponent<CharacterMotor>();

            if (characterAnimator != null )
                animator = characterAnimator.Animator;

            if(characterMotor != null)
                bodyRotationModule = characterMotor.GetModule<BodyRotationModule>();

            SetAggressiveState(characterCombat.IsAggressive);

            characterCombat.OnAggressiveStateChanged.AddListener(SetAggressiveState);

            for (int i = 0; i < actionCategories.Count; i++)
            {
                var category = actionCategories[i];

                WeaponAction action = category.Action;

                if (action != null)
                    category.Action.Initialize(this);
            }

            OnInitialize();

            //Set the weapon's active state.
            if(!deactivators.HasAnyController())
            {
                isActive = false;   //Just to bypass the redundency check in Activate() function.
                Activate();
            }
            else
            {
                isActive = true;   //Just to bypass the redundency check in Deactivate() function.
                Deactivate();
            }
        }
        public void AddDeactivator(string deactivatorID)
        {
            deactivators.AddController(deactivatorID);
        }
        public void RemoveDeactivator(string deactivatorID)
        {
            deactivators.RemoveController(deactivatorID);
        }
        public void SetAggressiveState(bool aggressive)
        {
            if(animator != null && aggresiveStateAnimatorBoolHash != -1)
                animator.SetBool(aggresiveStateAnimatorBoolHash, aggressive);

            OnAggressiveStateChanged(aggressive);
        }
        public WeaponAction GetAction(int categoryIndex)
        {
            if (actionCategories.Count > categoryIndex)
                return actionCategories[categoryIndex].Action;
            else
                return null;
        }
        #endregion

        #region Children Only Functions
        protected virtual void OnInitialize() { }
        /// <summary>
        /// Gets called once the weapon is initialized (if no deactivators were added before this step), and whenever the weapon is deactivated and activated again afterwards.
        /// </summary>
        protected virtual void OnActivate() { }
        /// <summary>
        /// Gets called whenever the weapon is deactivated while it's active. Can get called on initialization if a deactivator exited.
        /// </summary>
        protected virtual void OnDeactivate() { }
        protected virtual void OnAggressiveStateChanged(bool aggressive) { }
        #endregion
    }
}