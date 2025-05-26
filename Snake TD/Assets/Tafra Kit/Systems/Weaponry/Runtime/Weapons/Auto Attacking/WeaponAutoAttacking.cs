using System;
using TafraKit.CharacterControls;
using TafraKit.Healthies;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace TafraKit.Weaponry
{
    public abstract class WeaponAutoAttacking : Weapon
    {
        [Header("Auto Attacking")]
        [SerializeField] private TafraFloat autoAttackRange = new TafraFloat(10);
        [SerializeField] private string animatorAttackTrigger = "Shoot";

        protected TargetsDetectorModule targetsDetector;
        /// <summary>
        /// The target that has been detected by the detector, not necessarily the weapon's attack target yet since the auto attack range needs to be checked first.
        /// </summary>
        protected TafraActor detectedTargetActor;
        /// <summary>
        /// The actor that the weapon should be attacking since it's detected by the detector and it's in the weapon's auto attack range.
        /// </summary>
        protected TafraActor attackTargetActor;
        /// <summary>
        /// The actor that the weapon should be attacking since it's detected by the detector and it's in the weapon's auto attack range.
        /// </summary>
        protected Healthy attackTargetHealthy;
        protected float curAutoAttackRangeSqr;
        /// <summary>
        /// Is true as long as a target is in range and the weapon is currently active (this means it's attacking the target).
        /// </summary>
        protected bool isAggressive;
        protected float nextAttackTime;
        protected int animatorAttackTriggerHash;

        private UnityEvent onAttackTargetUpdated = new UnityEvent();

        public TafraActor AttackTargetActor => attackTargetActor;
        public Healthy AttackTargetHealthy => attackTargetHealthy;
        public UnityEvent OnAttackTargetUpdated => onAttackTargetUpdated;

        #region MonoBehaviour Messages
        protected override void OnEnable()
        {
            base.OnEnable();

            OnAutoAttackRangeValueChanged(autoAttackRange.Value);
            
            if(autoAttackRange.ScriptableVariable)
                autoAttackRange.ScriptableVariable.OnValueChange.AddListener(OnAutoAttackRangeValueChanged);
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            if(autoAttackRange.ScriptableVariable)
                autoAttackRange.ScriptableVariable.OnValueChange.RemoveListener(OnAutoAttackRangeValueChanged);
        }
        private void Update()
        {
            if(!isActive)
                return;

            if(attackTargetHealthy)
            {
                if(Time.time > nextAttackTime)
                {
                    AttemptAttackingTaret(attackTargetHealthy);
                    UpdateNextAttackTime();
                }
            }

            //Check for attack target.
            bool targetIsInRange = false;
            if(detectedTargetActor != null)
            {
                float sqrDistance = (detectedTargetActor.transform.position - transform.position).sqrMagnitude;
                if(sqrDistance <= curAutoAttackRangeSqr)
                {
                    targetIsInRange = true;

                    if(detectedTargetActor != attackTargetActor)
                        OnFoundAttackTarget(detectedTargetActor);
                }
            }

            //If we have an attack target actor, yet the closest target isn't in range, or if the target was destroyed, then unhook from that attack target actor.
            if((!targetIsInRange && attackTargetActor) || (!attackTargetActor && isAggressive))
            {
                OnLostAttackTarget();
            }

            if(isAggressive)
            {
                characterCombat.PotentialPointOfInterest = attackTargetHealthy.TargetPoint.position;
            }
        }
        #endregion

        #region Inherited Methods
        protected override void OnInitialize()
        {
            base.OnInitialize();

            targetsDetector = characterCombat.GetModule<TargetsDetectorModule>();
            animatorAttackTriggerHash = Animator.StringToHash(animatorAttackTrigger);

            if(targetsDetector == null)
            {
                TafraDebugger.Log("Weapon Auto Attacking", "Can't use an auto attacking weapon without having a \"Targets Detector\" module on the Character Combat component.", TafraDebugger.LogType.Error, gameObject);
                return;
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();

            if(targetsDetector != null)
            {
                if(targetsDetector.Target != null)
                    OnDetectorFoundTarget(targetsDetector.Target);

                targetsDetector.OnFoundTarget.AddListener(OnDetectorFoundTarget);
                targetsDetector.OnLostTarget.AddListener(OnDetectorLostTarget);
            }
        }
        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            if (detectedTargetActor != null)
                OnDetectorLostTarget();

            if(attackTargetActor != null)
                OnLostAttackTarget();

            if(targetsDetector != null)
            {
                targetsDetector.OnFoundTarget.RemoveListener(OnDetectorFoundTarget);
                targetsDetector.OnLostTarget.RemoveListener(OnDetectorLostTarget);
            }
        }
        #endregion

        #region Callbacks
        private void OnAutoAttackRangeValueChanged(float newValue)
        {
            curAutoAttackRangeSqr = newValue * newValue;
        }
        private void OnDetectorFoundTarget(Collider targetCollider)
        {
            detectedTargetActor = ComponentProvider.GetComponent<TafraActor>(targetCollider.gameObject);
        }
        private void OnFoundAttackTarget(TafraActor actor)
        {
            //If we already have a target in attack range, unhook from it.
            if(attackTargetActor)
                UnhookFromCurrentAttackTarget();

            HookToAttackTarget(actor);

            if (bodyRotationModule != null)
                bodyRotationModule.LockAtTarget("weaponAutoAttacking", attackTargetHealthy.TargetPoint);

            if(!isAggressive)
                SwitchAggressionMode(true);

            onAttackTargetUpdated?.Invoke();
        }
        private void OnDetectorLostTarget()
        {
            detectedTargetActor = null;

            if(attackTargetActor != null)
                OnLostAttackTarget();
        }
        private void OnLostAttackTarget()
        {
            UnhookFromCurrentAttackTarget();

            if (bodyRotationModule != null)
                bodyRotationModule.UnlockTarget("weaponAutoAttacking");
           
            if(isAggressive)
                SwitchAggressionMode(false);

            onAttackTargetUpdated?.Invoke();
        }
        private void OnAttackTargetDeath(Healthy healthy, HitInfo hitInfo)
        {
            //Force a detection to be made immediately since we know we lost our target.
            if(targetsDetector != null)
                targetsDetector.Detect();
        }
        #endregion

        #region Private Functions
        private void HookToAttackTarget(TafraActor actor)
        {
            attackTargetActor = actor;
            attackTargetHealthy = actor.GetCachedComponent<Healthy>();

            if(attackTargetHealthy != null)
                attackTargetHealthy.Events.OnDeath.AddListener(OnAttackTargetDeath);
        }
        private void UnhookFromCurrentAttackTarget()
        {
            if(attackTargetHealthy != null)
                attackTargetHealthy.Events.OnDeath.RemoveListener(OnAttackTargetDeath);

            attackTargetActor = null;
            attackTargetHealthy = null;
        }
        private void SwitchAggressionMode(bool aggressive)
        {
            if(aggressive == isAggressive) return;

            isAggressive = aggressive;

            if(isAggressive)
            {
                SwitchToAggressive();
            }
            else
                SwitchToPassive();
        }
        protected virtual void UpdateNextAttackTime()
        {
            nextAttackTime = Time.time + curAttackCooldown;
        }
        private void SwitchToAggressive()
        {
            OnSwitchToAggressive();
            
            animator.SetBool(aggresiveStateAnimatorBoolHash, true);
         
            UpdateNextAttackTime();

            characterCombat.AddAggressiveStateContributer("weapon");
            characterCombat.AddPotentialPoIContinuousApplier("weapon");
        }
        private void SwitchToPassive()
        {
            OnSwitchToPassive();

            animator.SetBool(aggresiveStateAnimatorBoolHash, false);
            characterCombat.RemoveAggressiveStateContributer("weapon");
            characterCombat.RemovePotentialPoIContinuousApplier("weapon");
        }
        #endregion

        /// <summary>
        /// Gets called once the weapon finds a target after having none.
        /// </summary>
        protected virtual void OnSwitchToAggressive()
        {

        }
        /// <summary>
        /// Gets called once the weapons loses the target (if it dies or gets out of range) after having one.
        /// </summary>
        protected virtual void OnSwitchToPassive()
        {

        }

        public void AttemptAttackingTaret(Healthy target)
        {
            if(!CanAttack() || target == null || target.IsDead)
                return;

            StartAttackingTarget(target);
        }
        public virtual void StartAttackingTarget(Healthy target)
        {
            throw new NotImplementedException();
        }
        public virtual void PerformAttackAction(Healthy target)
        {
            throw new NotImplementedException();
        }
    }
}