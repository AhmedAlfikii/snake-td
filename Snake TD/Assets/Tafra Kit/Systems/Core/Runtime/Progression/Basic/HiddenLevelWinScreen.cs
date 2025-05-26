using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZUI;

namespace TafraKit
{
    public class HiddenLevelWinScreen : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private float nextLevelDelay = 1f;
        #endregion

        #region MonoBehaviour Messages
        void OnEnable()
        {
            BasicProgressManager.OnLevelCompleted.AddListener(OnLevelCompleted);
        }

        void OnDisable()
        {
            BasicProgressManager.OnLevelCompleted.RemoveListener(OnLevelCompleted);
        }
        #endregion

        #region Callbacks
        void OnLevelCompleted()
        {
            CompactCouroutines.StartCompactCoroutine(this, nextLevelDelay, 0, false, null, () =>
            {
                BasicProgressManager.LoadNextLevel();
            }, null);
        }
        #endregion
    }
}