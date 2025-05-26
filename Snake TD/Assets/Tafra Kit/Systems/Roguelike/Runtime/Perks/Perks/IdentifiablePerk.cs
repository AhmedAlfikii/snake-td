using System;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Roguelike;
using TafraKit.ContentManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.Internal.Roguelike
{
    public abstract class IdentifiablePerk : Perk
    {
        [Header("Identification")]
        [Tooltip("The display name to display for each level of this perk. Levels that don't have a corresponding element will use the last element in the list.")]
        [SerializeField] private List<string> displayNamePerLevel;
        [Tooltip("The description to display for each level of this perk. Levels that don't have a corresponding element will use the last element in the list.")]
        [SerializeField, TextArea] private List<string> descriptionPerLevel;
        [Tooltip("The icon to display for each level of this perk. Levels that don't have a corresponding element will use the last element in the list.")]
        [SerializeField] private List<TafraAsset<Sprite>> iconPerLevel;

        [NonSerialized] private List<Sprite> loadedIconPerLevel = new List<Sprite>();

        public override string OfferDisplayName
        {
            get
            {
                int index = appliesCount;

                int count = displayNamePerLevel.Count;

                if(index >= count)
                    index = count - 1;

                return displayNamePerLevel[index];
            }
        }
        public override string OfferDescription
        {
            get
            {
                int index = appliesCount;

                int count = descriptionPerLevel.Count;

                if(index >= count)
                    index = count - 1;

                return descriptionPerLevel[index];
            }
        }
        public override string AppliedDisplayName
        {
            get
            {
                int index = appliesCount - 1;

                if(index < 0)
                    index = 0;

                int count = displayNamePerLevel.Count;

                if(index >= count)
                    index = count - 1;

                return displayNamePerLevel[index];
            }
        }
        public override string AppliedDescription
        {
            get
            {
                int index = appliesCount - 1;

                if(index < 0)
                    index = 0;

                int count = descriptionPerLevel.Count;

                if(index >= count)
                    index = count - 1;

                return descriptionPerLevel[index];
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            for(int i = 0; i < iconPerLevel.Count; i++)
            {
                iconRequesters = 0;
                loadedIconPerLevel.Add(null);
            }
        }

        public override void LoadIcons()
        {
            if(iconRequesters == 0)
            {
                for(int i = 0; i < iconPerLevel.Count; i++)
                {
                    loadedIconPerLevel[i] = iconPerLevel[i].Load();
                }
            }

            base.LoadIcons();
        }
        public override Sprite GetLoadedAppliedIcon()
        {
            if(!iconsLoaded)
                TafraDebugger.Log("Perk", "Icons are not loaded, load them first by calling LoadIcons(). And make sure to release them when you no longer need them by calling ReleaseIcons()", TafraDebugger.LogType.Error);

            int index = appliesCount - 1;

            if(index < 0)
                index = 0;

            int count = loadedIconPerLevel.Count;

            if(index >= count)
                index = count - 1;
            
            return loadedIconPerLevel[index];
        }
        public override Sprite GetLoadedOfferIcon()
        {
            if(!iconsLoaded)
                TafraDebugger.Log("Perk", "Icons are not loaded, load them first by calling LoadIcons(). And make sure to release them when you no longer need them by calling ReleaseIcons()", TafraDebugger.LogType.Error);

            int index = appliesCount;

            int count = loadedIconPerLevel.Count;

            if(index >= count)
                index = count - 1;

            return loadedIconPerLevel[index];
        }
    }
}