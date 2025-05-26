using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    /// <summary>
    /// A base class that register controllers, prioritizes them, and applies their influence. 
    /// </summary>
    public class InfluenceReceiver<T>
    {
        public delegate bool ShouldReplace(T newInfluence, T oldInfluence);
      
        #region Private Fields
        private Dictionary<string, T> controllerInfluences;
        private string activeControllerId;
        private ShouldReplace shouldReplace;
        private Action<T> onActiveInfluenceUpdated;
        private Action<string, T> onActiveControllerUpdated;
        private Action onAllInfluencesCleared;
        #endregion

        public InfluenceReceiver(ShouldReplace shouldReplace, Action<T> onActiveInfluenceUpdated, Action<string, T> onActiveControllerUpdated, Action onAllInfluencesCleared)
        {
            controllerInfluences = new Dictionary<string, T>();
            activeControllerId = "";
            this.shouldReplace = shouldReplace;
            this.onActiveInfluenceUpdated = onActiveInfluenceUpdated;
            this.onActiveControllerUpdated = onActiveControllerUpdated;
            this.onAllInfluencesCleared = onAllInfluencesCleared;
        }

        #region Public Functions
        /// <summary>
        /// Adds an influence that is tied to a controller.
        /// </summary>
        /// <param name="controllerId"></param>
        /// <param name="influence"></param>
        public void AddInfluence(string controllerId, T influence)
        {
            if(string.IsNullOrEmpty(controllerId))
            {
                TafraDebugger.Log("Influence Reciever", "Controller ID can not be null or empty.", TafraDebugger.LogType.Error);
                return;
            }
            
            //To decide in the next if condition whether or not this controller should be the active one.
            bool shouldTakeControl = false;

            //Update or add the influence to the influences dictionary.
            if (controllerInfluences.ContainsKey(controllerId))
            {
                controllerInfluences[controllerId] = influence;

                shouldTakeControl = true;
            }
            else
                controllerInfluences.Add(controllerId, influence);
            
            //If there's an active controller.
            if(!string.IsNullOrEmpty(activeControllerId))
            {
                //Check if the new influence should replace the active controller's current influence.
                if(shouldReplace(influence, controllerInfluences[activeControllerId]))
                    shouldTakeControl = true;
            }
            //If there's no active controller, then this one should be the active controller.
            else
                shouldTakeControl = true;

            if(shouldTakeControl)
            {
                if(activeControllerId != controllerId)
                {
                    activeControllerId = controllerId;

                    onActiveControllerUpdated?.Invoke(controllerId, influence);
                }

                onActiveInfluenceUpdated?.Invoke(influence);
            }
        }
        public void RemoveInfluence(string controllerId)
        {
            if(!controllerInfluences.ContainsKey(controllerId)) return;

            controllerInfluences.Remove(controllerId);

            //If this was the active controller, then look for a new one.
            if(activeControllerId == controllerId)
            {
                if(controllerInfluences.Count > 0)
                {
                    string newMainController = controllerInfluences.Aggregate((l, r) => shouldReplace(l.Value, r.Value) ? l : r).Key;
                    
                    activeControllerId = newMainController;

                    T newInfluence = controllerInfluences[activeControllerId];

                    onActiveControllerUpdated?.Invoke(activeControllerId, newInfluence);
                    onActiveInfluenceUpdated?.Invoke(newInfluence);
                }
                else
                {
                    activeControllerId = "";

                    onAllInfluencesCleared?.Invoke();
                }
            }
        }
        public void RemoveAllInfluences()
        {
            controllerInfluences.Clear();
            activeControllerId = "";

            onAllInfluencesCleared?.Invoke();
        }
        public bool HasAnyInfluence()
        {
            return controllerInfluences.Count > 0;
        }
        public bool IsActiveInfluence(string controllerId)
        {
            return controllerId == activeControllerId;
        }
        #endregion
    }
}