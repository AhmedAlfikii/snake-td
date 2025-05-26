using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TafraKit.SceneManagement;
using ZUI;

namespace TafraKit
{
    public class LevelLoseScreen : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private UIElementsGroup myUIEG;
        [SerializeField] private Button replayBTN;
        [SerializeField] private SceneTransition sceneTransition;
        #endregion

        #region MonoBehaviour Messages
        void OnEnable()
        {
            BasicProgressManager.OnLevelFailed.AddListener(OnLevelFailed);

            if (replayBTN)
                replayBTN.onClick.AddListener(Replay);
        }

        void OnDisable()
        {
            BasicProgressManager.OnLevelFailed.RemoveListener(OnLevelFailed);

            if (replayBTN)
                replayBTN.onClick.RemoveListener(Replay);
        }
        #endregion

        #region Callbacks
        void OnLevelFailed()
        {
            if (replayBTN)
                replayBTN.interactable = true;

            myUIEG.ChangeVisibility(true);
        }
        #endregion

        #region Public Functions
        public void Replay()
        {
            BasicProgressManager.ReplayCurrentLevel();

            if (replayBTN)
                replayBTN.interactable = false;

            myUIEG.ChangeVisibility(false);
        }
        #endregion
    }
}