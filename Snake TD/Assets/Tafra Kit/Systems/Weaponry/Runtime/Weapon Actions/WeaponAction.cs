using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using UnityEngine;

namespace TafraKit.Weaponry
{
    [Serializable]
    public abstract class WeaponAction
    {
        protected Weapon weapon;
        protected bool isPerforming;
        protected Action completed;
        protected BodyRotationModule bodyRotationModule;

        public Action Completed => completed;

        public void Initialize(Weapon weapon)
        { 
            this.weapon = weapon;
            bodyRotationModule = weapon.BodyRotationModule;

            OnInitialize();
        }
        /// <summary>
        /// Inform the action that its input is down, actions that have "hold" behvaiour will be waiting for input up call to release the hold.
        /// </summary>
        /// <returns>Whether or not the action will be performed.</returns>
        public bool PerformanceInputDown()
        {
            if(!CanPerform())
                return false;

            isPerforming = true;

            StartPerformance();

            return true;
        }
        /// <summary>
        /// For actions that have "hold" behaviour. If called on an action that doesn't have one, nothing will happen.
        /// </summary>
        public void  PerformanceInputReleased()
        { 

        }
        public bool InterruptPerformanceOrder()
        {
            if(!isPerforming)
                return false;

            InterruptPerformance();
            
            isPerforming = false;

            return true;
        }

        protected void CompletePerformance()
        {
            if(!isPerforming)
                return;

            isPerforming = false;

            completed?.Invoke();
        }
        public virtual bool CanPerform()
        {
            if(isPerforming)
            {
                Debug.Log("Aready performing, can't perform again.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets called when the weapon is destroyed.
        /// </summary>
        public virtual void OnDestroy() { }
        protected virtual void OnInitialize() { }
        protected abstract void OnPerformanceInputReleased();
        protected abstract void StartPerformance();
        protected abstract void InterruptPerformance();
    }
}