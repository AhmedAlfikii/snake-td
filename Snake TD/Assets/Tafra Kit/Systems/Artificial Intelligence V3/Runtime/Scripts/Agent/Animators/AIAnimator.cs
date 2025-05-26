using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.ModularSystem;
using TafraKit.CharacterControls;
using System;

namespace TafraKit.AI3
{
    public class AIAnimator : CharacterAnimator
    {

        private AIAgent agent;

        public AIAgent Agent => agent;


        protected override void Awake()
        {
            base.Awake();
            
            agent = actor as AIAgent;
        }
    }
}