using System;
using UnityEngine;

namespace TafraKit.RPG
{
    public abstract class InventoryItemInfoPanel : MonoBehaviour
    {
        protected Inventory inventory;

        public abstract Type ItemType { get; }

        public void Initialize(Inventory inventory)
        {
            this.inventory = inventory;
        }

        public abstract void SetData(StorableScriptableObject item);
    }
}