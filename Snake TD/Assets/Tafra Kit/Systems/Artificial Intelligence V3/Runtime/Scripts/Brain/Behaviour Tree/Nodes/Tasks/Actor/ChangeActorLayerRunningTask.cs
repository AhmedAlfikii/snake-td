using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Actor/Change Actor Layer (Running)"), GraphNodeName("Change Actor Layer (Running)"), RawDisplayInGraphInspector()]
    public class ChangeActorLayerRunningTask : TaskNode
    {
        [SerializeField] private BlackboardDynamicIntGetter layer;
        [SerializeField] private bool targetThisAgent;
        [SerializeField] private BlackboardActorGetter targetActor;
        [SerializeField] private string changerID = "changeActorLayerRunningTask";

        protected override void OnInitialize()
        {
            layer.Initialize(agent.BlackboardCollection);

            if(!targetThisAgent)
                targetActor.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            if(targetThisAgent)
                agent.AddLayerChanger(changerID, layer.Value);
            else
                targetActor.Value.AddLayerChanger(changerID, layer.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            if(targetThisAgent)
                agent.RemoveLayerChanger(changerID);
            else
                targetActor.Value.RemoveLayerChanger(changerID);
        }
    }
}