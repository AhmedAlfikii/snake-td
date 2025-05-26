using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZUI;

namespace TafraKit
{
    public class LevelWinScreen : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private UIElementsGroup myUIEG;
        [SerializeField] private Button nextBTN;
        [SerializeField] private Button replayBTN;
        #endregion

        #region MonoBehaviour Messages
        void OnEnable()
        {
            BasicProgressManager.OnLevelCompleted.AddListener(OnLevelCompleted);

            if (nextBTN)
                nextBTN.onClick.AddListener(Next);
            if (replayBTN)
                replayBTN.onClick.AddListener(Replay);
        }

        void OnDisable()
        {
            BasicProgressManager.OnLevelCompleted.RemoveListener(OnLevelCompleted);

            if (nextBTN)
                nextBTN.onClick.RemoveListener(Next);
            if (replayBTN)
                replayBTN.onClick.RemoveListener(Replay);
        }
        #endregion

        #region Callbacks
        void OnLevelCompleted()
        {
            if (nextBTN)
                nextBTN.interactable = true;
            if (replayBTN)
                replayBTN.interactable = true;

            myUIEG.ChangeVisibility(true);
        }
        #endregion

        #region Public Functions
        public void Next()
        {
            BasicProgressManager.LoadNextLevel();

            if (nextBTN)
                nextBTN.interactable = false;
            if (replayBTN)
                replayBTN.interactable = false;

            myUIEG.ChangeVisibility(false);
        }

        public void Replay()
        {
            BasicProgressManager.ReplayCurrentLevel();

            if (nextBTN)
                nextBTN.interactable = false;
            if (replayBTN)
                replayBTN.interactable = false;

            myUIEG.ChangeVisibility(false);
        }
        #endregion
    }
}