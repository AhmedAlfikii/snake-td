using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using UnityEngine;

namespace TafraKit.AI3.Motor
{
    public abstract class AIMotor : CharacterMotor
    {
        public enum ActionCategories
        { 
            MainMovement = 0,
            MainRotation = 1,
            Displacement = 2
        }

        protected AIAgent agent;
        protected Dictionary<int, MotorActionCategory> motorActionsByCategory = new Dictionary<int, MotorActionCategory>();
        protected List<MotorActionCategory> allMotorActionCategories = new List<MotorActionCategory>();

        protected override void Awake()
        {
            base.Awake();

            agent = GetComponent<AIAgent>();
        }
        protected override void Start()
        {
            base.Start();
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            for(int i = 0; i < allMotorActionCategories.Count; i++)
            {
                var category = allMotorActionCategories[i];
                var activeAction = category.ActiveAction;

                if(activeAction != null)
                    activeAction.Interrupt();
            }
        }
        protected override void Update()
        {
            base.Update();

            for(int i = 0; i < allMotorActionCategories.Count; i++)
            {
                var category = allMotorActionCategories[i];
                var activeAction = category.ActiveAction;

                if (category.CanAct && activeAction != null && activeAction.UseUpdate)
                    activeAction.OnUpdate();
            }
        }
        protected override void LateUpdate()
        {
            base.LateUpdate();
          
            for(int i = 0; i < allMotorActionCategories.Count; i++)
            {
                var category = allMotorActionCategories[i];
                var activeAction = category.ActiveAction;

                if (category.CanAct && activeAction != null && activeAction.UseLateUpdate)
                    activeAction.OnLateUpdate();
            }
        }
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
       
            for(int i = 0; i < allMotorActionCategories.Count; i++)
            {
                var category = allMotorActionCategories[i];
                var activeAction = category.ActiveAction;

                if (category.CanAct && activeAction != null && activeAction.UseFixedUpdate)
                    activeAction.OnFixedUpdate();
            }
        }
        
        protected abstract GoToPointAction GetGoToPointAction(Vector3 destination);
        protected abstract ChaseTargetAction GetChaseTargetAction(Transform target, float chaseSpeed, float stoppingDinstance, bool completeOnArrival);
        protected abstract GoAroundTargetAction GetGoAroundTargetAction(Vector3 targetPoint, float speed, bool curDistanceIsOrbitDistance, float orbitDistance, float angle);
        protected abstract GoThroughPointAction GetGoThroughPointAction(Vector3 targetPoint, GoThroughPointAction.GoThroughTargetType motionType, float distanceAfterTarget, float speed, bool canDashThroughObstacles, LayerMask obstaclesLayerMask);
        protected abstract BodyLookAtTargetAction GetBodyLookAtTargetAction(Transform target, Vector3 point, float lookSpeed, bool completeOnAlign);

