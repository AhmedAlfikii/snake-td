using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Others/Disable Nav Agent On Death")]
    public class DisableAgentOnDeathModule : HealthyModule
    {
        [Header("Settings")]
        [SerializeField] private bool enableOnRevive = true;

        private NavMeshAgent agent;
        private bool disabled;

        public override bool DisableOnDeath => false;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            agent = healthy.GetComponent<NavMeshAgent>();
        }
        protected override void OnEnable()
        {
            if(disabled && !healthy.IsDead)
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
            agent.enabled = false;
            disabled = true;
        }
        private void OnRevive()
        {
            if(enableOnRevive)
            {
                agent.enabled = true;
                disabled = false;
            }
        }
    }
}