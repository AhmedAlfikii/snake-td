using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Conditions;
using TafraKit.GraphViews;

namespace TafraKit.Internal.GraphViews
{
    [Serializable]
    public abstract class BlackboardCondition
    {
        protected TafraActor actor;
        protected BlackboardCollection blackboardCollection;

        protected bool isInitialized;
        protected bool isActive;

        public void SetDependencies(TafraActor actor, BlackboardCollection blackboardCollection)
        {
            this.actor = actor;
            this.blackboardCollection = blackboardCollection;
        }

        public void Activate()
        {
            if(!isInitialized)
                Initialize();

            isActive = true;

            OnActivate();
        }
        public void Deactivate()
        {
            isActive = false;
            OnDeactivate();
        }

        private void Initialize()
        {
            isInitialized = true;

            OnInitialize();
        }
        protected virtual void OnInitialize() { }
        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }
        protected abstract bool PerformCheck();

        /// <summary>
        /// Check whether or not the condition should be satisfied, and satisfy it if it should. 
        /// </summary>
        public bool Check()
        {
            if(!isInitialized)
            {
                Debug.LogError("Condition isn't initialized, please activate it before performing a check.");
                return false;
            }

            bool satisfied = PerformCheck();

            return satisfied;
        }
    }
}