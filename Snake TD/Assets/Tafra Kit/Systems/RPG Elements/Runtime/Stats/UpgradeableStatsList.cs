using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.CharacterCustomization;
using TafraKit.ContentManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.RPG
{
    [CreateAssetMenu(menuName = "Tafra Kit/RPG/Stats/Upgradeable Stats List", fileName = "Upgradeable Stats List")]
    public class UpgradeableStatsList : IdentifiableScriptableObject, IStatValuesList
    {
        [SerializeField] protected TafraAsset<ScriptableFloat> level;
        [SerializeField] protected List<StatValue> stats;

        [NonSerialized] protected ScriptableFloat levelSF;

        public int Level
        {
            get
            {
                return levelSF.ValueInt;
            }
            set
            {
                levelSF.Set(value);
            }
        }

        public List<StatValue> StatValues => stats;

        protected override void OnEnable()
        {
            base.OnEnable();

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
         
            level.Release();

            if (levelSF != null)
                levelSF.OnValueChange.RemoveListener(OnLevelChange);
        }
        protected virtual void OnLevelChange(float value)
        {
            UpdateStatsLevel();
        }

        protected void UpdateStatsLevel()
        {
            int level = 1;

            if(levelSF != null)
                level = levelSF.ValueInt;

            for (int i = 0; i < stats.Count; i++)
            {
                stats[i].SetLevel(level);
            }
        }

        public void InitializeStatValues()
        {
            if(level.HasReference)
            {
                levelSF = level.Load();

                levelSF.OnValueChange.AddListener(OnLevelChange);
            }

            UpdateStatsLevel();
        }
    }
}