        /// <summary>
        /// Start an action in the category with the given ID. If that category doesn't exist, a new one will be created, if it does and it already has an active action, it will be interrupted.
        /// </summary>
        /// <param name="actionCategory">The category that this action should be started in.</param>
        /// <param name="motorAction">The motor action to start.</param>
        /// <returns>The ID of the action. Can be used later to interrupt it. -1 if the action didn't start.</returns>
        public int StartAction(ActionCategories actionCategory, MotorAction motorAction, Action onActionCompletion, Action onActionInterruption)
        {
            MotorActionCategory category;
            int actionCategoryID = (int)actionCategory;

            if(!motorActionsByCategory.TryGetValue(actionCategoryID, out category))
            {
                category = new MotorActionCategory();

                motorActionsByCategory.Add(actionCategoryID, category);
                allMotorActionCategories.Add(category);
            }

            int actionID = category.StartAction(motorAction, onActionCompletion, onActionInterruption);

            return actionID;
        }
        /// <summary>
        /// Checks if the category with the given ID can perform actions. Returns false if the category exists and can't act, true if it can act or doesn't exist.
        /// </summary>
        /// <param name="actionCategoryID"></param>
        /// <returns>False if the category exists and can't act, true if it can act or doesn't exist.</returns>
        public bool CanCategoryAct(ActionCategories actionCategory)
        {
            int actionCategoryID = (int)actionCategory;
           
            //If the category doesn't allow for actions, then don't start the action.
            if(motorActionsByCategory.TryGetValue(actionCategoryID, out var category) && !category.CanAct)
                return false;

            return true;
        }
        /// <summary>
        /// Call this before setting up a new action in case the same action you're setting up is already active. 
        /// Interrupts the currently active action in this category if found, returns whether or not a new action can be added to the category.
        /// </summary>
        /// <param name="actionCategoryID"></param>
        /// <returns>Whether or not a new action can be added to the category. True if the category exists and it can act or if it doesn't exist, false if it exists and can't act.</returns>
        public bool PrepareCategoryForNewAction(ActionCategories actionCategory)
        {
            int actionCategoryID = (int)actionCategory;
         
            //If the category exist, then see if it can act, and if it does, interrupt it's currently active action.
            if(motorActionsByCategory.TryGetValue(actionCategoryID, out var category))
            {
                if(!category.CanAct)
                    return false;
                else
                {
                    category.InterruptActiveAction();
                    return true;
                }
            }
            else
                return true;
        }
        /// <summary>
        /// Interrupt this category's current active action if any.
        /// </summary>
        /// <param name="actionCategoryID"></param>
        public void InterruptCategoryActiveAction(ActionCategories actionCategory)
        {
            int actionCategoryID = (int)actionCategory;
       
            if(motorActionsByCategory.TryGetValue(actionCategoryID, out var category))
                category.InterruptActiveAction();
        }
        /// <summary>
        /// Stop the action with the given ID if it's currently active. If the given ID is -1, then stop the active action regardless of its ID.
        /// </summary>
        /// <param name="actionID">The ID of the action to stop. If -1, then stop the active action regardless of its ID.</param>
        public void StopAction(int actionID, ActionCategories actionCategory)
        {
            int actionCategoryID = (int)actionCategory;

            if(motorActionsByCategory.TryGetValue(actionCategoryID, out var category))
                category.InterruptActiveAction(actionID);
        }

