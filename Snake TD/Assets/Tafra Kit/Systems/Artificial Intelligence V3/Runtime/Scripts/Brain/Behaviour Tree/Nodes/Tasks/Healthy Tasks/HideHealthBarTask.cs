using UnityEngine;
using TafraKit.AI3;
using TafraKit.Healthies;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Healthy/Hide Health Bar"), GraphNodeName("Hide Health Bar")]
    public class HideHealthBarTask : TaskNode
    {
        [SerializeField] private BlackboardObjectGetter healthBar;
        [SerializeField] private string hiderID = "hideHealthBarTask";

        private HealthBar hb;
        private VisibilityModule visibilityModule;

        protected override void OnInitialize()
        {
            healthBar.Initialize(agent.BlackboardCollection);

            Object healthBarValue = healthBar.Value;

            if (healthBarValue != null )
            {
                hb = healthBarValue as HealthBar;

                if(hb == null)
                    TafraDebugger.Log("Hide Health Bar Task", "The object you assigned is not a HealthBar component, make sure to assign the component itself not the game object.", TafraDebugger.LogType.Error);
                else
                    visibilityModule = hb.GetModule<VisibilityModule>();
            }
        }
        protected override void OnStart()
        {
            if (visibilityModule != null)
                visibilityModule.AddHider(hiderID);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            if(visibilityModule != null)
                visibilityModule.RemoveHider(hiderID);
        }
    }
}