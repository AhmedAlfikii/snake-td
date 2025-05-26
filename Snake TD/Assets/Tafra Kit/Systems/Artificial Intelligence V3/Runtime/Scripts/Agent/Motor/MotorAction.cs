using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Motor
{
    public abstract class MotorAction
    {
        protected AIMotor motor;

        public Action OnCompletion;

        public abstract bool UseUpdate { get; }
        public abstract bool UseLateUpdate { get; }
        public abstract bool UseFixedUpdate { get; }

        public MotorAction(AIMotor motor)
        {
            this.motor = motor;

        }

        protected void Complete()
        {
            OnComplete();
            OnConclude();

            OnCompletion?.Invoke();
        }

        public void Interrupt()
        {
            OnInterrupt();
            OnConclude();
        }

        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        public virtual void OnLateUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnComplete() { }
        public virtual void OnInterrupt() { }
        public virtual void OnConclude() { }
    }
}