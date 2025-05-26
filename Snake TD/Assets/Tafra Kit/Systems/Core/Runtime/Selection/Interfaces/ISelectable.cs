using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public enum SelectableState
    { 
        Normal,
        Disabled,
        Hovered,
        Active,
        Selected
    }

    public interface ISelectable
    {
        public bool Interactable { get; set; }
        /// <summary>
        /// Contains information about whether or not this selectable is selected. Changing this value will not change the visuals or fire events.
        /// If you're looking to do those, call Select() instead.
        /// </summary>
        public bool IsSelected { get; set; }

        public void SetSelectableState(SelectableState state);
        public void Select();
        public void SelectWhileNotInteractable();
        public void Deselect();
    }
}