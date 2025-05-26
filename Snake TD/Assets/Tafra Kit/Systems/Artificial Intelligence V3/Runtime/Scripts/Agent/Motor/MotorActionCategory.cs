using System;
using UnityEngine;

namespace TafraKit.AI3.Motor
{
    public class MotorActionCategory
    {
        private ControlReceiver blockers;
        private bool canAct;
        private MotorAction activeAction;
        private int activeActionID;
        private Action onActiveActionCompleted;
        private Action onActiveActionInterrupted;

        public MotorAction ActiveAction => activeAction;
        public bool CanAct => canAct;

        public MotorActionCategory()
        {
            canAct = true;
            blockers = new ControlReceiver(OnFirstBlockerAdded, null, OnAllBlockersCleared);
        }

        private void OnFirstBlockerAdded()
        {
            canAct = false;
        }
        private void OnAllBlockersCleared()
        {
            canAct = true;
        }
        private void OnActiveActionCompleted()
        {
            onActiveActionCompleted?.Invoke();

            activeAction = null;
            activeActionID = -1;
        }

        /// <summary>
        /// Start this action, if an action is already running, interrupt it.
        /// </summary>
        /// <param name="motorAction">The action that needs to be started.</param>
        /// <returns>The ID of the action. Can be used later to interrupt it. -1 if the action didn't start.</returns>
        public int StartAction(MotorAction motorAction, Action onComplete, Action onInterrupt)
        {
            if(!canAct)
                return -1;

            //Signal the interruption of the previous action if it was still active.
            if(activeAction != null)
                InterruptActiveAction();

            onActiveActionCompleted = onComplete;
            onActiveActionInterrupted = onInterrupt;

            activeAction = motorAction;
            activeAction.OnCompletion = OnActiveActionCompleted;

            activeActionID++;

            activeAction.OnStart();

            //There's a possibility that activeActionID could be -1 here. Which means that the active action was completed on start.

            return activeActionID;
        }
        /// <summary>
        /// Interrupt the currently active action if any.
        /// </summary>
        /// <param name="actionID">If the active action has this ID, then interrupt it. If -1, interrupt the active action regardless of what ID it has.</param>
        public void InterruptActiveAction(int actionID = -1)
        {
            if(activeAction == null || (actionID != -1 && activeActionID != actionID))
                return;

            activeAction.Interrupt();

            onActiveActionInterrupted?.Invoke();

            activeAction.OnCompletion = null;
            activeAction = null;
            activeActionID = -1;
        }
        /// <summary>
        /// Add a blocker that would prevent any future action from being activated and would interrupt the currently playing action.
        /// </summary>
        /// <param name="blockerID"></param>
        public void AddActionBlocker(string blockerID)
        {
            //If there's a currently active action, interrupt it.
            if(activeAction != null)
                InterruptActiveAction();

            blockers.AddController(blockerID);
        }
        /// <summary>
        /// Remove the blocker to allow for new actions to be activated.
        /// </summary>
        /// <param name="blockerID"></param>
        public void RemoveActionBlocker(string blockerID)
        {
            blockers.RemoveController(blockerID);
        }
    }
}