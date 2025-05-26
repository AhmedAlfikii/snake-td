using TafraKit.AI3;
using TafraKit.AI3.Motor;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Motor/Go To Point Around Target"), GraphNodeName("Go Around Target", "Around Target")]
    public class GoAroundTargetTask : MotorActionTask
    {
        [SerializeField] private BlackboardDynamicPointGetter target;
        [Tooltip("The speed of which the agent will orbit it's target. 0 means use the agent's default speed.")]
        [SerializeField] private BlackboardDynamicFloatGetter orbitSpeed = new BlackboardDynamicFloatGetter(3.5f);
        [Tooltip("Use the current distance between the agent and the target point as the orbit distance.")]
        [SerializeField] private BlackboardBoolGetter useCurDistanceAsOrbitRadius = new BlackboardBoolGetter(true);
        [Tooltip("Only used if curDistanceIsOrbitDistance is set to false. The distance between the target and the point around it (circle radius).")]
        [SerializeField] private BlackboardDynamicFloatGetter orbitRadius = new BlackboardDynamicFloatGetter(6);
        [SerializeField] private BlackboardDynamicFloatGetter orbitAngle = new BlackboardDynamicFloatGetter(30);

        protected override AIMotor.ActionCategories ActionCategory => AIMotor.ActionCategories.MainMovement;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            target.Initialize(agent.BlackboardCollection);
            orbitSpeed.Initialize(agent.BlackboardCollection);
            useCurDistanceAsOrbitRadius.Initialize(agent.BlackboardCollection);
            orbitRadius.Initialize(agent.BlackboardCollection);
            orbitAngle.Initialize(agent.BlackboardCollection);
        }

        protected override int StartAction()
        {
            return motor.GoToPointAroundTarget(target.Value, orbitSpeed.Value, useCurDistanceAsOrbitRadius.Value, orbitRadius.Value, orbitAngle.Value, OnActionCompleted, OnActionInterrupted);
        }
    }
}