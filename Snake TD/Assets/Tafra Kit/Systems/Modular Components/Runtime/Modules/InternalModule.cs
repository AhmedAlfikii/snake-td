using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.ModularSystem
{
    [Serializable]
    public abstract class InternalModule
    {
        public abstract bool UseUpdate { get; }
        public abstract bool UseLateUpdate { get; }
        public abstract bool UseFixedUpdate { get; }

        protected bool isEnabled;

        public void Enable()
        {
            if(isEnabled)
                return;

            isEnabled = true;
            
            OnEnable();
        }
        public void Disable()
        {
            if(!isEnabled)
                return;

            isEnabled = false;

            OnDisable();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        public virtual void OnDestroy() { }

        public virtual void OnControllerStart() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void FixedUpdate() { }

        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }
    }
}