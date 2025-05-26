using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.CharacterCustomization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.RPG
{
    [CreateAssetMenu(menuName = "Tafra Kit/RPG/Stats/External Stats Container", fileName = "External Stats Container")]
    public class ExternalStatsContainer : ScriptableObject, IStatsContainerProvider
    {
        [SerializeField] private StatsContainer statsContainer;

        [NonSerialized] private bool isInitialized;
        [NonSerialized] private bool isActive;
        [NonSerialized] private int dependants;

        public StatsContainer StatsContainer => statsContainer;

        protected void OnEnable()
        {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

        }
        private void OnDisable()
        {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif
        }

        private void Initialize()
        {
            statsContainer.Initialize(false);
            isInitialized = true;
        }
        private void Activate()
        {
            statsContainer.Activate();
            isActive = true;
        }
        private void Deactivate()
        {
            statsContainer.Deactivate();
            isActive = false;
        }

        public void AddDependant()
        {
            dependants++;

            if(!isInitialized)
                Initialize();

            if(!isActive)
                Activate();
        }
        public void RemoveDependant()
        {
            dependants--;

            if(dependants <= 0 && isActive)
                Deactivate();
        }
    }
}