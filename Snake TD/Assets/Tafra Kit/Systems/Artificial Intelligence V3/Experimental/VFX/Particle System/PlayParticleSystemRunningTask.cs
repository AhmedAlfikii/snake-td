using TafraKit.AI3;
using TafraKit;
using UnityEngine;
using TafraKit.Internal.AI3;
using TafraKit.GraphViews;

[SearchMenuItem("Tasks/VFX/Particle System/Play Particle System (Running)"), GraphNodeName("Play Particle System (Running)")]
public class PlayParticleSystemRunningTask : TaskNode
{
    [SerializeField] private BlackboardObjectGetter particles;

    private ParticleSystem ps;

    protected override void OnInitialize()
    {
        particles.Initialize(agent.BlackboardCollection);

        UnityEngine.Object particlesValue = particles.Value;

        if (particlesValue != null)
        {
            ps = (ParticleSystem)particlesValue;

            if (ps == null)
                TafraDebugger.Log("Play Particle System Task", "The object assigned is not a particle system component, make sure to drag the component itself not the game object.", TafraDebugger.LogType.Error);
        }
    }
    protected override void OnStart()
    {
        if (ps)
            ps.Play();
    }
    protected override void OnEnd()
    {
        if (ps)
            ps.Stop();
    }
    protected override BTNodeState OnUpdate()
    {
        return BTNodeState.Running;
    }
}
