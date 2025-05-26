using System.Collections.Generic;
using TafraKit.Internal;
using TafraKit.Internal.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TafraKit.UI
{
    public static class GenericPopup
    {
        private static bool isEnabled;
        private static List<DynamicPool<GenericPopupObject>> basicPopupPools = new List<DynamicPool<GenericPopupObject>>();
        private static Dictionary<int, GenericPopupObject> basicPopupsStylesLookup = new Dictionary<int, GenericPopupObject>();
        private static GenericPopupSettings settings;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<GenericPopupSettings>();

            if (!settings.Enabled)
                return;

            basicPopupPools.Clear();
            for (int i = 0; i < settings.BasicPopups.Length; i++)
                basicPopupPools.Add(new DynamicPool<GenericPopupObject>());

            SceneManager.sceneLoaded += OnSceneLoaded;

            isEnabled = true;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            GenericPopupObject popupObjectPrefab, popupObjectUnit;

            for (int i = 0; i < basicPopupPools.Count; i++)
            {
                basicPopupPools[i].Uninitialize();

                popupObjectPrefab = settings.BasicPopups[i];

                popupObjectPrefab.gameObject.SetActive(false);

                popupObjectUnit = GameObject.Instantiate(popupObjectPrefab.gameObject).GetComponent<GenericPopupObject>();

                popupObjectPrefab.gameObject.SetActive(true);

                basicPopupPools[i].AddUnit(popupObjectUnit);
                basicPopupPools[i].Initialize();
            }
        }

        public static void ShowBasic(string title, string info, string confirmButtonText = "Ok", string cancelButtonText = "Cancel",
            UnityAction OnConfirmAction = null, UnityAction OnCancelAction = null, int styleIndex = 0)
        {
            if (!isEnabled)
                return;

            if (styleIndex < 0 || styleIndex >= basicPopupPools.Count)
                return;

            if (!basicPopupsStylesLookup.ContainsKey(styleIndex))
                basicPopupsStylesLookup[styleIndex] = basicPopupPools[styleIndex].RequestUnit();
            basicPopupsStylesLookup[styleIndex].Show(title, info, confirmButtonText, cancelButtonText, OnConfirmAction, OnCancelAction);
        }
    }
}