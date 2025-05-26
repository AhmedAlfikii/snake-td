using System;
using System.Text;
using TafraKit.ContentManagement;
using TafraKit.Internal.CharacterControls;
using Unity.VisualScripting;
using UnityEngine;

namespace TafraKit.RPG
{
    [SearchMenuItem("Stat Manipulator")]
    public class NormalStatManipulatorUpgradeModule : NormalUpgradeModule
    {
        [Header("Identification")]
        [SerializeField] private int statIconIndex = 1;

        [Header("Properties")]
        [SerializeField] private Stat stat;
        [SerializeField] private ValueManipulator manipulator;

        [NonSerialized] private Sprite loadedIcon;
        [NonSerialized] private StringBuilder descriptionSB;
        [NonSerialized] private string description;

        public override Sprite LoadedIcon => loadedIcon;
        public override string DisplayName => stat.DisplayName;
        public override string Description => description;

        protected override void OnInitialize()
        {
            descriptionSB = new StringBuilder();

            switch(manipulator.operation)
            {
                case NumberOperation.Add:
                    descriptionSB.Append('+');
                    descriptionSB.Append(manipulator.value.Value);
                    descriptionSB.Append(' ');
                    break;
                case NumberOperation.Subtract:
                    descriptionSB.Append('-');
                    descriptionSB.Append(manipulator.value.Value);
                    descriptionSB.Append(' ');
                    break;
                case NumberOperation.Multiply:
                    float val = manipulator.value.Value;

                    if(val > 1)
                    {
                        val = (val - 1) * 100;
                        descriptionSB.Append('+');
                        descriptionSB.Append(val);
                        descriptionSB.Append('%');
                    }
                    else
                    {
                        val = 1 - val;
                        descriptionSB.Append('-');
                        descriptionSB.Append(val);
                        descriptionSB.Append('%');
                    }

                    descriptionSB.Append(' ');
                    break;
                case NumberOperation.Divide:
                    Debug.LogError("Not implemented");
                    break;
                case NumberOperation.Set:
                    Debug.LogError("Not implemented");
                    break;
            }

            descriptionSB.Append(stat.DisplayName);

            description = descriptionSB.ToString();
        }
        public override Sprite LoadIcon()
        {
            if(loadedIcon != null)
                return loadedIcon;

            loadedIcon = stat.RequestIcon(statIconIndex);

            return loadedIcon;
        }
        public override void ReleaseIcon()
        {
            if(loadedIcon == null)
                return;

            loadedIcon = null;

            stat.ReleaseIcon(statIconIndex);
        }
        protected override void OnApply()
        {
            AddManipulatorToPlayer();
        }
        protected override void OnSceneLoaded()
        {
            AddManipulatorToPlayer();
        }

        private void AddManipulatorToPlayer()
        {
            SceneReferences.Player.StatsContainer.AddStatManipulator(stat, manipulator);
        }
    }
}