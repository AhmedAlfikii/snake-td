using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.ContentManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit
{
    public abstract class IdentifiableScriptableObject : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("The id used to identify this object. If empty, the scriptable obejct's name will be used instead.")]
        [SerializeField] private string id;
        [Tooltip("The names that will be displayed to players.")]
        [SerializeField] protected List<string> displayNames;
        [TextArea(), Tooltip("The description that will be displayed to players.")]
        [SerializeField] protected string description;
        [Tooltip("The icons that will be displayed to players.")]
        [SerializeField] protected List<TafraAsset<Sprite>> icons;
        [Tooltip("The icons that will be displayed to players in case this object is locked.")]
        [SerializeField] protected List<TafraAsset<Sprite>> lockedIcons;

        [NonSerialized] private bool isInitialized;
        [NonSerialized] private string curID;
        [NonSerialized] private List<string> curDisplayNames;
        [NonSerialized] private string curDescription;
        /// <summary>
        /// Each element represents the number of objects that has requested this specific icon. If the number drops to 0, the icon will be unloaded.
        /// </summary>
        [NonSerialized] private List<int> curIconRequesters = new List<int>();
        /// <summary>
        /// Each element represents the number of objects that has requested this specific locked icon. If the number drops to 0, the icon will be unloaded.
        /// </summary>
        [NonSerialized] private List<int> curLockedIconRequesters = new List<int>();

        /// <summary>
        /// The identifier that is unique to instances of this object.
        /// </summary>
        public virtual string ID
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                return curID;
            }
        }
        /// <summary>
        /// The first name that should be displayed to players.
        /// </summary>
        public virtual string DisplayName
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                if(curDisplayNames.Count == 0)
                    return "";

                return curDisplayNames[0];
            }
        }
        /// <summary>
        /// The description that should be displayed to players.
        /// </summary>
        public virtual string Description
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                return curDescription;
            }
        }

        protected virtual void OnEnable()
        {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

            if (!isInitialized)
                Initialize();
        }

        private void Initialize()
        {
            curID = !string.IsNullOrEmpty(id) ? id : name;
            curDisplayNames = new List<string>(displayNames);
            curDescription = description;

            for (int i = 0; i < icons.Count; i++)
            {
                curIconRequesters.Add(0);
            }

            for (int i = 0; i < curLockedIconRequesters.Count; i++)
            {
                curLockedIconRequesters.Add(0);
            }

            isInitialized = true;

            OnInitialized();
        }

        protected virtual void OnInitialized() { }

        /// <summary>
        /// Request one of the icons. Make sure to release it once it's no longer needed to get it to unload from memory.
        /// </summary>
        /// <param name="iconIndex"></param>
        /// <returns></returns>
        public Sprite RequestIcon(int iconIndex = 0)
        {
            if(!isInitialized)
                Initialize();

            curIconRequesters[iconIndex]++;

            return icons[iconIndex].Load();
        }
        /// <summary>
        /// Request one of the locked icons. Make sure to release it once it's no longer needed to get it to unload from memory.
        /// </summary>
        /// <param name="iconIndex"></param>
        /// <returns></returns>
        public Sprite RequestLockedIcon(int iconIndex = 0)
        {
            if(!isInitialized)
                Initialize();

            if(lockedIcons.Count <= 0)
                return null;

            curLockedIconRequesters[iconIndex]++;

            return lockedIcons[iconIndex].Load();
        }
        /// <summary>
        /// Will return the icon only if it was loaded before. DON'T release it by calling ReleaseIcon. (Only use this if you know the icon should be loaded, otherwise use RequestIcon)
        /// </summary>
        /// <param name="iconIndex"></param>
        /// <returns></returns>
        public Sprite GetIconIfLoaded(int iconIndex = 0)
        {
            if(!isInitialized)
                Initialize();

            if(icons[iconIndex].IsLoaded)
                return icons[iconIndex].Load();

            return null;
        }
        /// <summary>
        /// Will return the locked icon only if it was loaded before. DON'T release it by calling ReleaseLockedIcon. (Only use this if you know the locked icon should be loaded, otherwise use RequestLockedIcon)
        /// </summary>
        /// <param name="iconIndex"></param>
        /// <returns></returns>
        public Sprite GetLockedIconIfLoaded(int iconIndex = 0)
        {
            if(!isInitialized)
                Initialize();

            if(lockedIcons[iconIndex].IsLoaded)
                return lockedIcons[iconIndex].Load();

            return null;
        }

        /// <summary>
        /// Release the icon that you have previously requested.
        /// </summary>
        /// <param name="iconIndex"></param>
        public void ReleaseIcon(int iconIndex = 0)
        {
            if(!isInitialized)
                Initialize();

            curIconRequesters[iconIndex]--;

            if(curIconRequesters[iconIndex] <= 0)
            {
                curIconRequesters[iconIndex] = 0;

                icons[iconIndex].Release();
            }
        }
        /// <summary>
        /// Release the locked icon that you have previously requested.
        /// </summary>
        /// <param name="iconIndex"></param>
        public void ReleaseLockedIcon(int iconIndex = 0)
        {
            if(!isInitialized)
                Initialize();

            curLockedIconRequesters[iconIndex]--;

            if(curLockedIconRequesters[iconIndex] <= 0)
            {
                curLockedIconRequesters[iconIndex] = 0;

                lockedIcons[iconIndex].Release();
            }
        }

        public string GetDisplayName(int nameIndex)
        {
            if(!isInitialized)
                Initialize();

            return curDisplayNames[nameIndex];
        }

        public void SetDisplayNames(List<string> displayNames)
        { 
            curDisplayNames = new List<string>(displayNames);
        }
        public void SetDescription(string description)
        {
            curDescription = description;
        }
    }
}