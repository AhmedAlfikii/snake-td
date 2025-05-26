using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    /// <summary>
    /// A class that register controllers. 
    /// </summary>
    public class ControlReceiver
    {
        #region Private Fields
        private HashSet<string> allControllers = new HashSet<string>();
        private Action onFirstControllerAdded;
        private Action<string> onControllerAdded;
        private Action onAllControllersCleared;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onFirstControllerAdded">Once a controller is added while there were no active contorllers.</param>
        /// <param name="onControllerAdded">Once a new controller is added.</param>
        /// <param name="onAllControllersCleared">Once the last controller is removed.</param>
        public ControlReceiver(Action onFirstControllerAdded, Action<string> onControllerAdded, Action onAllControllersCleared)
        {
            this.onFirstControllerAdded = onFirstControllerAdded;
            this.onControllerAdded = onControllerAdded;
            this.onAllControllersCleared = onAllControllersCleared;
        }

        public ControlReceiver()
        {
        }

        #region Public Functions
        /// <summary>
        /// Adds a controller that is tied to a controller.
        /// </summary>
        /// <param name="controllerId"></param>
        public void AddController(string controllerId)
        {
            if(string.IsNullOrEmpty(controllerId))
            {
                TafraDebugger.Log("Control Reciever", "Controller ID can not be null or empty.", TafraDebugger.LogType.Error);
                return;
            }

            if (allControllers.Contains(controllerId))
                return;

            bool isFirstController = allControllers.Count == 0;

            allControllers.Add(controllerId);

            if (isFirstController)
                onFirstControllerAdded?.Invoke();

            onControllerAdded?.Invoke(controllerId);
        }
        public void RemoveController(string controllerId)
        {
            if(!allControllers.Contains(controllerId)) return;

            allControllers.Remove(controllerId);

            if (allControllers.Count == 0)
                onAllControllersCleared?.Invoke();
        }
        public void RemoveAllControllers()
        {
            allControllers.Clear();

            onAllControllersCleared?.Invoke();
        }
        public bool HasAnyController()
        {
            return allControllers.Count > 0;
        }
        public HashSet<string> GetCurrentControllers()
        {
            return allControllers;
        }
        public bool IsAController(string controllerId)
        {
            return allControllers.Contains(controllerId);
        }
        #endregion
    }
}