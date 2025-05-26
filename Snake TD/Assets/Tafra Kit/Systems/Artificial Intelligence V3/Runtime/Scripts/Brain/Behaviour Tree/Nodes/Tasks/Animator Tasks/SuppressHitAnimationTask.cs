using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.CharacterControls;
using System;
using TafraKit.GraphViews;
using TafraKit.Healthies;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Animator/Suppress Hit Animation"), GraphNodeName("Suppress Hit Animation", "Suppress Hit Animation")]
    public class SuppressHitAnimationTask : TaskNode
    {
        private AIAnimator aiAnimator;
        private Healthy healthy;
        private HealthyBasicsAnimatorModule healthyBasicsModule;
        private StopAIAgentOnHitModule stopAIAgentOnHitModule;

        protected override void OnInitialize()
        {
            aiAnimator = agent.GetCachedComponent<AIAnimator>();
            healthy = agent.GetCachedComponent<Healthy>();
            
            if (aiAnimator != null)
                healthyBasicsModule = aiAnimator.GetModule<HealthyBasicsAnimatorModule>();

            if (healthy != null)
                stopAIAgentOnHitModule = healthy.GetModule<StopAIAgentOnHitModule>();
        }
        protected override void OnStart()
        {
            if (healthyBasicsModule != null)
                healthyBasicsModule.AddGotHitAnimationSupressor(guid);

            if (stopAIAgentOnHitModule != null)
                stopAIAgentOnHitModule.AddEffectSupressor(guid);
        }
        protected override void OnEnd()
        {
            if(healthyBasicsModule != null)
                healthyBasicsModule.RemoveGotHitAnimationSupressor(guid);

            if(stopAIAgentOnHitModule != null)
                stopAIAgentOnHitModule.RemoveEffectSupressor(guid);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
    }
}