using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [System.Serializable]
    public abstract class AIBTNode : BTNode
    {
        [NonSerialized] protected AIAgent agent;

        public void Initialize(AIAgent agent)
        {
            this.agent = agent;

            OnInitialize();
        }

        public override void OnDestroy()
        {

        }
    }
}
