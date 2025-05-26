using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.Consumables
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "Tafra Kit/Consumables/Consumable")]
    public class Consumable : ScriptableFloat
    {
        [SerializeField] private List<string> displayNames;
        [SerializeField] private Sprite[] icons;
        [SerializeField] private Sprite[] lockedIcons;
        [SerializeField] private Color[] colors;
        [SerializeField, TextArea] private string description;

        [NonSerialized] private List<string> curDisplayNames = new List<string>();
        [NonSerialized] private string curDescription;
        [NonSerialized] private List<Sprite> curIcons = new List<Sprite>();
        [NonSerialized] private List<Sprite> curLockedIcons = new List<Sprite>();
        [NonSerialized] private List<Color> curColors = new List<Color>();

        public string DisplayName
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                return curDisplayNames[0];
            }
        }
        public string Description
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                return curDescription;
            }
        }
        public Sprite Icon
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                return curIcons[0];
            }
        }
        public List<Sprite> Icons
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                return curIcons;
            }
        }
        public List<Sprite> LockedIcons
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                return curLockedIcons;
            }
        }
        public Color Color
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                return curColors[0];
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            for (int i = 0; i < displayNames.Count; i++)
            {
                curDisplayNames.Add(displayNames[i]);
            }
            curDescription = description;

            for (int i = 0; i < icons.Length; i++)
            {
                curIcons.Add(icons[i]);
            }
            for (int i = 0; i < colors.Length; i++)
            {
                curColors.Add(colors[i]);
            }
        }

        public string GetDisplayName(int nameIndex)
        {
            if(!isInitialized)
                Initialize();

            if(displayNames.Count > nameIndex)
                return displayNames[nameIndex];
            else
                return displayNames[0];
        }
        public Sprite GetIcon(int iconIndex)
        {
            if (!isInitialized)
                Initialize();

            if (curIcons.Count > iconIndex)
                return curIcons[iconIndex];
            else
                return curIcons[0];
        }
        public Sprite GetLockedIcon(int iconIndex)
        {
            if (!isInitialized)
                Initialize();

            if (curLockedIcons.Count > iconIndex)
                return curLockedIcons[iconIndex];
            else
                return curLockedIcons[0];
        }
        public Color GetColor(int colorIndex)
        {
            if (!isInitialized)
                Initialize();

            return curColors[colorIndex];
        }

        public void SetDisplayNames(List<string> displayNames)
        {
            if (!isInitialized)
                Initialize();

            curDisplayNames = displayNames;
        }
        public void SetDescription(string description)
        {
            if (!isInitialized)
                Initialize();

            curDescription = description;
        }
        public void SetIcons(List<Sprite> icons)
        {
            if (!isInitialized)
                Initialize();

            curIcons = icons;
        }
        public void SetLockedIcons(List<Sprite> lockedIcons)
        {
            if (!isInitialized)
                Initialize();

            curLockedIcons = lockedIcons;
        }
        public void SetColors(List<Color> colors)
        {
            if (!isInitialized)
                Initialize();

            curColors = colors;
        }
    }
}