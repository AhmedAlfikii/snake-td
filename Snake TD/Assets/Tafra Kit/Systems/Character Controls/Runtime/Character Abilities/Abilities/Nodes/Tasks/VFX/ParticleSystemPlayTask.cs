using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;
using System;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/VFX/Play Particle System"), GraphNodeName("Play Particle System")]
    public class ParticleSystemPlayTask : AbilityTaskNode
    {
        [SerializeField] private BlackboardObjectGetter particleSystem;

        public ParticleSystemPlayTask(ParticleSystemPlayTask other) : base(other)
        {
            particleSystem = new BlackboardObjectGetter(other.particleSystem);
        }
        public ParticleSystemPlayTask()
        {

        }

        protected override void OnInitialize()
        {
            particleSystem.Initialize(ability.BlackboardCollection);
        }
        protected override BTNodeState OnUpdate()
        {
            ParticleSystem ps = particleSystem.Value as ParticleSystem;

            if(ps != null)
                ps.Play();

            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            ParticleSystemPlayTask clonedNode = new ParticleSystemPlayTask(this);

            return clonedNode;
        }
    }
}