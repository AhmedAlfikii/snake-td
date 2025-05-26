using System.Collections;
using System.Collections.Generic;
using TafraKit.ModularSystem;
using UnityEngine;

namespace TafraKit.Healthies
{
    [System.Serializable]
    public abstract class HealthyModule : InternalModule
    {
        protected Healthy healthy;

        public abstract bool DisableOnDeath { get; }

        public void Initialize(Healthy healthy) 
        { 
            this.healthy = healthy;

            OnInitialize();
        }
    }
}