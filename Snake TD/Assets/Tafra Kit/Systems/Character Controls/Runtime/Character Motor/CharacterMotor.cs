using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TafraKit.ModularSystem;

namespace TafraKit.CharacterControls
{
    public abstract class CharacterMotor : InternallyModularComponent<CharacterMotorModule>
    {
        [Tooltip("Will be used in case a motor action started that doesn't want to override the default speed.")]
        [SerializeField] protected TafraFloat defaultSpeed = new TafraFloat(3.5f);

        [SerializeReferenceListContainer("modules", false, "Module", "Modules")]
        [SerializeField] private CharacterMotorModulesContainer modulesContainer;

        private UnityEvent<float> movementSpeedMultiplierChange = new UnityEvent<float>();
        private InfluenceReceiver<float> speedMultiplierInfluenceReceiver;

        protected float movementSpeedMultiplier = 1;

        public UnityEvent<float> MovementSpeedMultiplierChange => movementSpeedMultiplierChange;
        public float DefaultSpeed => defaultSpeed.Value;
        public float MovementSpeedMultiplier
        {
            get => movementSpeedMultiplier;
            private set 
            {
                movementSpeedMultiplier = value;
                OnMovementSpeedMultiplierChange();
                MovementSpeedMultiplierChange?.Invoke(movementSpeedMultiplier);
            }
        }
        protected override List<CharacterMotorModule> InternalModules => modulesContainer.Modules;

        protected override void Awake()
        {
            base.Awake();

            for(int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if(module == null)
                    continue;

                module.Initialize(this);
            }

            speedMultiplierInfluenceReceiver = new InfluenceReceiver<float>(ShouldReplaceSpeedMultiplier, OnActiveSpeedMultiplierInfluenceUpdated, null, OnAllSpeedMultiplierInfluencesCleared);
        }

        #region Callbacks
        private bool ShouldReplaceSpeedMultiplier(float newSpeed, float oldSpeed)
        {
            return newSpeed < oldSpeed;
        }
        private void OnActiveSpeedMultiplierInfluenceUpdated(float speed)
        {
            MovementSpeedMultiplier = speed;
        }
        private void OnAllSpeedMultiplierInfluencesCleared()
        {
            MovementSpeedMultiplier = 1;
        }

        protected virtual void OnMovementSpeedMultiplierChange() { }
        #endregion

        public void SetMovementSpeedMultiplier(string influencer, float newMultiplier)
        {
            speedMultiplierInfluenceReceiver.AddInfluence(influencer, newMultiplier);
        }
        public void ResetMovementSpeedMultiplier(string influencer)
        {
            speedMultiplierInfluenceReceiver.RemoveInfluence(influencer);
        }
        public void ClearMovementSpeedMultiplierInfluences()
        {
            speedMultiplierInfluenceReceiver.RemoveAllInfluences();
        }
    }
}