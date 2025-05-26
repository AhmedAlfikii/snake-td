using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal.AI3;
using TafraKit.CharacterControls;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.AI3.Motor
{
    public class AINavMotor : AIMotor
    {
        private NavMeshAgent navAgent;
        protected NavGoToPointAction goToPointAction;
        protected NavChaseTargetAction chaseTargetAction;
        protected NavGoAroundTargetAction goAroundTargetAction;
        protected NavGoThroughPointAction goThroughPointAction;
        protected NavBodyLookAtTargetAction bodyLookAtTargetAction;
        protected InfluenceReceiver<float> agentSpeedInfluences;

        public NavMeshAgent NavAgent => navAgent;

        protected override void Awake()
        {
            base.Awake();
           
            navAgent = agent.GetCachedComponent<NavMeshAgent>();
           
            agentSpeedInfluences = new InfluenceReceiver<float>(ShouldReplaceAgentSpeed, OnActiveAgentSpeedInfluenceUpdated, null, OnAllAgentSpeedInfluencesCleared);
         
            agentSpeedInfluences.AddInfluence("default", defaultSpeed.Value * movementSpeedMultiplier);
        }

        #region Callbacks
        private bool ShouldReplaceAgentSpeed(float newSpeed, float oldSpeed)
        {
            //Always apply the latest speed.
            return true;
        }
        private void OnActiveAgentSpeedInfluenceUpdated(float speed)
        {
            navAgent.speed = speed;
        }
        private void OnAllAgentSpeedInfluencesCleared()
        {
            navAgent.speed = defaultSpeed.Value;
        }
        protected override void OnMovementSpeedMultiplierChange()
        {
            //Update the default movement speed influence, since default movement speed should always be affected by the multiplier.
            agentSpeedInfluences.AddInfluence("default", defaultSpeed.Value * movementSpeedMultiplier);
        }
        #endregion

        #region Action Functions
        protected override GoToPointAction GetGoToPointAction(Vector3 destination)
        {
            if (goToPointAction == null)
                goToPointAction = new NavGoToPointAction(this);

            goToPointAction.Initialize(destination);

            return goToPointAction;
        }
        protected override ChaseTargetAction GetChaseTargetAction(Transform target, float chaseSpeed, float stoppingDistance, bool completeOnArrival)
        {
            if (chaseTargetAction == null)
                chaseTargetAction = new NavChaseTargetAction(this);

            chaseTargetAction.Initialize(target, chaseSpeed, stoppingDistance, completeOnArrival);

            return chaseTargetAction;
        }
        protected override GoAroundTargetAction GetGoAroundTargetAction(Vector3 targetPoint, float speed, bool curDistanceIsOrbitDistance, float orbitDistance, float angle)
        {
            if (goAroundTargetAction == null)
                goAroundTargetAction = new NavGoAroundTargetAction(this);

            goAroundTargetAction.Initialize(targetPoint, speed, curDistanceIsOrbitDistance, orbitDistance, angle);

            return goAroundTargetAction;
        }
        protected override GoThroughPointAction GetGoThroughPointAction(Vector3 targetPoint, GoThroughPointAction.GoThroughTargetType motionType, float distanceAfterTarget, float speed, bool canDashThroughObstacles, LayerMask obstaclesLayerMask)
        {
            if (goThroughPointAction == null)
                goThroughPointAction = new NavGoThroughPointAction(this);

            goThroughPointAction.Initialize(targetPoint, motionType, distanceAfterTarget, speed, canDashThroughObstacles, obstaclesLayerMask);

            return goThroughPointAction;
        }
        protected override BodyLookAtTargetAction GetBodyLookAtTargetAction(Transform target, Vector3 point, float lookSpeed, bool completeOnAlign)
        {
            if(bodyLookAtTargetAction == null)
                bodyLookAtTargetAction = new NavBodyLookAtTargetAction(this);

            if (target != null)
                bodyLookAtTargetAction.Initialize(target, lookSpeed, completeOnAlign);
            else
                bodyLookAtTargetAction.Initialize(point, lookSpeed, completeOnAlign);

            return bodyLookAtTargetAction;
        }
        #endregion

        #region Public Functions
        public void SetAgentSpeed(string influencer, float speed)
        {
            agentSpeedInfluences.AddInfluence(influencer, speed);
        }
        public void ResetAgentSpeed(string influencer)
        {
            agentSpeedInfluences.RemoveInfluence(influencer);
            agentSpeedInfluences.AddInfluence("default", defaultSpeed.Value * movementSpeedMultiplier);
        }
        #endregion
    }
}