using System;
using System.Collections.Generic;
using UnityEngine;
using ZUI;

namespace TafraKit.UI
{
    public class NavigationBar : MonoBehaviour
    {
        [System.Serializable]
        public class FeatureScreen
        {
            public string name;
            public UIElementsGroup screenUIEG;
            public Lock featureLock;
            public List<GameObject> lockedGOs;
            public List<GameObject> unlockedGos;

            [Header("Alerting")]
            public UnityEngine.Object alertContainer;
            public UIAlert uiAlert;
        }

        [SerializeField] private SelectableCellChain cellChain;
        [SerializeField] private List<FeatureScreen> screens;

        private void Awake()
        {
            if (cellChain == null)
                cellChain = GetComponent<SelectableCellChain>();

            if(cellChain == null)
                TafraDebugger.Log("Navigation Bar", "There's no \"Cell Chain\" component on the game object, and its not assigned in the inspector reference, the navigation bar will not work.", TafraDebugger.LogType.Error, gameObject);
        }
        private void Start()
        {
            for (int i = 0; i < screens.Count; i++)
            {
                var screen = screens[i];

                bool isUnlocked = screen.featureLock == null || screen.featureLock.IsUnlocked();

                for(int j = 0; j < screen.lockedGOs.Count; j++)
                {
                    screen.lockedGOs[j].SetActive(!isUnlocked);
                }

                for(int j = 0; j < screen.unlockedGos.Count; j++)
                {
                    screen.unlockedGos[j].SetActive(isUnlocked);
                }

                if (isUnlocked && screen.alertContainer != null && screen.uiAlert != null)
                    screen.uiAlert.SetAlertContainer(screen.alertContainer);
            }
        }
        private void OnEnable()
        {
            cellChain.OnCellAboutToBeSelected.AddListener(OnCellAboutToBeSelected);
        }
        private void OnDisable()
        {
            cellChain.OnCellAboutToBeSelected.RemoveListener(OnCellAboutToBeSelected);
        }

        private void OnCellAboutToBeSelected(int cellIndex)
        {
            FeatureScreen targetScreen = screens[cellIndex];

            if(targetScreen.featureLock != null && !targetScreen.featureLock.IsUnlocked())
            {
                FleetingMessages.Show(targetScreen.featureLock.LockedMessage);

                cellChain.SuppressNextCellSelection();

                return;
            }

            if(targetScreen.screenUIEG != null)
                targetScreen.screenUIEG.ChangeVisibility(true);

            //Hide the rest of the screens.
            for (int i = 0; i < screens.Count; i++)
            {
                if(i == cellIndex)
                    continue;

                var screen = screens[i];

                if(screen.screenUIEG != null && screen.screenUIEG.Visible)
                    screen.screenUIEG.ChangeVisibility(false);
            }
        }
    }
}