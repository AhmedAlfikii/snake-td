using UnityEngine;
using TafraKit.AI3;
using TafraKit.Healthies;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Healthy/Death Preventer"), GraphNodeName("Death Preventer", "DP")]
    public class DeathPreventerState : ActionState
    {
        [SerializeField] private bool completeOnPrevention = true;
        
        private Healthy healthy;

        protected override void OnInitialize()
        {
            healthy = agent.GetCachedComponent<Healthy>();
        }
        protected override void OnPlay()
        {
            //healthy.Events.OnAboutToTakeDamage.AddListener(OnAboutToTakeDamage);
            healthy.Events.OnAboutToDie.AddListener(OnAboutToDie);
        }
        protected override void OnConclude()
        {
            //healthy.Events.OnAboutToTakeDamage.RemoveListener(OnAboutToTakeDamage);
            healthy.Events.OnAboutToDie.RemoveListener(OnAboutToDie);
        }
        private void OnAboutToDie(Healthy healthy, AboutToDieEventArgs args)
        {
            args.PreventDeath = true;

            if(completeOnPrevention)
                Complete();
        }

        private void OnAboutToTakeDamage(Healthy healthy, HitEventArgs args)
        {
            if (args.ManipulatedHitInfo.damage >= healthy.CurrentHealth)
            {
                args.SetDamage(healthy.CurrentHealth - 1);
                
                if(completeOnPrevention)
                    Complete();
            }
        }
    }
}