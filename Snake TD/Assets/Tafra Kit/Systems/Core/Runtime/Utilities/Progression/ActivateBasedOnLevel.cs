using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    /// <summary>
    /// Activates a game object based on the current level number.
    /// Usage note: make sure that all of the game objects are disabled by default.
    /// </summary>
    public class ActivateBasedOnLevel : MonoBehaviour
    {
        #region Classes, Structs & Enums
        [Serializable]
        public class Data
        {
            public int levelNumber;
            public GameObject target;
        }
        #endregion

        #region Private Serialized Fields
        [SerializeField] private Data[] activationElements;
        #endregion

        #region MonoBehaviour Messages
        void Awake()
        {
            for (int i = 0; i < activationElements.Length; i++)
            {
                if (activationElements[i].levelNumber == BasicProgressManager.OpenedLevelNumber)
                    activationElements[i].target.SetActive(true);
            }
        }
        #endregion
    }
}