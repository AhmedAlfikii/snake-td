using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.Cinemachine;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Camera/Screen Shake"), GraphNodeName("Screen Shake")]
    public class ScreenShakeAbilityTaskNode : AbilityTaskNode
    {
        [SerializeField] private float shakeDuration = 0.35f;
        [SerializeField] private float shakeAmplitude = 1f;
        [SerializeField] private float shakeFrequency = 1f;

        public ScreenShakeAbilityTaskNode(ScreenShakeAbilityTaskNode other) : base(other)
        {
            shakeDuration = other.shakeDuration;
            shakeAmplitude = other.shakeAmplitude;
            shakeFrequency = other.shakeFrequency;
        }
        public ScreenShakeAbilityTaskNode()
        {

        }

        protected override void OnStart()
        {
            ScreenShaker.Shake(shakeAmplitude, shakeFrequency, shakeDuration);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            ScreenShakeAbilityTaskNode clonedNode = new ScreenShakeAbilityTaskNode(this);

            return clonedNode;
        }
    }
}