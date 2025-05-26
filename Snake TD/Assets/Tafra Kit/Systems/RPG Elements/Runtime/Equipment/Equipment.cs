using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.CharacterCustomization;
using TafraKit.ContentManagement;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.RPG
{
    [CreateAssetMenu(menuName = "Tafra Kit/RPG/Equipment", fileName = "Equipment")]
    public class Equipment : StorableScriptableObject, IStatValuesList
    {
        [Serializable]
        public class RarityStats
        {
            [Tooltip("Serves no purpose but to nicefy the array element names.")]
            [SerializeField] private string rarityName;
            [SerializeField] private List<StatValue> statValues;

            public List<StatValue> StatValues => statValues;
        }

        [SerializeField] protected List<RarityStats> statsPerRarity;

        [Header("LODs")]
        [Tooltip("An equipment could either have a skin or a GO. A customization skin needs another system to apply it (e.g. a customizable character script).")]
        [SerializeField] private List<TafraAsset<MeshCustomization>> skinLODS;
        [Tooltip("An equipment could either have a skin or a GO. GO would be automatically instantiated under the equipper's game object.")]
        [SerializeField] protected List<TafraAsset<GameObject>> goLODS;

        [NonSerialized] protected int level = 1;
        [NonSerialized] protected int rarity = 0;
        [NonSerialized] protected GameObject equipmentInstanceGO;
        [NonSerialized] protected UnityEvent<int> onLevelChanged = new UnityEvent<int>();
        [NonSerialized] protected UnityEvent<int> onRarityChanged = new UnityEvent<int>();

        public int Level 
        {
            get
            {
                return level;
            }
            set
            {
                if (level == value)
                    return;

                level = value;
                OnLevelChange();
                onLevelChanged?.Invoke(level);
            }
        }
        public int Rarity 
        {
            get
            {
                return rarity;
            }
            set
            {
                rarity = value;
                OnRarityChange();
                onRarityChanged?.Invoke(rarity);
            }
        }

        public List<StatValue> StatValues
        {
            get 
            {
                if (rarity >= statsPerRarity.Count)
                    return null;

                    return statsPerRarity[rarity].StatValues; 
            }
        }
        public List<RarityStats> StatsPerRarity => statsPerRarity;
        public GameObject EquipmentInstanceGO
        {
            get 
            {
                return equipmentInstanceGO;
            }
            set
            {
                equipmentInstanceGO = value;
            }
        }
        public UnityEvent<int> OnLevelChanged => onLevelChanged;
        public UnityEvent<int> OnRarityChanged => onRarityChanged;

        protected override void OnEnable()
        {
            base.OnEnable();

            #if UNITY_EDITOR
            if(!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

            Initialize();
        }
        private void OnDestroy()
        {
            ReleaseLoadedObjects();
        }
        protected virtual void Initialize()
        {
            UpdateStatsLevel();
        }

        protected virtual void OnLevelChange()
        {
            UpdateStatsLevel();
            Save();
        }
        protected virtual void OnRarityChange()
        {
            UpdateStatsLevel();
            Save();
        }

        public void UpdateStatsLevel()
        {
            int curRarityLevel = rarity;

            if (curRarityLevel >= statsPerRarity.Count)
                curRarityLevel = statsPerRarity.Count - 1;

            if (curRarityLevel < 0)
                return;

            var rarityStats = statsPerRarity[rarity];

            for (int i = 0; i < rarityStats.StatValues.Count; i++)
            {
                var stat = rarityStats.StatValues[i];

                stat.SetLevel(level);
            }
        }
        public override void Save()
        {
            base.Save();

            PlayerPrefs.SetInt($"EQUIPMENT_{InstancableSO.GetSOInstanceID()}_LEVEL", level);
            PlayerPrefs.SetInt($"EQUIPMENT_{InstancableSO.GetSOInstanceID()}_RARITY", rarity);
        }
        public override void Load()
        {
            base.Load();

            level = PlayerPrefs.GetInt($"EQUIPMENT_{InstancableSO.GetSOInstanceID()}_LEVEL", level);
            rarity = PlayerPrefs.GetInt($"EQUIPMENT_{InstancableSO.GetSOInstanceID()}_RARITY", rarity);

            UpdateStatsLevel();
        }
        public override void ClearSavedKeys()
        {
            base.ClearSavedKeys();

            PlayerPrefs.DeleteKey($"EQUIPMENT_{InstancableSO.GetSOInstanceID()}_LEVEL");
            PlayerPrefs.DeleteKey($"EQUIPMENT_{InstancableSO.GetSOInstanceID()}_RARITY");
        }
        public void ReleaseLoadedObjects()
        {
            for(int i = 0; i < skinLODS.Count; i++)
            {
                skinLODS[i].Release();
            }
            for(int i = 0; i < goLODS.Count; i++)
            {
                goLODS[i].Release();
            }
        }

        public bool HasSkin(int lods)
        {
            if(skinLODS.Count <= lods)
                return false;

            return skinLODS[lods].HasReference;
        }
        public bool HasGO(int lods)
        {
            if(goLODS.Count <= lods)
                return false;

            return goLODS[lods].HasReference;
        }

        public MeshCustomization LoadSkin(int lods)
        {
            if(lods < skinLODS.Count)
            {
                return skinLODS[lods].Load();
            }
            else if(skinLODS.Count > 0)
                return skinLODS[skinLODS.Count - 1].Load();
            else
                return null;
        }
        public GameObject LoadGO(int lods)
        {
            if(lods < goLODS.Count)
                return goLODS[lods].Load();
            else if (goLODS.Count > 0)
                return goLODS[goLODS.Count - 1].Load();
            else
                return null;
        }

        public void InitializeStatValues()
        {

        }
    }
}