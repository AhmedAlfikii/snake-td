using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TafraKit;

namespace TafraKit.Demos
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelTitle;

        private void Awake()
        {
            levelTitle.text = "Level " + BasicProgressManager.OpenedLevelNumber;
        }

        #region Public Functions
        public void CompleteLevel()
        {
            BasicProgressManager.CompleteCurrentLevel();
        }

        public void FailLevel()
        {
            BasicProgressManager.FailCurrentLevel();
        }
        #endregion
    }
}