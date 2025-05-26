using UnityEngine;

namespace TafraKit.RPG
{
    [System.Serializable]
    public abstract class UpgradeModule
    {
        protected LinearUpgradePath path;

        public abstract Sprite LoadedIcon { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }

        public void Initialize(LinearUpgradePath path)
        {
            this.path = path;

            OnInitialize();
        }
        /// <summary>
        /// Will be called if this is the first time this upgrade was applied.
        /// </summary>
        public void Apply()
        {
            OnApply();
        }
        /// <summary>
        /// Will be called if the upgrade should apply because it was applied in a previous session.
        /// </summary>
        public void LoadedApply()
        {
            OnApply();
        }
        public void SceneLoaded()
        {
            OnSceneLoaded();
        }

        public abstract Sprite LoadIcon();
        public abstract void ReleaseIcon();

        protected virtual void OnInitialize() { }
        protected virtual void OnApply() { }
        protected virtual void OnSceneLoaded() { }
    }
}