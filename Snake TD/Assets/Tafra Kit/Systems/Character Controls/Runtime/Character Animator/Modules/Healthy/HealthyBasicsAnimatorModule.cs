using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Healthies;
using TafraKit.CharacterControls;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Healthy/Healthy Basics")]
    public class HealthyBasicsAnimatorModule : CharacterAnimatorModule
    {
        [Header("Death")]
        [SerializeField] private bool trackDeath;
        [Tooltip("The name of the animator's bool parameter that should contian the is dead state.")]
        [SerializeField] private string deathAnimatorBoolParam = "Is Dead";
        [Tooltip("The name of the animator's trigger parameter that will get triggered whenever the healthy dies. Can be left empty if not needed.")]
        [SerializeField] private string deathAnimatorTriggerParam = "Died";
        [Tooltip("The number of got death animation variants in the animator, one of them will be randomly played whenever the healthy dies. Can be left empty if not needed.")]
        [Min(1)]
        [SerializeField] private int deathAnimationsCount = 1;
        [Tooltip("The name of the int animator parameter that will contain the index of the random death animation that got selected. This will be a value starting at 0 up to death animations count - 1.")]
        [SerializeField] private string deathAnimationIndexAnimatorParam = "Death Index";

        [Header("Hits Taken")]
        [SerializeField] private bool trackHitsTaken;
        [SerializeField] private string gotHitAnimatorTriggerParam = "Got Hit";
        [Tooltip("The number of got hit animation variants in the animator, one of them will be randomly played each time the healthy gets hit.")]
        [Min(1)]
        [SerializeField] private int gotHitAnimationsCount = 1;
        [Tooltip("The name of the int animator parameter that will contain the index of the random hit animation that got selected. This will be a value starting at 0 up to hit animations count - 1.")]
        [SerializeField] private string gotHitAnimationIndexAnimatorParam = "Got Hit Index";

        [Header("Reviving")]
        [SerializeField] private bool trackRevive;
        [Tooltip("The name of the animator's trigger parameter that should be triggered whenever the healthy revives.")]
        [SerializeField] private string reviveAnimatorTriggerParam = "Revived";

        private Healthy healthy;
        private int deathAnimatorBoolParamHash = -1;
        private int deathAnimatorTriggerParamHash = -1;
        private int deathAnimationIndexAnimatorParamHash = -1;
        private int gotHitAnimatorTriggerParamHash = -1;
        private int gotHitAnimationIndexAnimatorParamHash = -1;
        private int reviveAnimatorTriggerParamHash = -1;
        private ControlReceiver hitAnimationSuppressors;
        private bool supressGotHitAnimation;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            hitAnimationSuppressors = new ControlReceiver(OnFirstHitSuppressorAdded, null, OnAllHitSuppressorsCleared);
            healthy = actor.GetCachedComponent<Healthy>();

            if (trackDeath)
            {
                if (!string.IsNullOrEmpty(deathAnimatorBoolParam))
                    deathAnimatorBoolParamHash = Animator.StringToHash(deathAnimatorBoolParam);

                if (!string.IsNullOrEmpty(deathAnimatorTriggerParam))
                    deathAnimatorTriggerParamHash = Animator.StringToHash(deathAnimatorTriggerParam);

                if (deathAnimationsCount > 1)
                    deathAnimationIndexAnimatorParamHash = Animator.StringToHash(deathAnimationIndexAnimatorParam);
            }

            if (trackHitsTaken)
            {
                gotHitAnimatorTriggerParamHash = Animator.StringToHash(gotHitAnimatorTriggerParam);

                if (gotHitAnimationsCount > 1)
                    gotHitAnimationIndexAnimatorParamHash = Animator.StringToHash(gotHitAnimationIndexAnimatorParam);
            }

            if (trackRevive)
            {
                reviveAnimatorTriggerParamHash = Animator.StringToHash(reviveAnimatorTriggerParam);
            }
        }

        protected override void OnEnable()
        {
            if (trackDeath)
            {
                healthy.Events.OnDeath.AddListener(OnDeath);

                if (healthy.IsInitialized && healthy.IsDead)
                    OnDeath(healthy, new HitInfo());
            }

            if (trackHitsTaken)
                healthy.Events.OnTakenDamage.AddListener(OnGotHit);

            if (trackRevive)
                healthy.Events.OnRevive.AddListener(OnRevive);
        }
        protected override void OnDisable()
        {
            if (trackDeath)
                healthy.Events.OnDeath.RemoveListener(OnDeath);

            if (trackHitsTaken)
                healthy.Events.OnTakenDamage.RemoveListener(OnGotHit);

            if (trackRevive)
                healthy.Events.OnRevive.RemoveListener(OnRevive);

            hitAnimationSuppressors.RemoveAllControllers();
            supressGotHitAnimation = false;
        }

        private void OnDeath(Healthy healthy, HitInfo killerHit)
        {
            if (deathAnimationsCount > 1)
            {
                int randomDeathAnimIndex = UnityEngine.Random.Range(0, deathAnimationsCount);
                animator.SetInteger(deathAnimationIndexAnimatorParamHash, randomDeathAnimIndex);
            }

            if (deathAnimatorTriggerParamHash != -1)
                animator.SetTrigger(deathAnimatorTriggerParamHash);
            if (deathAnimatorBoolParamHash != -1)
                animator.SetBool(deathAnimatorBoolParamHash, true);
        }
        private void OnGotHit(Healthies.Healthy healthy, HitInfo hitInfo)
        {
            if(supressGotHitAnimation)
                return;

            if (gotHitAnimationsCount > 1)
            {
                int randomHitAnimIndex = UnityEngine.Random.Range(0, gotHitAnimationsCount);
                animator.SetInteger(gotHitAnimationIndexAnimatorParamHash, randomHitAnimIndex);
            }

            animator.SetTrigger(gotHitAnimatorTriggerParamHash);
        }
        private void OnRevive()
        {
            if (deathAnimatorBoolParamHash != -1)
                animator.SetBool(deathAnimatorBoolParamHash, false);

            animator.SetTrigger(reviveAnimatorTriggerParamHash);
        }

        private void OnFirstHitSuppressorAdded()
        {
            supressGotHitAnimation = true;
        }
        private void OnAllHitSuppressorsCleared()
        {
            supressGotHitAnimation = false;
        }

        public void AddGotHitAnimationSupressor(string suppressor)
        {
            hitAnimationSuppressors.AddController(suppressor);
        }
        public void RemoveGotHitAnimationSupressor(string suppressor)
        {
            hitAnimationSuppressors.RemoveController(suppressor);
        }
    }
}