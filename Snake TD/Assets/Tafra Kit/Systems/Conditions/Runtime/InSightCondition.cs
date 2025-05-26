using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Conditions
{
    public class InSightCondition : Condition
    {
        [SerializeField] private ZFieldOfView fieldOfView;

        protected override void OnActivate()
        {          
            fieldOfView.OnSomeoneSighted.AddListener(OnSomeoneSighted);
        }
        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            fieldOfView.OnSomeoneSighted.RemoveListener(OnSomeoneSighted);
        }

        protected override bool PerformCheck()
        {
            return fieldOfView.IsSomeoneSighted();
        }

        private void OnSomeoneSighted()
        {
            Satisfy();
        }
    }
}