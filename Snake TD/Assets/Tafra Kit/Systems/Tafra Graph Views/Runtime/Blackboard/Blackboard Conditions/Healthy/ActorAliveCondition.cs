using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Healthies;
using TafraKit.GraphViews;

namespace TafraKit.Internal.GraphViews
{
    [SearchMenuItem("Healthy/Actor is Alive")]
    public class ActorAliveCondition : BlackboardCondition
    {
        [SerializeField] private bool targetIsThisAgent;
        [SerializeField] private BlackboardActorGetter target;
        [SerializeField] private bool targetLivingState = true;

        private TafraActor cachedActor;
        private TafraKit.Healthies.Healthy healthy;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            target.Initialize(blackboardCollection);
        }
        protected override void OnActivate()
        {
            cachedActor = targetIsThisAgent ? actor : target.Value;

            if(cachedActor != null )
                healthy = cachedActor.GetCachedComponent<Healthy>();
        }
        protected override bool PerformCheck()
        {
            TafraActor curActor = targetIsThisAgent ? actor : target.Value;

            if(curActor == null)
                return false;

            if(curActor != cachedActor)
            {
                cachedActor = curActor;
                healthy = cachedActor.GetCachedComponent<Healthy>();
            }

            if(healthy == null)
                return false;

            bool isAlive = !healthy.IsInitialized || !healthy.IsDead;

            return isAlive == targetLivingState;
        }
    }
}