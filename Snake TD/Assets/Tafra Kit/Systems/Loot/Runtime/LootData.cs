using TafraKit.Mathematics;
using UnityEngine;

namespace TafraKit.Loot
{
    [CreateAssetMenu(menuName = "Tafra Kit/Loot/Loot Data",fileName = "Loot Data")]
    public class LootData : IdentifiableScriptableObject
    {
        public enum LootRewardType
        { 
            AddToScriptableFloat = 0,
            SubtractFromScriptableFloat = 1,
            SetScriptableFloat = 2,
            AddToInventory = 10,
            Internal = 11
        }

        [Header("Main Data")]
        [SerializeField] private GameObject lootPrefab;
        [SerializeField] private IdentifiableScriptableObject item;

        [Header("VFX")]
        [SerializeField] private GameObject collectionVFXPrefab;

        [Header("SFX")]
        [SerializeField] private SFXClips dropSFX;
        [SerializeField] private SFXClips collectSFX;

        [Header("Reward")]
        [SerializeField] private LootRewardType rewardType;
        [SerializeField] private ScriptableFloatChange scriptableFloatChange;
        [SerializeField] private bool useChangeEquation;
        [SerializeField] private ScriptableFloatChangeEquation scriptableFloatChangeEquation;

        public GameObject LootPrefab => lootPrefab;
        public IdentifiableScriptableObject Item => item;
        public SFXClips DropSFX => dropSFX;
        public SFXClips CollectSFX => collectSFX;
        public LootRewardType RewardType => rewardType;
        public ScriptableFloatChange FloatChange 
        { 
            get 
            {
                if (useChangeEquation)
                    scriptableFloatChange.ChangeAmount = scriptableFloatChangeEquation.GetChangeAmount();

                return scriptableFloatChange;
            }
        }

        /// <summary>
        /// Action on reward type "Internal"
        /// </summary>
        public virtual void OnCollected() { }
    }
}