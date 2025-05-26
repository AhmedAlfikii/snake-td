using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.Cinemachine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Camera/ScreenShake"), GraphNodeName("Screen Shake")]
    public class ScreenShakeTaskNode : TaskNode
    {
        [SerializeField] private BlackboardDynamicFloatGetter shakeDuration = new BlackboardDynamicFloatGetter(0.35f);
        [SerializeField] private BlackboardDynamicFloatGetter shakeAmplitude = new BlackboardDynamicFloatGetter(1f);
        [SerializeField] private BlackboardDynamicFloatGetter shakeFrequency = new BlackboardDynamicFloatGetter(1f);

        protected override void OnInitialize()
        {
            shakeDuration.Initialize(agent.BlackboardCollection);
            shakeAmplitude.Initialize(agent.BlackboardCollection);
            shakeFrequency.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            ScreenShaker.Shake(shakeAmplitude.Value, shakeFrequency.Value, shakeDuration.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}