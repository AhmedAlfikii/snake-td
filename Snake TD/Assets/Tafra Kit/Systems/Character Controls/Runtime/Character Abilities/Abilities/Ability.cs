using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEngine;
using TafraKit.Internal.CharacterControls;
using UnityEngine.Events;
using TafraKit.Internal.GraphViews;
using TafraKit.RPG;
using TafraKit.ContentManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.CharacterControls
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Tafra Kit/Character Controls/Abilities/Ability")]
    public class Ability : ScriptableObject, IInstanceableScriptableObject, IBTNodesContainer<AbilityNode>, IGraphBlackboardContainer
    {
        [Header("Info")]
        [SerializeField] private string id;
        [Tooltip("The display name to display for each level of this ability. Levels that don't have a corresponding element will use the last element in the list.")]
        [SerializeField] private List<string> displayNamePerLevel;
        [TextArea]
        [Tooltip("The description to display for each level of this ability. Levels that don't have a corresponding element will use the last element in the list.")]
        [SerializeField] private List<string> descriptionPerLevel;
        [Tooltip("The icon to display for each level of this ability. Levels that don't have a corresponding element will use the last element in the list.")]
        [SerializeField] private List<TafraAsset<Sprite>> iconPerLevel;
        [SerializeField] private List<TafraString> keywords;

        [Header("Cooldown")]
        [Tooltip("Should this ability go on cooldown after it has been performed? (only valid for abilities that has an active effect)")]
        [SerializeField] private bool hasCooldown;
        [Tooltip("The number of times this ability can be used before it deactivates because it's on cooldown. (charges will be charged on use not when all charges are consumed)")]
        [SerializeField] private int chargesCount = 1;
        [Tooltip("The cooldown between charging each charge.")]
        [SerializeField] private float cooldown;

        [Header("Associated Assets")]
        [SerializeReferenceListContainer("modules", false, "Asset", "Assets"), Tooltip("Assets that will be loaded when this ability is equipped, and will be unloaded once it's unequipped.")]
        [SerializeField] private AbilityAssetsContainer associatedAssets;

        [Header("System Object Initializers")]
        [SerializeReferenceListContainer("initializers", false, "Initializer", "Initializers")]
        [SerializeField] private AbilitySystemObjectInitiailzersContainer systemObjectInitializers = new AbilitySystemObjectInitiailzersContainer();

        [Header("Upgradeable Properties")]
        [Tooltip("Whenever the ability levels up, the internal blackboard properties will be updated to reflect the values below.")]
        [SerializeReferenceListContainer("upgradeableProperties", false, "Property", "Properties")]
        [SerializeField] private AbilityUpgradeablePropertiesContainer upgradeablePropertiesContainer = new AbilityUpgradeablePropertiesContainer();

        [Header("Upgradeable Properties (obsolete, migrate to the list above)")]
        [SerializeField] private List<AbilityUpgradeablePropertyOld> upgradeableProperties;

        [Header("Previewing")]
        [Tooltip("Will be used to preview the effect of the ability before it's executed (ex. while the player is hovering over it).")]
        [SerializeField] private Ability previewAbility;

        [Header("Temporarily Serialized")]
        [SerializeReference, HideInGraphInspector] private List<AbilityNode> nodes;
        [SerializeReference, HideInInspector] private PassiveAbilityNode passiveNode;
        [SerializeReference, HideInInspector] private ActiveAbilityNode activeNode;
        [SerializeField] private GraphBlackboard blackboard = new GraphBlackboard();

        [NonSerialized] protected int instanceNumber;
        [NonSerialized] protected bool isInstance;
        [NonSerialized] protected CharacterAbilities characterAbilities;
        [NonSerialized] protected TafraActor actor;
        [NonSerialized] protected bool isPaused;
        [NonSerialized] protected bool isActive;
        [NonSerialized] protected bool isPerforming;
        [NonSerialized] protected int nodesCount;
        [NonSerialized] protected bool isOnCooldown;
        [NonSerialized] protected float curCooldown;
        [NonSerialized] protected int curCharges;
        [NonSerialized] protected IEnumerator coolingDownEnum;
        [NonSerialized] protected BlackboardCollection blackboardCollection;
        [NonSerialized] protected int level = 1;
        [NonSerialized] protected List<Sprite> loadedIconPerLevel = new List<Sprite>();
        [NonSerialized] protected int iconRequesters;
        [NonSerialized] protected bool initializedPreviewAbility;
        [NonSerialized] protected List<AbilityUpgradeableProperty> upgradeablePropertiesList;
        [NonSerialized] protected HashSet<string> keywordsHashSet;

        [NonSerialized] protected Action<bool> onFinishedPerformed;
      
        [NonSerialized] public UnityEvent OnStartedCooldown = new UnityEvent();
        [NonSerialized] public UnityEvent OnFinishedCooldown = new UnityEvent();
        [NonSerialized] public UnityEvent OnPerformed = new UnityEvent();
        [NonSerialized] public UnityEvent<int> OnLevelChange = new UnityEvent<int>();

        public int InstanceNumber { get => instanceNumber; set => instanceNumber = value; }
        public string ID => isInstance? InstanceableSO.GetSOInstanceID() : id;
        public string OriginalID => isInstance ? OriginalAbility.ID : ID;
        public string InstanceID => InstanceableSO.GetSOInstanceID();
        public bool IsInstance { get => isInstance; set => isInstance = value; }
        public ScriptableObject OriginalScriptableObject { get; set; }
        public Ability OriginalAbility {
            get
            {
                if(OriginalScriptableObject != null)
                    return OriginalScriptableObject as Ability;
                else
                    return this;
            }
        }
        public IInstanceableScriptableObject InstanceableSO => this;
        public CharacterAbilities CharacterAbilities => characterAbilities;
        public TafraActor Actor => actor;
        public Ability PreviewAbility
        {
            get
            {
                Ability originalAbility = OriginalAbility;

                if(!originalAbility.initializedPreviewAbility)
                    originalAbility.InitializePreviewAbility();

                return previewAbility;
            }
        }
        public bool IsPaused => isPaused;
        public bool IsActive => isActive;
        public bool HasCooldown => hasCooldown;
        public int TotalCharges => chargesCount;
        public int RemainingCharges => curCharges;
        public bool IsOnCooldown => isOnCooldown;
        public float Cooldown => cooldown;
        public float RemainingCooldown => curCooldown;
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if(level == value)
                    return;

                level = value;
                PlayerPrefs.SetInt($"{InstanceID}_Ability_Level", level);

                OnLevelChanged();
            }
        }
        public List<AbilityNode> Nodes => nodes;
        public AbilityNode ActiveNode => activeNode;
        public AbilityNode PassiveNode => passiveNode;
        public AbilityNode RootNode => null;
        public GraphBlackboard Blackboard => blackboard;
        public BlackboardCollection BlackboardCollection => blackboardCollection;
        public bool HasPassiveEffect => passiveNode.Children.Count > 0;
        public bool InitializedPreviewAbility => initializedPreviewAbility;
        public bool HasIcon => iconPerLevel.Count > 0;

        public Ability()
        {
            nodes = new List<AbilityNode>();

            passiveNode = new PassiveAbilityNode() { Position = new Rect(150, 0, 0, 0) };
            activeNode = new ActiveAbilityNode() { Position = new Rect(-150, 0, 0, 0) };

            nodes.Add(activeNode);
            nodes.Add(passiveNode);
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

            bool isOriginal = !name.Contains("(Clone)");

            if(isOriginal)
            {
                if(keywords.Count > 0)
                {
                    keywordsHashSet = new HashSet<string>();

                    for(int i = 0; i < keywords.Count; i++)
                    {
                        keywordsHashSet.Add(keywords[i].Value);
                    }
                }
            }

            for (int i = 0; i < iconPerLevel.Count; i++)
            {
                iconRequesters = 0;
                loadedIconPerLevel.Add(null);
            }

            if (string.IsNullOrEmpty(id))
                TafraDebugger.Log("Ability", "Ability ID can't be empty", TafraDebugger.LogType.Error, this);
        }
        private void OnDestroy()
        {
            for (int i = 0; i < nodesCount; i++)
            {
                nodes[i].OnDestroy();
            }

            for (int i = 0; i < associatedAssets.Modules.Count; i++)
            {
                associatedAssets.Modules[i].ReleaseAsset();
            }

            for(int i = 0; i < iconPerLevel.Count; i++)
            {
                if(loadedIconPerLevel[i] != null)
                {
                    loadedIconPerLevel[i] = null;
                    iconPerLevel[i].Release();
                }
            }
        }
        public void InitializeInstance(CharacterAbilities characterAbilities, TafraActor actor)
        {
            this.characterAbilities = characterAbilities;
            this.actor = actor;
            nodesCount = nodes.Count;

            level = PlayerPrefs.GetInt($"{InstanceID}_Ability_Level", 1);

            blackboard.Initialize(null);

            upgradeablePropertiesList = upgradeablePropertiesContainer.UpgradeableProperties;

            for (int i = 0; i < upgradeablePropertiesList.Count; i++)
            {
                var upgradeableProperty = upgradeablePropertiesList[i];

                upgradeableProperty.Initialize(blackboard);
            }

            //TODO: Remove after migrating all active projects (today is: 03/29/2025)
            #region TO BE DELETED
            if (upgradeableProperties.Count > 0)
                TafraDebugger.Log($"{name} Ability", $"You are still using the old upgradeable properties, please migrate to the new ones and remove the old ones.", TafraDebugger.LogType.Error, this);

            for(int i = 0; i < upgradeableProperties.Count; i++)
            {
                var upgradeableProperty = upgradeableProperties[i];
                
                var intProp = blackboard.TryGetIntProperty(Animator.StringToHash(upgradeableProperty.PropertyName), -1);

                if(intProp != null)
                {
                    upgradeableProperty.SetIntProperty(intProp);
                    continue;
                }

                var floatProp = blackboard.TryGetFloatProperty(Animator.StringToHash(upgradeableProperty.PropertyName), -1);

                if(floatProp != null)
                {
                    upgradeableProperty.SetFloatProperty(floatProp);
                    continue;
                }

                GenericExposableProperty<TafraAdvancedFloat> advancedFloatProp = blackboard.TryGetAdvancedFloatProperty(Animator.StringToHash(upgradeableProperty.PropertyName), -1);

                if(advancedFloatProp != null)
                {
                    advancedFloatProp.value.MyType = TafraAdvancedFloat.FloatType.Value;
                    upgradeableProperty.SetAdvancedFloatProperty(advancedFloatProp);
                    continue;
                }
            }
            #endregion

            blackboardCollection = new BlackboardCollection();
            blackboardCollection.SetDependencies(actor);
            blackboardCollection.SetInternalBlackboard(blackboard);
            
            OnLevelChanged();

            for (int i = 0; i < characterAbilities.ExternalBlackboards.Count; i++)
            {
                blackboardCollection.AddExternalBlackboard(characterAbilities.ExternalBlackboards[i].Blackboard);
            }

            if (hasCooldown)
            {
                curCooldown = 0;
                curCharges = chargesCount;
            }

            for(int i = 0; i < associatedAssets.Modules.Count; i++)
            {
                associatedAssets.Modules[i].LoadAsset(this);
            }

            for(int i = 0; i < nodesCount; i++)
            {
                var node = nodes[i];
                node.Initialize(this);
            }
            for(int i = 0; i < nodesCount; i++)
            {
                var node = nodes[i];
                node.LateInitialize();
            }

            OnInitialize();
        }
        /// <summary>
        /// Gets called when this ability is equipped for the first time. Won't be called again unless the ability was unequipped.
        /// </summary>
        public void EquippedFirstTime()
        {
            OnEquipFirstTime();

            Equipped();
        }
        /// <summary>
        /// Gets called when this ability is equipped for the first time in a session as a result of loading equipped abilities that were equipped in a previous session.
        /// Will keep getting called every new session until the ability is unequipped.
        /// </summary>
        public void EquippedAfterLoad()
        {
            OnEquipAfterLoad();

            Equipped();
        }
        /// <summary>
        /// Gets called when this ability is unequipped.
        /// </summary>
        public void Unequipped()
        {
            OnUnequip();
        }
        /// <summary>
        /// Activate the active effects of this ability. Abilities will only be active if the actor object is available.
        /// </summary>
        public void Activate()
        {
            isActive = true;
            isPaused = false;

            OnActivate();
        }
        /// <summary>
        /// Deactivate the active effects of this ability. Abilities will be deactived if the actor object is not available.
        /// </summary>
        public void Deactivate()
        {
            isActive = false;

            passiveNode.Reset();

            if (isPerforming)
            {
                activeNode.Reset();
                isPerforming = false;
                onFinishedPerformed?.Invoke(false);
            }

            OnDeactivate();
        }
        public void Pause()
        {
            isPaused = true;
        }
        public void Resume()
        {
            isPaused = false;
        }
        public Ability GetOrCreateInstance()
        {
            if(isInstance)
                return this;

            Ability instance = InstanceableSO.CreateInstance() as Ability;

            return instance;
        }
        public void Tick()
        {
            passiveNode.Update();

            if(isPerforming)
            {
                var activeNodeState = activeNode.Update();

                if(activeNodeState != BTNodeState.Running)
                {
                    isPerforming = false;
                    onFinishedPerformed?.Invoke(true);
                }
            }
        }
        /// <summary>
        /// Attempt to perform the active effect and returns true if it exists.
        /// </summary>
        /// <returns>Whether or not the ability can be performed, has an active effect (any nodes under its active node) and is not already being performed.</returns>
        public bool Perform(Action onStartedPerforming = null, Action<bool> onFinishedPerforming = null)
        {
            this.onFinishedPerformed = onFinishedPerforming;

            if(isPerforming || (hasCooldown && curCharges <= 0) || activeNode.Children.Count == 0)
            {
                onFinishedPerforming?.Invoke(false);
                return false;
            }

            isPerforming = true;

            if(hasCooldown)
            {
                curCharges--;

                if(!isOnCooldown)
                    StartCooldown();
            }

            onStartedPerforming?.Invoke();
            OnPerformed?.Invoke();

            return true;
        }
        /// <summary>
        /// Force the active node to stop if it's active.
        /// </summary>
        public void StopPerforming()
        {
            if(!isPerforming)
                return;

            isPerforming = false;

            if(activeNode.Children.Count > 0)
                activeNode.Reset();

            onFinishedPerformed?.Invoke(false);
        }
        public void InitializePreviewAbility()
        {
            initializedPreviewAbility = true;

            if (previewAbility != null)
                previewAbility.systemObjectInitializers = systemObjectInitializers;
        }
        public void ApplySystemObjectInitializers()
        {
            for (int i = 0; i < systemObjectInitializers.Initializers.Count; i++)
            {
                var initializer = systemObjectInitializers.Initializers[i];

                initializer.Apply(blackboard);
            }
        }
        /// <summary>
        /// Returns the display name that should appear to players for the desired level. If level is set to 0 or less, then the display name of the abilities current level will be returned.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetDisplayName(int level = 0)
        {
            int displayNamesCount = displayNamePerLevel.Count;

            if(level <= 0)
                level = this.level;

            if(level <= displayNamesCount)
                return displayNamePerLevel[level - 1];
            else
                return displayNamePerLevel[displayNamesCount - 1];
        }
        /// <summary>
        /// Returns the description that should appear to players for the desired level. If level is set to 0 or less, then the description of the abilities current level will be returned.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetDescription(int level = 0)
        {
            int descriptionsCount = descriptionPerLevel.Count;

            if(level <= 0)
                level = this.level;

            if(level <= descriptionsCount)
                return descriptionPerLevel[level - 1];
            else
                return descriptionPerLevel[descriptionsCount - 1];
        }
        public void LoadIcons()
        {
            if(iconRequesters == 0)
            {
                for(int i = 0; i < iconPerLevel.Count; i++)
                {
                    loadedIconPerLevel[i] = iconPerLevel[i].Load();
                }
            }

            iconRequesters++;
        }
        /// <summary>
        /// Returns the icon that should appear to players for the desired level. If level is set to 0 or less, then the icon of the abilities current level will be returned.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public Sprite GetLoadedIcon(int level = 0)
        {
            int iconsCount = iconPerLevel.Count;

            if(level <= 0)
                level = this.level;

            int iconIndex;
            if(level <= iconsCount)
                iconIndex = level - 1;
            else
                iconIndex = iconsCount - 1;

            Sprite loadedIcon = loadedIconPerLevel[iconIndex];

            return loadedIcon;
        }
        public void ReleaseIcons()
        {
            iconRequesters--;

            if(iconRequesters <= 0)
            {
                iconRequesters = 0;

                for(int i = 0; i < iconPerLevel.Count; i++)
                {
                    loadedIconPerLevel[i] = null;
                    iconPerLevel[i].Release();
                }
            }
        }
        public bool HasKeyword(string keyword)
        {
            if(isInstance)
                return OriginalAbility.HasKeyword(keyword);
            else
            {
                if(keywordsHashSet == null)
                    return false;

                return keywordsHashSet.Contains(keyword);
            }
        }

        private void StartCooldown()
        {
            OnStartedCooldown?.Invoke();

            isOnCooldown = true;

            if(coolingDownEnum != null)
                characterAbilities.StopCoroutine(coolingDownEnum);
           
            coolingDownEnum = CoolingDown();

            characterAbilities.StartCoroutine(coolingDownEnum);
        }
        private void EndCooldown()
        {
            curCharges++;

            isOnCooldown = false;

            OnFinishedCooldown?.Invoke();

            if(curCharges < chargesCount)
                StartCooldown();
        }
        private IEnumerator CoolingDown()
        {
            float startTime = Time.time;
            float endTime = startTime + cooldown;

            while(Time.time < endTime)
            { 
                curCooldown = endTime - Time.time;
                yield return null;
            }
            curCooldown = 0;
        
            coolingDownEnum = null;

            EndCooldown();
        }
        /// <summary>
        /// Gets called whenever this ability is equipped, whether it's the very first time or if it was loaded.
        /// </summary>
        private void Equipped()
        {
            OnEquip();
        }

        private void OnLevelChanged()
        {
            for(int i = 0; i < upgradeablePropertiesList.Count; i++)
            {
                var upgradeableProperty = upgradeablePropertiesList[i];

                upgradeableProperty.UpdateValue(level);
            }

            //TODO: Remove after migrating all active projects (today is: 03/29/2025)
            #region TO BE DELETED
            for(int i = 0; i < upgradeableProperties.Count; i++)
            {
                var upgradeableProperty = upgradeableProperties[i];

                switch(upgradeableProperty.PropertyType)
                {
                    case AbilityUpgradeablePropertyOld.BBPropertyType.Float:
                        upgradeableProperty.FloatProperty.value = upgradeableProperty.Formula.Evaluate(level);
                        upgradeableProperty.FloatProperty.SignalValueChange();
                        break;
                    case AbilityUpgradeablePropertyOld.BBPropertyType.AdvancedFloat:
                        upgradeableProperty.AdvancedFloatProperty.value.Value = upgradeableProperty.Formula.Evaluate(level);
                        upgradeableProperty.AdvancedFloatProperty.SignalValueChange();
                        break;
                    case AbilityUpgradeablePropertyOld.BBPropertyType.Int:
                        upgradeableProperty.IntProperty.value = Mathf.RoundToInt(upgradeableProperty.Formula.Evaluate(level));
                        upgradeableProperty.IntProperty.SignalValueChange();
                        break;
                }
            }
            #endregion

            OnLevelChange?.Invoke(level);
        }
        /// <summary>
        /// Gets called everytime the ability gets assigned a character abilities. Will happen every time the actor is spawned.
        /// </summary>
        protected virtual void OnInitialize() { }
        /// <summary>
        /// Gets called when this ability is equipped for the first time. Won't be called again unless the ability was unequipped.
        /// </summary>
        protected virtual void OnEquipFirstTime() { }
        /// <summary>
        /// Gets called when this ability is equipped for the first time in a session as a result of loading equipped abilities that were equipped in a previous session.
        /// Will keep getting called every new session until the ability is unequipped.
        /// </summary>
        protected virtual void OnEquipAfterLoad() { }
        /// <summary>
        /// Gets called whenever this ability is equipped, whether it's the very first time or if it was loaded.
        /// </summary>
        protected virtual void OnEquip() { }
        /// <summary>
        /// Gets called when this ability is unequipped.
        /// </summary>
        protected virtual void OnUnequip() { }
        /// <summary>
        /// Gets called when the active effects of this ability are activated. Abilities will only be active if the actor object is available.
        /// </summary>
        protected virtual void OnActivate() { }
        /// <summary>
        /// Gets called when the active effects of this ability are deactivated. Abilities will be deactived if the actor object is not available.
        /// </summary>
        protected virtual void OnDeactivate() { }

        [ContextMenu("Increase Level")]
        public void IncreaseLevel()
        {
            Level++;
        }
    }
}