using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TafraKit.RPG
{
    public static class LinearUpgrades
    {
        private static LinearUpgradesSettings settings;
        private static MainLinearUpgradePath mainUpgradePath;
        private static List<SecondaryLinearUpgradePath> secondaryUpgradePaths = new List<SecondaryLinearUpgradePath>();
        private static int mainPathUpgradesMade;
        private static List<int> upgradesMadePerSecondaryPath = new List<int>();
        private static bool isFirstSceneLoad = true;

        public static MainLinearUpgradePath MainUpgradePath => mainUpgradePath;
        public static List<SecondaryLinearUpgradePath> SecondaryUpgradePaths => secondaryUpgradePaths;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<LinearUpgradesSettings>();

            if(settings == null || !settings.Enabled)
                return;

            mainUpgradePath = settings.MainUpgradePath;
            secondaryUpgradePaths = settings.SecondaryUpgradePaths;

            mainUpgradePath.Initialize();
            for (int i = 0; i < secondaryUpgradePaths.Count; i++)
            {
                secondaryUpgradePaths[i].Initialize();
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            GeneralCoroutinePlayer.StartCoroutine(LateOnSceneLoaded(scene, loadMode));
        }
        private static IEnumerator LateOnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            //We're only concerned about full scene loads.
            if(loadMode == LoadSceneMode.Additive)
                yield break;

            //The first frame of the game is always 0. And in frame 0, WaitForEndOfFrame actually skips a frame then waits for the end of that new frame.
            //So, to guarantee that the upgrades will be applied at the end of frame 1, we will skip frame 0.
            if(Time.frameCount == 0)
                yield return null;

            //We want to wait for the end of the frame to make sure that the player equipment were equipped, since this happens on start, and some upgrades could depend on equipment.
            yield return Yielders.EndOfFrame;

            if(!isFirstSceneLoad)
            {
                mainUpgradePath.SceneLoaded();

                for(int i = 0; i < secondaryUpgradePaths.Count; i++)
                {
                    var path = secondaryUpgradePaths[i];

                    path.SceneLoaded();
                }
            }

            isFirstSceneLoad = false;

            //Initialize the groups if this is the first time a scene load occurs.
            //if(!initializedGroups)
            //{
            //    for(int i = 0; i < perkGroups.Count; i++)
            //    {
            //        var group = perkGroups[i];

            //        if(group == null)
            //            continue;

            //        group.OnPerkApplied.AddListener(OnPerkApply);
            //        group.OnPerkReapplied.AddListener(OnPerkReapply);
            //        group.OnPerkAppliedFirstTimeInSession.AddListener(OnPerkApplyFirstTimeInSession);
            //        group.OnPerkAppliedFirstTimeEver.AddListener(OnPerkApplyFirstTimeEver);

            //        group.Initialize();
            //    }

            //    initializedGroups = true;
            //}
            //else
            //{
            //    //Inform all the groups that a new scene has been loaded so that they can inform their applied perks.
            //    //It's important to do this only if the groups were initialized before, since we don't want to double apply their perks (by loading them and by telling it that the a scene was loaded).
            //    for(int i = 0; i < perkGroups.Count; i++)
            //    {
            //        var group = perkGroups[i];

            //        if(group == null)
            //            continue;

            //        group.SceneLoaded();
            //    }
            //}
        }

    }
}