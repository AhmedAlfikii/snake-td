using System;
using UnityEngine;
using TafraKit.Internal;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit
{
    [CreateAssetMenu(menuName = "Tafra Kit/Progression/Level/Game Level", fileName = "Game Level")]
    public class GameLevel : GameLevelBase, IResettable
    {
        [Tooltip("The name of the scene that should be loaded when this level starts.")]
        [SerializeField] private TafraString sceneName;

        [NonSerialized] protected bool isInitialized;
        [NonSerialized] private int clearsCount;

        [NonSerialized] private string clearsCountSaveKey;

        public string SceneName => sceneName.Value;
        /// <summary>
        /// Whether or not the player has cleared this level at least once.
        /// </summary>
        public bool IsCleared => clearsCount > 0;
        /// <summary>
        /// The number of times the player has won this level.
        /// </summary>
        public int ClearsCount {
            get 
            { 
                return clearsCount;
            }
            set
            {
                if (clearsCount == value)
                    return;

                clearsCount = value;

                TafraSaveSystem.SaveInt(clearsCountSaveKey, clearsCount);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            #if UNITY_EDITOR
            if(!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

            if (!isInitialized)
                Initialize();
        }

        protected virtual void LoadSavedData()
        {
            clearsCount = TafraSaveSystem.LoadInt(clearsCountSaveKey, 0);
        }

        public void Initialize()
        {
            if (isInitialized)
                return;
            
            clearsCountSaveKey = $"GAME_LEVEL_{ID}_CLEARS_COUNT";

            LoadSavedData();

            OnInitialize();

            isInitialized = true;
        }
        public void ResetSavedData()
        {
            OnResetSavedData();
        }


        protected virtual void OnInitialize() { }
        protected virtual void OnResetSavedData() { }
    }
}