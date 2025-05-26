using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TafraKit.SceneManagement;
using ZUI;

namespace TafraKit
{
    public class HiddenLevelLoseScreen : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private float replayDelay = 1f;
        #endregion

        #region MonoBehaviour Messages
        void OnEnable()
        {
            BasicProgressManager.OnLevelFailed.AddListener(OnLevelFailed);
        }

        void OnDisable()
        {
            BasicProgressManager.OnLevelFailed.RemoveListener(OnLevelFailed);
        }
        #endregion

        #region Callbacks
        void OnLevelFailed()
        {
            CompactCouroutines.StartCompactCoroutine(this, replayDelay, 0, false, null, () =>
            {
                BasicProgressManager.ReplayCurrentLevel();
            }, null);
        }
        #endregion
    }
}