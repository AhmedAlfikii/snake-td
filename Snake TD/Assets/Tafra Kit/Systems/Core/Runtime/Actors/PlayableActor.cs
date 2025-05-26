using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit
{
    public class PlayableActor : TafraActor, IStatsContainerProvider, ICharacterController
    {
        [SerializeField] private StatsContainer statsContainer;

        [Header("Control Categories")]
        [SerializeField] private CharacterControlsCategory[] characterControlCategories;

        private Dictionary<string, List<ITafraPlayable>> controlsCategories = new Dictionary<string, List<ITafraPlayable>>();
        private List<string> allCategories = new List<string>();
        private HashSet<int> activeActionHashes = new HashSet<int>();
        private bool canPerformNewAction = true;

        bool ICharacterController.CanPerformNewAction { get => canPerformNewAction; set => canPerformNewAction = value; }

        public StatsContainer StatsContainer => statsContainer;
        public Dictionary<string, List<ITafraPlayable>> ControlsCategories => controlsCategories;
        public List<string> AllCategories => allCategories;
        public HashSet<int> ActiveActionHashes => activeActionHashes;

        protected override void Awake()
        {
            statsContainer.Initialize();

            //Initialize the control categories.
            for (int i = 0; i < characterControlCategories.Length; i++)
            {
                var ccc = characterControlCategories[i];

                ccc.Initialize();

                string categoryID = ccc.Category.Value;

                allCategories.Add(categoryID);
                controlsCategories.Add(categoryID, ccc.ControlPlayables);
            }

            base.Awake();
        }
    }
}