        #region Movement Actions
        /// <summary>
        /// Attempts to go to the desired destination.
        /// </summary>
        /// <param name="destination">The destination point to go to.</param>
        /// <returns>The ID of the started action if it did. -1 if it didn't.</returns>
        public int GoToPoint(Vector3 destination, Action onComplete, Action onInterruption)
        {
            //If the category doesn't allow for actions, then don't start the action.
            if(!PrepareCategoryForNewAction(ActionCategories.MainMovement))
            {
                Debug.LogError($"Can't start new action {gameObject}", gameObject);
                return -1;
            }

            return StartAction(ActionCategories.MainMovement, GetGoToPointAction(destination), onComplete, onInterruption);
        }
        /// <summary>
        /// Attempts to start chasing the desired target.
        /// </summary>
        /// <param name="target">The target to chase.</param>
        /// <param name="chaseSpeed">The speed of which the motor shoud chase its target. If a negative value is set, the default value will be used.</param>
        /// <param name="stoppingDistance">When should the motor stop.</param>
        /// <param name="completeOnArrival">Should the action be completed once the motor reaches the target? if false, it will keep chasing until interrupted.</param>
        /// <param name="onComplete"></param>
        /// <param name="onInterruption"></param>
        /// <returns>The ID of the started action if it did. -1 if it didn't.</returns>
        public int ChaseTarget(Transform target, float chaseSpeed, float stoppingDistance, bool completeOnArrival, Action onComplete, Action onInterruption)
        {
            //If the category doesn't allow for actions, then don't start the action.
            if (!PrepareCategoryForNewAction(ActionCategories.MainMovement))
                return -1;

            return StartAction(ActionCategories.MainMovement, GetChaseTargetAction(target, chaseSpeed, stoppingDistance, completeOnArrival), onComplete, onInterruption);
        }
        /// <summary>
        /// Attempts to go to a point around the given target.
        /// </summary>
        /// <param name="targetPoint">The point to go around.</param>
        /// <param name="speed">The movement speed.</param>
        /// <param name="curDistanceIsOrbitDistance">Use the current distance between the motor and the target point as the orbit distance.</param>
        /// <param name="orbitDistance">Only used if curDistanceIsOrbitDistance is set to false. The distance between the target and the point around it (circle radius).</param>
        /// <param name="angle">The angle difference between the current angle from motor to target, and the final point to target angle (ex. move 10 or -10 degrees around target)</param>
        /// <param name="onComplete"></param>
        /// <param name="onInterruption"></param>
        public int GoToPointAroundTarget(Vector3 targetPoint, float speed, bool curDistanceIsOrbitDistance, float orbitDistance, float angle, Action onComplete, Action onInterruption)
        {
            //If the category doesn't allow for actions, then don't start the action.
            if (!PrepareCategoryForNewAction(ActionCategories.MainMovement))
                return -1;
            
            return StartAction(ActionCategories.MainMovement, GetGoAroundTargetAction(targetPoint, speed, curDistanceIsOrbitDistance, orbitDistance, angle), onComplete, onInterruption);
        }
        /// <summary>
        /// Attempts to through a point.
        /// </summary>
        /// <param name="targetPoint">The position to go through.</param>
        /// <param name="motionType">If dash, the motor will dash through the object not worrying about colliding with it. Ex: NavMeshAgent will be disabled during dashes.</param>
        /// <param name="distanceAfterTarget">The distance to move beyond the target.</param>
        /// <param name="speed">The speed of which the character will dash if it's enabled.</param>
        /// <param name="canDashThroughObstacles">If motion type is dashing, should the motor be able to dash through obstacles (colliders)? If not, this action will complete without dashing if an obstacle is in the way.</param>
        /// <param name="obstaclesLayerMask">The layer mask of the dashing obstacles.</param>
        /// <param name="onComplete"></param>
        /// <param name="onInterruption"></param>
        public int GoThroughPoint(Vector3 targetPoint, GoThroughPointAction.GoThroughTargetType motionType, float distanceAfterTarget, float speed, bool canDashThroughObstacles, LayerMask obstaclesLayerMask, Action onComplete, Action onInterruption)
        {
            //If the category doesn't allow for actions, then don't start the action.
            if (!PrepareCategoryForNewAction(ActionCategories.MainMovement))
                return -1;

            return StartAction(ActionCategories.MainMovement, GetGoThroughPointAction(targetPoint, motionType, distanceAfterTarget, speed, canDashThroughObstacles, obstaclesLayerMask), onComplete, onInterruption);
        }
        #endregion

        #region Rotation Actions
        /// <summary>
        /// Attempts to start looking at target.
        /// </summary>
        /// <param name="target">The target transform to keep looking at.</param>
        /// <param name="lookSpeed">The speed of which the motor will look at it's target, use this to smooth the rotation. 0 or less means no smoothing.</param>
        /// <param name="completeOnAlign"></param>
        /// <param name="onComplete"></param>
        /// <param name="onInterruption"></param>
        /// <returns></returns>
        public int BodyLookAtTarget(Transform target, float lookSpeed, bool completeOnAlign, Action onComplete, Action onInterruption)
        {
            //If the category doesn't allow for actions, then don't start the action.
            if (!PrepareCategoryForNewAction(ActionCategories.MainRotation))
                return -1;

            return StartAction(ActionCategories.MainRotation, GetBodyLookAtTargetAction(target, Vector3.zero, lookSpeed, completeOnAlign), onComplete, onInterruption);
        }
        /// <summary>
        /// Attempts to start looking at a point.
        /// </summary>
        /// <param name="point">The target to point to look at.</param>
        /// <param name="lookSpeed">The speed of which the motor will look at it's target, use this to smooth the rotation. 0 or less means no smoothing.</param>
        /// <param name="completeOnAlign"></param>
        /// <param name="onComplete"></param>
        /// <param name="onInterruption"></param>
        /// <returns></returns>
        public int BodyLookAtTarget(Vector3 point, float lookSpeed, bool completeOnAlign, Action onComplete, Action onInterruption)
        {
            //If the category doesn't allow for actions, then don't start the action.
            if (!PrepareCategoryForNewAction(ActionCategories.MainRotation))
                return -1;

            return StartAction(ActionCategories.MainRotation, GetBodyLookAtTargetAction(null, point, lookSpeed, completeOnAlign), onComplete, onInterruption);
        }
        #endregion
    }
}