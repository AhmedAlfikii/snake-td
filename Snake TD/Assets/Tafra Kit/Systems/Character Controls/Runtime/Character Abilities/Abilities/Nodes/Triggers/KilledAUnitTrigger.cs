using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Triggers/Healthy/Killed a Unit"), GraphNodeName("Killed a Unit")]
    public class KilledAUnitTrigger : TriggerAbilityNode
    {
        /// <summary>
        /// The name of the property that will be added to the trigger blackboard and will be sent to all children that contains the healthy of the killed unit.
        /// </summary>
        [SerializeField] private string killedUnitPropertyName = "Unit Killed";

        [NonSerialized] private int killedUnitPropertyNameHash;

        public override bool UseTriggerBlackboard => true;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            killedUnitPropertyNameHash = Animator.StringToHash(killedUnitPropertyName);
        }
        protected override void OnStart()
        {
            base.OnStart();

            HealthiesTracker.OnHealthyDeath.AddListener(OnHealthDeath);
        }
        protected override void OnEnd()
        {
            base.OnEnd();

            HealthiesTracker.OnHealthyDeath.RemoveListener(OnHealthDeath);
        }

        private void OnHealthDeath(Healthy healthy, HitInfo hitInfo)
        {
            if(hitInfo.hitter != actor)
                return;
            
            TriggerInvoked((blackboard) => 
            {
                blackboard.SetTafraActorProperty(killedUnitPropertyNameHash, ComponentProvider.GetComponent<TafraActor>(healthy.gameObject));
            });
        }
    }
}