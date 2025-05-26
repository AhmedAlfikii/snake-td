using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;
using ZUI;

namespace ZCasualGameKit
{
    public class ConsumablesManager : MonoBehaviour
    {
        public static ConsumablesManager Instance;

        #region Classes & Enums
        [Serializable]
        public class RectTransformPool : DynamicPool<RectTransform> { }

        [Serializable]
        public class Consumable
        {
            [Tooltip("The data for this consumable.")]
            public ConsumableData Data;
            [Tooltip("The bar to display this consumable's amount.")]
            public ConsumableBar Bar;

            [Header("Animation")]
            public ConsumableIncreaseAnimationData DefaultIncreaseAnimationData;
            public RectTransformPool AnimationUnitsPool;
            [Tooltip("If true, in case an animation is requested to play while another one is playing, the currently running animation will be stopped and the units its animating will be taken by the new animation.")]
            public bool ReusePreviousUnits = false;

            [Header("Others")]
            [Tooltip("OPTIONAL: override the default out of units popup in-case this consumable was fully consumed.")]
            public UIElementsGroup OutOfUnitsUIEGOverride;

            public ConsumableAnimator Animator = new ConsumableAnimator();
            public IEnumerator animationEnum;

            public float Units
            {
                get
                {
                    return PlayerPrefs.GetFloat(string.Format("CONSUMABLE_{0}_UNITS", Data.ID), Data.StartingValue);
                }
                set
                {
                    PlayerPrefs.SetFloat(string.Format("CONSUMABLE_{0}_UNITS", Data.ID), value);
                }
            }
        }
        #endregion

        #region Private Serialized Fields
        [Header("Consumables")]
        [SerializeField] private List<Consumable> consumables = new List<Consumable>();

        [Header("Defaults")]
        [SerializeField] private UIElementsGroup defaultOutOfUnitsUIEG;
        #endregion

        #region Private Fields
        private Dictionary<string, Consumable> consumablesDict = new Dictionary<string, Consumable>();
        #endregion

        #region MonoBehaviour Messages
        void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                for (int i = 0; i < consumables.Count; i++)
                {
                    consumablesDict.Add(consumables[i].Data.ID, consumables[i]);
                    consumables[i].AnimationUnitsPool.Initialize();
                    consumables[i].Animator.Initialize(this);

                    consumables[i].Bar.Amount = consumables[i].Units;
                }
            }
        }

        void Start()
        {
            for (int i = 0; i < consumables.Count; i++)
            {
                consumables[i].Animator.Initialize(this);
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Adds units to the consumable with the given ID.
        /// </summary>
        /// <param name="consumableID">The ID if the consumable to add units to.</param>
        /// <param name="units">The units to be added to the consumable.</param>
        /// <param name="animation">The animation data to play while increasing the units, leave it as null to play the default animation. If you don't want to play an animation, set the next parameter (<param name="disableAnimation">) to true. (if "hidden" is set to true, no animation will be played in all cases).</param>
        /// <param name="disableAnimation">If true, no animation will be played.</param>
        /// <param name="hidden">If true, the consumable bar will not be updated.</param>
        public void AddUnits(string consumableID, float units, ConsumableIncreaseAnimationData animation = null, bool disableAnimation = false, bool hidden = false)
        {
            if (!consumablesDict.ContainsKey(consumableID))
            {
                Debug.LogError(string.Format("There's no consumable with the given id ({0})", consumableID));
                return;
            }

            Consumable consumable = consumablesDict[consumableID];

            float newUnitsCount = consumable.Units + units;

            if (consumable.Data.IsCapped && newUnitsCount > consumable.Data.MaximumAmount)
            {
                //Set the units to the actual amount that will be added to the consumable. To use that new added units in the animation.
                units -= newUnitsCount - consumable.Data.MaximumAmount;

                newUnitsCount = consumable.Data.MaximumAmount;
            }

            consumable.Units = newUnitsCount;

            if (!hidden)
            {
                if (disableAnimation)
                    consumable.Bar.Amount = newUnitsCount;
                else if (animation != null)
                    consumable.Animator.PlayIncrease(animation, consumable, units, consumable.ReusePreviousUnits);
                else
                    consumable.Animator.PlayIncrease(consumable.DefaultIncreaseAnimationData, consumable, units, consumable.ReusePreviousUnits);
            }
        }

        /// <summary>
        /// Deducts units from the consumable with the given ID, if available.
        /// </summary>
        /// <param name="consumableID">The ID if the consumable to add units to.</param>
        /// <param name="units">The units to be added to the consumable.</param>
        /// <param name="displayOutOfUnitsUIEG">Should the "Out of Units" popup appear if the player doesn't have enough units to consume?</param>
        /// <param name="hidden">If true, the consumable bar will not be updated.</param>
        /// <returns>True if successfully deducted, or false if not enough units found, or the consumable with the given ID does not exist.</returns>
        public bool DeductUnits(string consumableID, float units, bool displayOutOfUnitsUIEG, bool hidden = false)
        {
            if (!consumablesDict.ContainsKey(consumableID))
            {
                Debug.LogError(string.Format("There's no consumable with the given id ({0})", consumableID));
                return false;
            }

            Consumable consumable = consumablesDict[consumableID];

            float curUnitsCount = consumable.Units;

            float newUnitsCount = curUnitsCount - units;

            if (newUnitsCount < 0)
            {
                if (displayOutOfUnitsUIEG)
                {
                    if (consumable.OutOfUnitsUIEGOverride != null)
                        consumable.OutOfUnitsUIEGOverride.ChangeVisibility(true);
                    else
                        defaultOutOfUnitsUIEG.ChangeVisibility(true);
                }
                return false;
            }

            consumable.Units = newUnitsCount;

            if (!hidden)
            {
                consumablesDict[consumableID].Bar.Amount = newUnitsCount;
            }

            return true;
        }

        /// <summary>
        /// Instantly sets the units count of a consumable to the given count.
        /// </summary>
        /// <param name="consumableID">The ID of the consumable to set its units count.</param>
        /// <param name="units">The number of units to set the consumable to.</param>
        /// <param name="hidden">If true, the consumable bar will not be updated.</param>
        public void SetConsumableUnits(string consumableID, float units, bool hidden)
        {
            if (!consumablesDict.ContainsKey(consumableID))
            {
                Debug.LogError(string.Format("There's no consumable with the given id ({0})", consumableID));
                return;
            }

            Consumable consumable = consumablesDict[consumableID];

            consumable.Units = units;

            if (!hidden)
            {
                consumable.Bar.Amount = units;
            }
        }

        /// <summary>
        /// Returns the number of units the consumable with the given ID has.
        /// </summary>
        /// <param name="consumableID">The ID of the consumable to get its units count.</param>
        public float GetConsumableUnits(string consumableID)
        {
            if (!consumablesDict.ContainsKey(consumableID))
            {
                Debug.LogError(string.Format("There's no consumable with the given id ({0})", consumableID));
                return -1;
            }

            return consumablesDict[consumableID].Units;
        }
        #endregion
    }
}