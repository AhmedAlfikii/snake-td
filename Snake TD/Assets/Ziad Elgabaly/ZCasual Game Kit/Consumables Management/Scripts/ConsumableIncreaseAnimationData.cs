using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

namespace ZCasualGameKit
{
    [Serializable]
    public class ConsumableIncreaseAnimationData
    {
        #region Classes, Structs & Enums
        public enum HideStartBehaviour
        {
            AfterSelfShow,
            AfterAllShow
        }

        public enum HidingPosition
        {
            InPlace,
            Bar
        }
        #endregion

        #region Public Fields
        [Header("Settings")]
        [Tooltip("The number of visual units to spawn for each 100 units added.")]
        public int VisualsPer100Units = 10;
        [Tooltip("The minimum number of visual units to spawn.")]
        public int MinimumVisualUnits = 1;
        [Tooltip("The maximum number of visual units to spawn. 0 means unlimited.")]
        public int MaximumVisualUnits = 0;

        [Header("Showing Animation")]
        [Tooltip("The duration to wait before starting the animation.")]
        public float ShowingAnimationStartDelay = 0;
        [Tooltip("The position on the screen that the units will be centered around while spawning (if null, the unit spawn center point will be the center of the screen).")]
        public Vector2? ShowingUnitsCustomPosition;
        [Tooltip("The scale that the units will be shown up with")]
        public Vector3 ShowingUnitsScale = Vector3.one;
        [Tooltip("The duration of each unit's showing (scaling up) animation.")]
        public float ShowingSignleUnitDuration = 0.75f;
        [Tooltip("The duration between each unit shown, 0 means all the units will be shown at once.")]
        public float ShowingUnitsInterval = 0.07f;
        [Tooltip("The radius around the showing position of which the units will be spawned in, in relation to the smallest screen dimension (width in portrait and height in landscapes).")]
        [Range(0, 1)]
        public float ShowingUnitsRadiusPercentage = 0.2f;
        [Tooltip("The easing type of the units showing animation.")]
        public EasingType ShowEasingType = new EasingType(MotionType.EaseOutElastic, new TafraKit.EasingEquationsParameters(new TafraKit.EasingEquationsParameters.EaseInOutElasticParameters(0.4f)));

        [Header("Hiding Start Animation")]
        [Tooltip("How will the units start hiding.")]
        public HideStartBehaviour UnitsHideStartBehaviour = HideStartBehaviour.AfterSelfShow;
        [Tooltip("The delay between showing a unit, and hiding it if \"AfterSelfShow\" hide start behaviour is selected, or the delay after showing all the units and hiding the first one if \"AfterAllShow\" hide start behaviour is selected.")]
        public float HidingUnitsDelay = 0;
        [Tooltip("Intervals between hiding a unit and hiding the next one if \"AfterAllShow\" hide start behaviour.")]
        public float HidingUnitsInterval = 0.07f;

        [Header("Hiding Animation")]
        [Tooltip("Should the units move to the consumable's icon on the bar before it hides.")]
        public bool MoveToBar = true;
        [Tooltip("The scale that the units will scale to while moving towards the bar.")]
        public Vector3 ReachBarUnitsScale = Vector3.one;
        [Tooltip("The duration each unit will take to move to the bar if enabled.")]
        public float HidingUnitsMovementDuration = 0.5f;
        [Tooltip("The duration of each unit's hiding (scaling down) animation, after it has reached its destination.")]
        public float HidingSignleUnitDuration = 0;
        [Tooltip("The easing type of the units hiding movement.")]
        public EasingType MoveEasingType = new EasingType(MotionType.EaseInBack, new TafraKit.EasingEquationsParameters(new TafraKit.EasingEquationsParameters.EaseInOutBackParameters(0.9f)));
        [Tooltip("The easing type of the units hiding animation.")]
        public EasingType HideEasingType = new EasingType(MotionType.EaseOut, new TafraKit.EasingEquationsParameters());

        [Header("Bar")]
        public Color BarInreasingColor = new Color(0.992f, 0.843f, 0.023f, 1);
        public Vector3 BarUnitIconIncreaseScale = new Vector3(1.2f, 1.2f, 1.2f);

        [Header("SFX")]
        [Tooltip("The audio clip to play when each unit starts showing.")]
        public SFXClip SingleUnitShowAC = new SFXClip(null, new FloatRange(0.5f, 0.6f), new FloatRange(0.8f, 1.2f));
        [Tooltip("The audio clip to play when each unit moves towards its hiding place (unless it's stays in place).")]
        public SFXClip SingleUnitMoveAC = new SFXClip(null, new FloatRange(0.5f, 0.6f), new FloatRange(0.8f, 1.2f));
        [Tooltip("The audio clip to play when each unit starts hiding (reaches its destination).")]
        public SFXClip SingleUnitHideAC = new SFXClip(null, new FloatRange(0.5f, 0.6f), new FloatRange(0.8f, 1.2f));
        [Tooltip("How many units to not play a sound for after playing the sound effect for a unit.")]
        public int SingleUnitSoundSkips = 1;
        [Tooltip("The audio clip to play the first unit starts showing.")]
        public SFXClip GroupShowAC = new SFXClip(null, new FloatRange(0.8f, 1.0f), new FloatRange(0.8f, 1.2f));
        [Tooltip("The audio clip to play the first unit starts moving.")]
        public SFXClip GroupMoveAC = new SFXClip(null, new FloatRange(0.8f, 1.0f), new FloatRange(0.8f, 1.2f));
        [Tooltip("The audio clip to play the first unit starts hiding (reaches its destination).")]
        public SFXClip GroupHideAC = new SFXClip(null, new FloatRange(0.8f, 1.0f), new FloatRange(0.8f, 1.2f));
        #endregion
    }
}