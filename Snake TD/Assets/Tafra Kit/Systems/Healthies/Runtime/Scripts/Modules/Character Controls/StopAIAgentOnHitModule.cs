using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.CharacterControls;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Character Controls/Stop AI Agent On Hit")]
    public class StopAIAgentOnHitModule : HealthyModule
    {
        [SerializeField] private float duration = 0.25f;

        private AIAgent agent;
        private IEnumerator disablingAgentEnum;
        private ControlReceiver effectSuppressors;
        private bool supressEffect;

        public override bool DisableOnDeath => false;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            agent = healthy.GetComponent<AIAgent>();
            effectSuppressors = new ControlReceiver(OnFirstEffectSuppressorAdded, null, OnAllEffectSuppressorsCleared);
        }
        protected override void OnEnable()
        {
            healthy.Events.OnTakenDamage.AddListener(OnTakenDamage);
        }
        protected override void OnDisable()
        {
            healthy.Events.OnTakenDamage.RemoveListener(OnTakenDamage);
           
            if(disablingAgentEnum != null)
                healthy.StopCoroutine(disablingAgentEnum);
        }
        private void OnFirstEffectSuppressorAdded()
        {
            supressEffect = true;
        }
        private void OnAllEffectSuppressorsCleared()
        {
            supressEffect = false;
        }

        private void OnTakenDamage(Healthy healthy, HitInfo hitInfo)
        {
            if(supressEffect)
                return;

            if (disablingAgentEnum != null)
                healthy.StopCoroutine(disablingAgentEnum);

            if(duration > 0.001f)
            {
                disablingAgentEnum = DisablingAgent();

                healthy.StartCoroutine(disablingAgentEnum);
            }
        }

        private IEnumerator DisablingAgent()
        {
            agent.Playable.Pause("StopAIAgentOnHitModule");

            yield return Yielders.GetWaitForSeconds(duration);

            agent.Playable.Resume("StopAIAgentOnHitModule");
        }

        public void AddEffectSupressor(string suppressor)
        {
            effectSuppressors.AddController(suppressor);
        }
        public void RemoveEffectSupressor(string suppressor)
        {
            effectSuppressors.RemoveController(suppressor);
        }
    }
}