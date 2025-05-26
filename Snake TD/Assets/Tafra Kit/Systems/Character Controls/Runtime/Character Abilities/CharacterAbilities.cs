using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.CharacterControls
{
    public class CharacterAbilities : MonoBehaviour, ITafraPlayable
    {
        public enum AbilitiesContainerTypeProperty
        { 
            Internal = 0,
            External = 1
        }

        [Tooltip("Only use external abilities container for characters that only have one instance. Such as the player.")]
        [SerializeField] private AbilitiesContainerTypeProperty abilitiesContainerType;
        [SerializeField] private ExternalAbilitiesContainer externalAbilitiesContainer;
        [SerializeField] private AbilitiesContainer internalAbilitiesContainer;
        [SerializeField] private List<ExternalBlackboard> externalBlackboards;

        private TafraActor actor;
        private AbilitiesContainer activeAbilitiesContainer;
        private bool isInitialized;
        private bool isPlaying;
        private bool isPaused;
        private bool shouldBePlaying;
        private bool isWaitingToBePlayed;
        private HashSet<string> pausers = new HashSet<string>();
        private UnityEvent onInitialized = new UnityEvent();
        private UnityEvent<Ability, string> onAbilityEquipped = new UnityEvent<Ability, string>();
        private UnityEvent<Ability> onAbilityUnquipped = new UnityEvent<Ability>();

        public AbilitiesContainer AbilitiesContainer => activeAbilitiesContainer;
        public TafraActor Actor => actor;
        public bool IsInitialized => isInitialized;
        public bool IsPlaying => isPlaying;
        public List<ExternalBlackboard> ExternalBlackboards => externalBlackboards;
        public UnityEvent OnInitialized => onInitialized;
        /// <summary>
        /// Gets fired when an ability is equipped, contains the equipped instance and the slot it was added to if any.
        /// </summary>
        public UnityEvent<Ability, string> OnAbilityEquipped => onAbilityEquipped;
        /// <summary>
        /// Gets fired when an ability is unequipped, contains the original ability of the unequipped instance (since the instance is destroyed at this point).
        /// </summary>
        public UnityEvent<Ability> OnAbilityUnquipped => onAbilityUnquipped;

        public ITafraPlayable Playable => this;
        bool ITafraPlayable.IsPlaying { get => isPlaying; set => isPlaying = value; }
        bool ITafraPlayable.ShouldBePlaying { get => shouldBePlaying; set => shouldBePlaying = value; }
        bool ITafraPlayable.IsPaused { get => isPaused; set => isPaused = value; }
        HashSet<string> ITafraPlayable.Pausers => pausers;
        bool ITafraPlayable.IsWaitingToBePlayed { get => isWaitingToBePlayed; set => isWaitingToBePlayed = value; }

        private void Awake()
        {
            Playable.Play();

            actor = GetComponent<TafraActor>();

            if (abilitiesContainerType == AbilitiesContainerTypeProperty.External)
                activeAbilitiesContainer = externalAbilitiesContainer.Container;
            else
                activeAbilitiesContainer = internalAbilitiesContainer;
        }
        private IEnumerator Start()
        {
            yield return Yielders.EndOfFrame;

            activeAbilitiesContainer.Initialize(this, actor);

            isInitialized = true;

            onInitialized?.Invoke();
        }
        private void OnDestroy()
        {
            if(!isInitialized)
                return;

            activeAbilitiesContainer.DeactivateEquippedAbilities();
        }
        //private void Update()
        //{
        //    if(!isInitialized)
        //        return;

        //    activeAbilitiesContainer.Tick();
        //}
        private void FixedUpdate()
        {
            if(!isInitialized)
                return;

            activeAbilitiesContainer.Tick();
        }

        public Ability Equip(Ability ability, string slot = null, bool save = true, Action<Ability> onBeforeActivate = null)
        {
            Ability equippedAbility = activeAbilitiesContainer.Equip(ability, slot, save, false, onBeforeActivate);

            if(equippedAbility != null)
                onAbilityEquipped?.Invoke(equippedAbility, slot);

            return equippedAbility;
        }
        public bool Unequip(Ability ability)
        {
            Ability originalAbility = ability.OriginalAbility;

            bool success = activeAbilitiesContainer.Unequip(ability);

            if(success)
                onAbilityUnquipped?.Invoke(originalAbility);

            return success;
        }
        public bool IsEquipped(Ability ability)
        {
            return activeAbilitiesContainer.IsEquipped(ability);
        }
        public bool TryGetEquippedAbility(Ability ability, out Ability equippedInstance)
        {
            return activeAbilitiesContainer.TryGetEquippedAbility(ability, out equippedInstance);
        }

        void ITafraPlayable.OnPlay()
        {
            
        }

        void ITafraPlayable.OnStop()
        {
            
        }

        void ITafraPlayable.OnPause()
        {
            activeAbilitiesContainer.PauseEquippedAbilities();
        }

        void ITafraPlayable.OnResume()
        {
            activeAbilitiesContainer.ResumeEquippedAbilities();
        }

        void ITafraPlayable.OnResumeIntoPlay()
        {
            
        }
    }
}