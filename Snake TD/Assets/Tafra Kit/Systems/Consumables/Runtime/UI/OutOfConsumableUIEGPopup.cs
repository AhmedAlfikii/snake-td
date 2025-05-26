using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZUI;
using TMPro;

namespace TafraKit.Consumables
{
    public class OutOfConsumableUIEGPopup : OutOfConsumablePopup
    {
        [SerializeField] private UIElementsGroup uieg;
        [SerializeField] private TextMeshProUGUI descriptionTXT;

        public override void Show(Consumable consumable, float requiredAmount,Action onConsumed = null)
        {
            descriptionTXT.text = $"Out of <sprite=\"{consumable.DisplayName}\" index=0><color=#{ColorUtility.ToHtmlStringRGB(consumable.Color)}>{consumable.GetDisplayName(1)}";
            uieg.ChangeVisibility(true);
        }
        public override void Hide()
        {
            uieg.ChangeVisibility(false);
        }
    }
}