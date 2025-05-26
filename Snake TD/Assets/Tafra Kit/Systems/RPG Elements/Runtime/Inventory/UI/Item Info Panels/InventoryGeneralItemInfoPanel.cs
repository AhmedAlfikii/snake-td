using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TafraKit.RPG
{
    public class InventoryGeneralItemInfoPanel : InventoryItemInfoPanel
    {
        [SerializeField] private TextMeshProUGUI titleTXT;
        [SerializeField] private Image iconIMG;
        [SerializeField] private TextMeshProUGUI descriptionTXT;

        public override Type ItemType => null;

        public override void SetData(StorableScriptableObject item)
        {
            titleTXT.text = item.name;
            iconIMG.sprite = item.GetIconIfLoaded();
            descriptionTXT.text = item.Description;
        }
    }
}