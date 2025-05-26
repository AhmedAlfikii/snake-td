using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TafraKit;
using TafraKit.CharacterControls;
using TafraKit.RPG;
using TafraKit.Healthies;
using UnityEngine;
using System;
using TafraKit.SceneManagement;
using TafraKit.Internal;

namespace TafraKit
{
    public class SceneReferences : MonoBehaviour
    {
        #region Main Control
        private static SceneReferences activeSceneReferences;
        private static SceneReferencesSettings settings;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
            settings = TafraSettings.GetSettings<SceneReferencesSettings>();

            if(settings == null || !settings.Enabled)
                return;

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if(loadMode == LoadSceneMode.Additive)
                return;

            if(activeSceneReferences != null)
                return;

            GameObject go = new GameObject("Scene References");
            activeSceneReferences = go.AddComponent<SceneReferences>();
        }
        #endregion

        #region Private Fields
        protected static PlayableActor player;
        protected static Healthy playerHealthy;
        protected static CharacterAbilities playerAbilities;
        protected static CharacterCombat playerCombat;
        protected static CharacterEquipment playerEquipment;
        protected static Inventory playerInventory;
        protected static Camera mainCamera;
        protected static OnScreenJoystick joystick;
        #endregion

        #region Public Properties
        public static PlayableActor Player => player;
        public static Healthy PlayerHealthy => playerHealthy;
        public static CharacterAbilities PlayerAbilities => playerAbilities;
        public static CharacterCombat PlayerCombat => playerCombat;
        public static CharacterEquipment PlayerEquipment => playerEquipment;
        public static Inventory PlayerInventory => playerInventory;
        public static Camera MainCamera => mainCamera;
        public static OnScreenJoystick Joystick => joystick;
        #endregion

        #region MonoBehaviour Messages
        protected virtual void Awake()
        {
            activeSceneReferences = this;

            mainCamera = Camera.main;

            player = FindAnyObjectByType<PlayableActor>();
            joystick = FindAnyObjectByType<OnScreenJoystick>();

            if(player != null)
            {
                playerHealthy = player.GetCachedComponent<Healthy>();
                playerAbilities = player.GetCachedComponent<CharacterAbilities>();
                playerCombat = player.GetCachedComponent<CharacterCombat>();
                playerEquipment = player.GetCachedComponent<CharacterEquipment>();
                playerInventory = player.GetCachedComponent<Inventory>();
            }
        }
        #endregion
    }
}