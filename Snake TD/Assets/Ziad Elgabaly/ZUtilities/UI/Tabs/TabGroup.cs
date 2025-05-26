using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TafraKit;
using ZUI;

namespace ZUtilities
{
    public class TabGroup : MonoBehaviour
    {
        #region Classes, Structs & Enums
        [System.Serializable]
        public class Tab
        {
            public ZUIElementBase tabUIE;
            public Button tabButton;
        }
        #endregion

        #region Private Serialized Fields
        [SerializeField] protected Tab[] tabs;
        [SerializeField] protected int defaultTabIndex = 0;
        #endregion

        #region Public Events
        public IntUnityEvent OnTabOpened;
        public IntUnityEvent OnTabClosed;
        #endregion

        #region Public Properties
        public int ActiveTabIndex { get; protected set; }
        #endregion

        void Start()
        {
            if (defaultTabIndex >= 0)
                ActiveTabIndex = defaultTabIndex;

            for (int i = 0; i < tabs.Length; i++)
            {
                if (i != ActiveTabIndex)
                {
                    tabs[i].tabUIE.ChangeVisibilityImmediate(false);
                    tabs[i].tabButton.interactable = true;
                }
                else
                {
                    tabs[i].tabUIE.ChangeVisibilityImmediate(true);
                    tabs[i].tabButton.interactable = false;
                }

                int tabIndex = i;

                tabs[i].tabButton.onClick.AddListener(()=> OpenTab(tabIndex));
            }
        }

        public void OpenTab(int tabIndex)
        {
            if (ActiveTabIndex == tabIndex)
                return;

            //Close previous tab.
            tabs[ActiveTabIndex].tabUIE.ChangeVisibility(false);
            tabs[ActiveTabIndex].tabButton.interactable = true;

            OnTabClosed?.Invoke(ActiveTabIndex);

            //Open new tab.
            tabs[tabIndex].tabUIE.ChangeVisibility(true);
            tabs[tabIndex].tabButton.interactable = false;

            OnTabOpened?.Invoke(tabIndex);

            ActiveTabIndex = tabIndex;
        }

        public void OpenTabImmediate(int tabIndex)
        {
            if (ActiveTabIndex == tabIndex)
                return;

            //Close previous tab.
            tabs[ActiveTabIndex].tabUIE.ChangeVisibilityImmediate(false);
            tabs[ActiveTabIndex].tabButton.interactable = true;

            OnTabClosed?.Invoke(ActiveTabIndex);

            //Open new tab.
            tabs[tabIndex].tabUIE.ChangeVisibilityImmediate(true);
            tabs[tabIndex].tabButton.interactable = false;

            OnTabOpened?.Invoke(tabIndex);

            ActiveTabIndex = tabIndex;
        }
    }
}