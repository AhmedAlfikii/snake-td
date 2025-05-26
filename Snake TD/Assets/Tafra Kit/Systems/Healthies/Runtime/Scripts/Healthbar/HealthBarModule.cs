using System.Collections;
using System.Collections.Generic;
using TafraKit.ModularSystem;
using UnityEngine;

namespace TafraKit.Healthies
{
    [System.Serializable]
    public abstract class HealthBarModule : InternalModule
    {
        protected HealthBar healthBar;

        public void Initialize(HealthBar healthBar)
        {
            this.healthBar = healthBar;

            OnInitialize();
        }
    }
}