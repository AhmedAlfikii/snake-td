using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Consumables
{
    public abstract class OutOfConsumablePopup : MonoBehaviour
    {
        public abstract void Show(Consumable consumable, float requiredAmount,Action onConsumed = null);
        public abstract void Hide();
    }
}