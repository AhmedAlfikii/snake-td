using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Effects/Healing Effects")]
    public class HealingEffectsModule : HealthyModule
    {
        [SerializeField] private ParticleSystem healingPSPrefab;
        [SerializeField] private SFXClips healingAudio;

        private ParticleSystem healingPS;
        
        public override bool DisableOnDeath => true;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            if (healingPSPrefab != null)
            {
                healingPS = GameObject.Instantiate(healingPSPrefab, healthy.transform);
                healingPS.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
        protected override void OnEnable()
        {
            healthy.Events.OnHeal.AddListener(OnHeal);
        }
        protected override void OnDisable()
        {
            healthy.Events.OnHeal.RemoveListener(OnHeal);
        }

        private void OnHeal(Healthy healthy, float healAmount)
        {
            if (healingPS != null)
                healingPS.Play();

            SFXPlayer.Play(healingAudio);
        }
    }
}