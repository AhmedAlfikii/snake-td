using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.ContentManagement;
using TafraKit.GraphViews;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Stats/Stat Manipulator"), GraphNodeName("Stat Manipulator")]
    public class StatManipulatorNode : AbilityTaskNode
    {
        [SerializeField] private TafraAsset<Stat> stat;
        [SerializeField] private NumberOperation operation;
        [SerializeField] private BlackboardAdvancedFloatGetter value;
        [Tooltip("If true, the value added to the stat by this element will be part of the \"Extra Value\", which is the value caused by stat values or manipulators that are not considered base." +
            "Will not change the results of the stat calculation.")]
        [SerializeField] private bool isExtra;

        [NonSerialized] private ValueManipulator manipulator;
        [NonSerialized] private StatsContainer statsContainer;
        [NonSerialized] private Stat curStat;

        public StatManipulatorNode(StatManipulatorNode other) : base(other)
        {
            stat = other.stat;
            statsContainer = other.statsContainer;

            manipulator = other.manipulator;
            curStat = other.curStat;
        }
        public StatManipulatorNode()
        {

        }
        public override void OnDestroy()
        {
            base.OnDestroy();

            if (value.Property != null)
                value.Property.OnValueChange.RemoveListener(OnValueChange);

            stat.Release();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            value.Initialize(ability.BlackboardCollection);
            
            manipulator = new ValueManipulator(operation, new TafraAdvancedFloat(), isExtra);

            curStat = stat.Load();

            value.CacheProperty();
            
            if(value.Property != null)
            {
                value.Property.EnableValueChangeSignal();
                value.Property.OnValueChange.AddListener(OnValueChange);
            }

            if(actor != null && actor is IStatsContainerProvider statsContainerProvider)
                statsContainer = statsContainerProvider.StatsContainer;

            OnValueChange();
        }

        protected override void OnStart()
        {
            if(statsContainer == null)
                return;

            OnValueChange();

            statsContainer.AddStatManipulator(curStat, manipulator);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            if(statsContainer == null)
                return;

            statsContainer.RemoveStatManipulator(curStat, manipulator);
        }

        protected override BTNode CloneContent()
        {
            StatManipulatorNode clonedNode = new StatManipulatorNode(this);

            return clonedNode;
        }

        private void OnValueChange()
        {
            manipulator.value.Value = value.Value.Value;
            statsContainer.ForceRecalculateStat(curStat);
        }
    }
}