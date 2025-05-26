using TafraKit.ModularSystem;
using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public abstract class TafraActorModule : InternalModule
    {
        protected TafraActor actor;

        public void Initialize(TafraActor actor)
        {
            this.actor = actor;

            OnInitialize();
        }
    }
}