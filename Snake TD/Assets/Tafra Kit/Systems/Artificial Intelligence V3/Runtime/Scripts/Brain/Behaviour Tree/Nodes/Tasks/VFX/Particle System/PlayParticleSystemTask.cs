using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/VFX/Particle System/Play Particle System"), GraphNodeName("Play Particle System", "Play Particle System")]
    public class PlayParticleSystemTask : TaskNode
    {
        [SerializeField] private BlackboardObjectGetter particles;

        private ParticleSystem ps;

        protected override void OnInitialize()
        {
            particles.Initialize(agent.BlackboardCollection);

            UnityEngine.Object particlesValue = particles.Value;

            if(particlesValue != null)
            {
                ps = (ParticleSystem)particlesValue;

                if(ps == null)
                    TafraDebugger.Log("Play Particle System Task", "The object assigned is not a particle system component, make sure to drag the component itself not the game object.", TafraDebugger.LogType.Error);
            }
        }
        protected override void OnStart()
        {
            if(ps)
                ps.Play();
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}