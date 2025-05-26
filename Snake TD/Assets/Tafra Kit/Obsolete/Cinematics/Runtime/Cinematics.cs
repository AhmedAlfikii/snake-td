using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZUI;

namespace TafraKit
{
    public class Cinematics
    {
        /// <summary>
        /// Fires whenever a cinematic starts, holds whether or not this action was instant (if there's a cinematic already in play, this won't fire if another cinematic started).
        /// </summary>
        public static UnityEvent<bool> OnCinematicStart = new UnityEvent<bool>();
        /// <summary>
        /// Fires whenever the cinematic ends, holds whether or not this action was instant (this won't fire unless the scene that ended was the only active scene).
        /// </summary>
        public static UnityEvent<bool> OnCinematicEnd = new UnityEvent<bool>();

        private static CinematicsSettings settings;
        
        private const string inputDisablerId = "cinematics";

        private static GameObject bars;
        private static UIElementsGroup barsUIEG;
        private static bool playingCinematic;
        private static ControlReceiver cinematicScenes = new ControlReceiver(OnFirstCinematicSceneStarted, OnCinematicSceneStarted, OnAllCinematicScenesEnded);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<CinematicsSettings>();

            if(!settings || !settings.Enabled) return;

            GameObject barsPrefab = Resources.Load<GameObject>("Cinematics/CinematicBars_Canvas");
            bars = GameObject.Instantiate(barsPrefab);
            GameObject.DontDestroyOnLoad(bars);
            barsUIEG = bars.GetComponentInChildren<UIElementsGroup>();
        }

        #region Callbacks
        private static void OnFirstCinematicSceneStarted()
        {

        }
        private static void OnCinematicSceneStarted(string scene)
        {

        }
        private static void OnAllCinematicScenesEnded()
        {

        }
        #endregion

        private static void StartCinematic(bool instant)
        {
            if(playingCinematic)
                return;

            playingCinematic = true;

            if(!instant)
                barsUIEG.ChangeVisibility(true);
            else
                barsUIEG.ChangeVisibilityImmediate(true);

            if(settings.DisableInputsDuringCinematics)
                InputTransmitter.AddInputDisabler(inputDisablerId);

            OnCinematicStart?.Invoke(instant);
        }
        private static void EndCinematic(bool instant)
        {
            if(!playingCinematic)
                return;

            playingCinematic = false;

            if(!instant)
                barsUIEG.ChangeVisibility(false);
            else
                barsUIEG.ChangeVisibilityImmediate(false);

            if(settings.DisableInputsDuringCinematics)
                InputTransmitter.RemoveInputDisabler(inputDisablerId);

            OnCinematicEnd?.Invoke(instant);
        }

        public static void StartCinematicScene(string sceneName)
        {
            if(!playingCinematic)
                StartCinematic(false);

            cinematicScenes.AddController(sceneName);
        }
        public static void StartCinematicSceneInstant(string sceneName)
        {
            if(!playingCinematic)
                StartCinematic(true);

            cinematicScenes.AddController(sceneName);
        }
        public static void EndCinematicScene(string sceneName)
        {
            cinematicScenes.RemoveController(sceneName);

            if(!cinematicScenes.HasAnyController())
                EndCinematic(false);
        }
        public static void EndCinematicSceneInstant(string sceneName)
        {
            cinematicScenes.RemoveController(sceneName);

            if(!cinematicScenes.HasAnyController())
                EndCinematic(true);
        }
        public static bool IsCinematicScenePlaying(string sceneName)
        {
            return playingCinematic && cinematicScenes.IsAController(sceneName);
        }
        public static bool IsCinematicPlaying()
        {
            return playingCinematic;
        }
    }
}