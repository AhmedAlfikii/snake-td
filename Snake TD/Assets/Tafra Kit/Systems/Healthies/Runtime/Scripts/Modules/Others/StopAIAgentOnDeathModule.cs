using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Others/Stop AI Agent On Death")]
    public class StopAIAgentOnDeathModule : HealthyModule
    {
        [Header("Settings")]
        [SerializeField] private bool playOnRevive = true;

        private AIAgent agent;
        private bool stopped;

        public override bool DisableOnDeath => false;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            agent = healthy.GetComponent<AIAgent>();
        }
        protected override void OnEnable()
        {
            if(stopped && !healthy.IsDead)
                OnRevive();

            healthy.Events.OnDeath.AddListener(OnDeath);
            healthy.Events.OnRevive.AddListener(OnRevive);
        }
        protected override void OnDisable()
        {
            healthy.Events.OnDeath.RemoveListener(OnDeath);
            healthy.Events.OnRevive.RemoveListener(OnRevive);
        }

        private void OnDeath(Healthy healthy, HitInfo killHit)
        {
            if (agent.IsPlaying)
                agent.Playable.Stop();

            stopped = true;
        }
        private void OnRevive()
        {
            if(playOnRevive)
            {
                if(!agent.IsPlaying)
                    agent.Playable.Play();

                stopped = false;
            }
        }
    }
}