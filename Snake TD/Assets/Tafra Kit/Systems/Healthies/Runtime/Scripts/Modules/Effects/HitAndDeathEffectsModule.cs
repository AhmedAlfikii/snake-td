using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Effects/Hit & Death Effects")]
    public class HitAndDeathEffectsModule : HealthyModule
    {
        [Header("Taking Damage")]
        [SerializeField] private SFXClips gotHitAudio;
        [SerializeField] private ParticleSystem gotHitPS;

        [Header("Death")]
        [SerializeField] private SFXClips diedAudio;
        [SerializeField] private ParticleSystem diedPS;

        public override bool DisableOnDeath => true;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnEnable()
        {
            healthy.Events.OnTakenDamage.AddListener(OnTakenDamage);
            healthy.Events.OnDeath.AddListener(OnDeath);
        }
        protected override void OnDisable()
        {
            healthy.Events.OnTakenDamage.RemoveListener(OnTakenDamage);
            healthy.Events.OnDeath.RemoveListener(OnDeath);
        }

        private void OnDeath(Healthy healthy, HitInfo killerHit)
        {
            SFXPlayer.Play(diedAudio);

            if(diedPS)
                diedPS.Play();
        }
        private void OnTakenDamage(Healthy healthy, HitInfo hitInfo)
        {
            SFXPlayer.Play(gotHitAudio);

            if(gotHitPS)
                gotHitPS.Play();
        }
    }
}