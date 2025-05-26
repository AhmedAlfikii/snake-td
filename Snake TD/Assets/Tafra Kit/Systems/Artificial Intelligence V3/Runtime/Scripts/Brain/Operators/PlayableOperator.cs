using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3
{
    public class PlayableOperator
    {
        #region MonoBehaviour Messages Mimic
        public void Initialize()
        {
            OnInitialize();
        }
        public void RaisePlayFlag()
        {
            OnPlayFlagRaised();
        }
        public void RaiseStopFlag()
        {
            OnStopFlagRaised();
        }
        public virtual void Update()
        {

        }
        public virtual void LateUpdate()
        {

        }
        public virtual void FixedUpdate()
        {

        }

        #if UNITY_EDITOR
        public virtual void OnDrawGizmos()
        {

        }
        public virtual void OnDrawGizmosSelected()
        {

        }
        #endif
        #endregion

        protected virtual void OnInitialize() { }
        protected virtual void OnPlayFlagRaised() { }
        protected virtual void OnStopFlagRaised() { }
    }
}