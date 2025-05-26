using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZUI;
using TMPro;

namespace TafraKit
{
    /// <summary>
    /// Use this to display a full screen loading indicator. Useful when you want to disable player input until an action is made.
    /// </summary>
    public class ScreenOverlayLoading : MonoBehaviour
    {
        private static InfluenceReceiver<int> influenceReceiver = new InfluenceReceiver<int>(ShouldReplace, OnActiveInfluenceUpdated, OnActiveControllerUpdated, OnAllInfluencesCleared);
        private static bool acive;
        private static UIElementsGroup myUIEG;

        //RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            string prefabName = "ScreenOverlayLoading";

            GameObject panel = Resources.Load<GameObject>("Utilities/" + prefabName);

            if(panel)
            {
                panel = Instantiate(panel);
                panel.name = prefabName;
                
                DontDestroyOnLoad(panel);

                int childCount = panel.transform.childCount;

                for(int i = 0; i < childCount; i++)
                {
                    Transform child = panel.transform.GetChild(i);

                    if(child.name == "UIEG")
                        myUIEG = child.GetComponent<UIElementsGroup>();
                }

                if(myUIEG == null)
                    TafraDebugger.Log("Screen Overlay Loading", "Found an overlay panel but couldn't find a UIElementGroup underneath it. Make sure there's a direct child of the prefab with the name \"UIEG\".", TafraDebugger.LogType.Warning);
            }
        }

        private static void OnActiveInfluenceUpdated(int priority)
        {
            if(myUIEG && !myUIEG.Visible)
                myUIEG.ChangeVisibility(true);
        }
        private static void OnActiveControllerUpdated(string controllerId, int priority)
        {
        }
        private static void OnAllInfluencesCleared()
        {
            if (myUIEG)
                myUIEG.ChangeVisibility(false);
        }
        private static bool ShouldReplace(int newPriority, int oldPriority)
        {
            return newPriority < oldPriority;
        }

        #region Public Functions
        public static void SetLoader(string controllerId, int priority)
        {
            influenceReceiver.AddInfluence(controllerId, priority);
        }

        public static void RemoveLoader(string controllerId)
        {
            influenceReceiver.RemoveInfluence(controllerId);
        }

        public static void RemoveAllLoaders()
        {
            influenceReceiver.RemoveAllInfluences();
        }
        #endregion
    }
}
