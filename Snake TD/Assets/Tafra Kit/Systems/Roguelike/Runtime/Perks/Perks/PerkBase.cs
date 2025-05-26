using TafraKit.Roguelike;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.Internal.Roguelike
{
    public abstract class PerkBase : ScriptableObject, IResettable
    {
        [SerializeField] protected string id;

        [NonSerialized] protected bool isInitialized;

        public string ID => id;

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if(!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

            if (!isInitialized)
                Initialize();
        }

        public void Initialize()
        {
            if(isInitialized)
                return;

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