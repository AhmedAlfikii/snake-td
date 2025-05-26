using System;
using TafraKit.Conditions;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit
{
    [CreateAssetMenu(menuName = "Tafra Kit/Utilities/Lock", fileName = "Lock")]
    public class Lock : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private ConditionsGroup unlockConditions;
        [SerializeField] private string lockedMessage = "This feature is not yet available.";

        [NonSerialized] private bool isInitialized;
        [NonSerialized] private bool isUnlocked;
        [NonSerialized] private string isUnlockedSaveKey; 
        [NonSerialized] private UnityEvent<bool> onUnlockedStateChange = new UnityEvent<bool>();

        public string ID => id;
        public string LockedMessage => lockedMessage;
        public UnityEvent<bool> OnUnlockedStateChange => onUnlockedStateChange;

        protected virtual void OnEnable()
        {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

            if (!isInitialized)
                Initialize();
        }
        private void OnDisable()
        {
            if (unlockConditions.IsActive)
                unlockConditions.Deactivate();
        }

        private void Initialize()
        {
            if(string.IsNullOrEmpty(id))
                TafraDebugger.Log("Lock", "Locks can't have an empty ID.", TafraDebugger.LogType.Error, this);
          
            isUnlockedSaveKey = $"LOCK_{id}_IS_UNLOCKED";

            isUnlocked = TafraSaveSystem.LoadBool(isUnlockedSaveKey, false);

            IsUnlocked();
        }

        public bool IsUnlocked()
        {
            unlockConditions.Activate();

            bool wasSatisfied = unlockConditions.WasSatisfied();

            unlockConditions.Deactivate();

            if(!isUnlocked && wasSatisfied)
            {
                isUnlocked = true;
                TafraSaveSystem.SaveBool(isUnlockedSaveKey, isUnlocked);
                onUnlockedStateChange?.Invoke(true);
            }
            else if(isUnlocked && !wasSatisfied)
            {
                isUnlocked = false;
                TafraSaveSystem.SaveBool(isUnlockedSaveKey, isUnlocked);
                onUnlockedStateChange?.Invoke(false);
            }

            return wasSatisfied;
        }
    }
}