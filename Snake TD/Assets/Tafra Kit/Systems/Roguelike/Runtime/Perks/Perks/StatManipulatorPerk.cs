using System;
using System.Collections.Generic;
using TafraKit.ContentManagement;
using UnityEngine;
using TafraKit.RPG;
using TafraKit.Mathematics;
using TafraKit.Internal.Roguelike;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.Roguelike
{
    [CreateAssetMenu(menuName = "Tafra Kit/Roguelike/Perks/Stat Manipulator Perk", fileName = "Stat Manipulator Perk", order = 1)]
    public class StatManipulatorPerk : IdentifiablePerk
    {
        [Serializable]
        public class Manipulator
        {
            public Stat stat;
            public NumberOperation operation;
            public FormulasContainer valueAtLevel;
            [Tooltip("If true, the value added to the stat by this element will be part of the \"Extra Value\", which is the value caused by stat values or manipulators that are not considered base." +
                "Will not change the results of the stat calculation.")]
            public bool isExtra;
        }

        [Header("Manipulators")]
        [SerializeField] private List<Manipulator> manipulators;

        [NonSerialized] protected List<ValueManipulator> valueManipulators = new List<ValueManipulator>();

        protected override void OnInitialize()
        {
            base.OnInitialize();

            for(int i = 0; i < manipulators.Count; i++)
            {
                var manipulator = manipulators[i];
                valueManipulators.Add(new ValueManipulator(manipulator.operation, new TafraAdvancedFloat(), manipulator.isExtra));
            }
        }

        protected override void OnApplied()
        {
            AddStatManipulatorsToPlayer();
        }
        protected override void OnSceneLoad()
        {
            AddStatManipulatorsToPlayer();
        }
        private void AddStatManipulatorsToPlayer()
        {
            PlayableActor player = SceneReferences.Player;

            if(player == null)
                return;

            for (int i = 0; i < manipulators.Count; i++)
            {
                var manipulator = manipulators[i];
                var valueManipulator = valueManipulators[i];

                valueManipulator.value.Value = manipulator.valueAtLevel.Evaluate(appliesCount);

                bool successfullyAdded = player.StatsContainer.AddStatManipulator(manipulator.stat, valueManipulator);

                //This means it was added before.
                if(!successfullyAdded)
                    player.StatsContainer.ForceRecalculateStat(manipulator.stat);
            }
        }

        protected override void OnResetSavedData()
        {
            base.OnResetSavedData();

            PlayableActor player = SceneReferences.Player;

            if(player == null)
                return;

            for(int i = 0; i < manipulators.Count; i++)
            {
                var manipulator = manipulators[i];
                var valueManipulator = valueManipulators[i];

                player.StatsContainer.RemoveStatManipulator(manipulator.stat, valueManipulator);
            }
        }
    }
}