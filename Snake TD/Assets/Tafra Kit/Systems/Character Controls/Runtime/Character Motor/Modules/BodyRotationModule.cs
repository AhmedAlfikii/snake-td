using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    public abstract class BodyRotationModule : CharacterMotorModule
    {
        public enum LookState
        { 
            LookAtMovementDirection = 0,
            LookAtTargetDirection = 1,
            LookAtLockedTarget = 2,
            LookAtMovementInput = 3
        }

        [Tooltip("While moving, rotate towards the movement direction in this speed.")]
        [SerializeField] protected float movementDirectionLookSpeed = 10;

        protected LookState lookState;
        protected Transform lockedLookAtTarget;
        protected Vector3 targetLookDirection;
        private InfluenceReceiver<Transform> lockedTargetLookAtInfluences;
        private InfluenceReceiver<Vector3> targetLookDirectionInfluences;
        private InfluenceReceiver<int> activeStateInfluences;

        private const string movementDirectionStateID = "movementDirection";
        private const string targetDirectionStateID = "targetDirection";
        private const string lockedTargetStateID = "lockedTarget";

        protected override void OnInitialize()
        {
            activeStateInfluences = new InfluenceReceiver<int>(ShouldReplaceActiveState, OnActiveStateUpdate, null, OnAllActiveStatesCleared);
            activeStateInfluences.AddInfluence(movementDirectionStateID, 0);

            lockedTargetLookAtInfluences = new InfluenceReceiver<Transform>(ShouldReplaceLockedTarget, OnActiveLockedTargetUpdate, null, OnAllLockedTargetsCleared);
            targetLookDirectionInfluences = new InfluenceReceiver<Vector3>(ShouldReplaceTargetLookDirection, OnActiveTargetLookDirectionUpdate, null, OnAllTargetLookDirectionsCleared);
        }

        #region Callbacks
        private bool ShouldReplaceActiveState(int newState, int oldState)
        {
            return newState > oldState;
        }
        private void OnActiveStateUpdate(int state)
        {
            lookState = (LookState)state;
        }
        private void OnAllActiveStatesCleared()
        {
            lookState = LookState.LookAtMovementDirection;
        }

        private bool ShouldReplaceLockedTarget(Transform newInfluence, Transform oldInfluence)
        {
            return true;
        }
        private void OnActiveLockedTargetUpdate(Transform transform)
        {
            lockedLookAtTarget = transform;

            if(lookState == LookState.LookAtLockedTarget)
            {
                OnLockAtTargetChanged();
                return;
            }

            activeStateInfluences.AddInfluence(lockedTargetStateID, 2);

            OnLockAtTarget();
        }
        private void OnAllLockedTargetsCleared()
        {
            lockedLookAtTarget = null;

            activeStateInfluences.RemoveInfluence(lockedTargetStateID);

            OnUnlockTarget();
        }

        private bool ShouldReplaceTargetLookDirection(Vector3 newInfluence, Vector3 oldInfluence)
        {
            return true;
        }
        private void OnActiveTargetLookDirectionUpdate(Vector3 direction)
        {
            targetLookDirection = direction;

            if(lookState == LookState.LookAtTargetDirection || lookState == LookState.LookAtTargetDirection) //Looking at locked target has a higher priority than looking in direction.
                return;

            activeStateInfluences.AddInfluence(targetDirectionStateID, 1);

            OnStartLookingAtDirection();
        }
        private void OnAllTargetLookDirectionsCleared()
        {
            activeStateInfluences.RemoveInfluence(targetDirectionStateID);

            OnStopLookingAtDirection();
        }
        #endregion

        /// <summary>
        /// Keep looking at a target transform until told to unlock.
        /// </summary>
        /// <param name="controllerID">The ID of the controller that orders this operation. Will be later required to unlock the target look at.</param>
        /// <param name="target"></param>
        public void LockAtTarget(string controllerID, Transform target)
        {
            lockedTargetLookAtInfluences.AddInfluence(controllerID, target);
        }
        /// <summary>
        /// Keep looking at a target transform until told to unlock.
        /// </summary>
        /// <param name="controllerID">The ID of the controller that ordered the target lock.</param>
        public void UnlockTarget(string controllerID)
        {
            lockedTargetLookAtInfluences.RemoveInfluence(controllerID);
        }
        
        /// <summary>
        /// Start looking in the given direction until told to stop.
        /// </summary>
        /// <param name="controllerID">The Id of the controller that orders this operation. Will be later required to stop looking at this direction.</param>
        /// <param name="targetLookDirection"></param>
        public void SetLookingInDirection(string controllerID, Vector3 targetLookDirection)
        {
            targetLookDirectionInfluences.AddInfluence(controllerID, targetLookDirection);
        }
        public void UnsetLookingAtDirection(string controllerID)
        {
            targetLookDirectionInfluences.RemoveInfluence(controllerID);
        }

        /// <summary>
        /// Gets called when a locked look at target is added or if it was replaced by another one.
        /// </summary>
        protected virtual void OnLockAtTarget() { }
        /// <summary>
        /// Gets fired if there was a locked look at target, and then it was replaced by a new one (doesn't get called if the target was completely removed).
        /// </summary>
        protected virtual void OnLockAtTargetChanged() { }
        /// <summary>
        /// Gets called if there was a locked look at target, and then it got removed (not replaced by another one).
        /// </summary>
        protected virtual void OnUnlockTarget() { }
        protected virtual void OnStartLookingAtDirection() { }
        protected virtual void OnStopLookingAtDirection() { }
    }
}