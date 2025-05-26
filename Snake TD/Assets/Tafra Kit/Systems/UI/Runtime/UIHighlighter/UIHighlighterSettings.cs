using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.UI
{
    public class UIHighlighterSettings : SettingsModule
    {
        [SerializeField] private bool enabled;
        [Tooltip("The sorting order of highlighted elements. The background will be 1 less than this.")]
        [SerializeField] private int sortingOrder = 30000;
        [SerializeField] private GameObject highlightBG;

        public bool Enabled => enabled;
        public int SortingOrder => sortingOrder;
        public GameObject HighlightBG => highlightBG;

        public override int Priority => 30;
        public override string Name => "UI/UI Highlighter";
        public override string Description => "Control how the UI highlighter would work.";
    }
}