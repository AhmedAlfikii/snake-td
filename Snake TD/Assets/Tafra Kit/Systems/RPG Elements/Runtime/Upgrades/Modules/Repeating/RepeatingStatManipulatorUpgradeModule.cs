using System;
using System.Text;
using TafraKit.ContentManagement;
using TafraKit.Internal.CharacterControls;
using TafraKit.Mathematics;
using TafraKit.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace TafraKit.RPG
{
    [SearchMenuItem("Stat Manipulator")]
    public class RepeatingStatManipulatorUpgradeModule : RepeatingUpgradeModule
    {
        [Header("Identification")]
        [SerializeField] private int statIconIndex = 1;
        [SerializeField] private bool useStatName = true;
        [SerializeField] private string customName;

        [Header("Properties")]
        [SerializeField] private Stat stat;
        [SerializeField] private NumberOperation operation;
        [SerializeField] private FormulasContainer valuePerRecurrence;
        [Tooltip("If true, the value added to the stat by this element will be part of the \"Extra Value\", which is the value caused by stat values or manipulators that are not considered base." +
            "Will not change the results of the stat calculation.")]
        [SerializeField] private bool isExtra;

        [NonSerialized] private ValueManipulator manipulator;
        [NonSerialized] private Sprite loadedIcon;
        [NonSerialized] private StringBuilder descriptionSB;
        [NonSerialized] private string description;
        [NonSerialized] private float value;

        public override Sprite LoadedIcon => loadedIcon;
        public override string DisplayName
        {
            get
            {
                if (useStatName)
                    return stat.DisplayName;
                else
                    return customName;
            }
        }
        public override string Description => description;

        protected override void OnInitialize()
        {
            value = valuePerRecurrence.Evaluate(recurrenceNumber);

            manipulator = new ValueManipulator(operation, new TafraAdvancedFloat(valuePerRecurrence.Evaluate(recurrenceNumber)), isExtra);

            descriptionSB = new StringBuilder();

            float val = value;

            switch(manipulator.operation)
            {
                case NumberOperation.Add:

                    if(TextHighlighter.HasPositivePrefix)
                        descriptionSB.Append(TextHighlighter.PositivePrefix);

                    descriptionSB.Append("<size=150%>");

                    if(!stat.IsPercentage)
                    {
                        descriptionSB.Append('+');
                        descriptionSB.Append(value);
                    }
                    else
                    {
                        descriptionSB.Append('+');
                        val *= 100;
                        descriptionSB.Append(val);
                        descriptionSB.Append('%');
                    }

                    descriptionSB.Append("</size>");

                    if(TextHighlighter.HasPositivePostfix)
                        descriptionSB.Append(TextHighlighter.PositivePostfix);

                    descriptionSB.Append(' ');

                    break;
                case NumberOperation.Subtract:

                    if(TextHighlighter.HasPositivePrefix)
                        descriptionSB.Append(TextHighlighter.PositivePrefix);

                    descriptionSB.Append("<size=150%>");

                    if(!stat.IsPercentage)
                    {
                        descriptionSB.Append('-');
                        descriptionSB.Append(value);
                    }
                    else
                    {
                        descriptionSB.Append('-');
                        val *= 100;
                        descriptionSB.Append(val);
                        descriptionSB.Append('%');
                    }

                    descriptionSB.Append("</size>");

                    if(TextHighlighter.HasPositivePostfix)
                        descriptionSB.Append(TextHighlighter.PositivePostfix);

                    descriptionSB.Append(' ');

                    break;
                case NumberOperation.Multiply:

                    if(TextHighlighter.HasPositivePrefix)
                        descriptionSB.Append(TextHighlighter.PositivePrefix);

                    descriptionSB.Append("<size=150%>");

                    if(val > 1)
                    {
                        val = (val - 1) * 100;
                        descriptionSB.Append('+');
                        descriptionSB.Append(val);

                        descriptionSB.Append('%');
                    }
                    else
                    {
                        val = (1 - val) * 100;
                        descriptionSB.Append('-');
                        descriptionSB.Append(val);
                        descriptionSB.Append('%');
                    }

                    descriptionSB.Append("</size>");

                    if(TextHighlighter.HasPositivePostfix)
                        descriptionSB.Append(TextHighlighter.PositivePostfix);

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
            var player = SceneReferences.Player;

            if(player == null)
                return;

            player.StatsContainer.AddStatManipulator(stat, manipulator);
        }

        public override RepeatingUpgradeModule Clone(int recurrenceNumber)
        {
            RepeatingStatManipulatorUpgradeModule copy = new RepeatingStatManipulatorUpgradeModule();

            copy.statIconIndex = statIconIndex;
            copy.useStatName = useStatName;
            copy.customName = customName;
            copy.stat = stat;
            copy.operation = operation;
            copy.valuePerRecurrence = valuePerRecurrence;
            copy.isExtra = isExtra;
            copy.recurrenceNumber = recurrenceNumber;
            return copy;
        }
    }
}