using System.Collections;
using System.Collections.Generic;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit.Demos.RPGElements
{
    public class InventoryDemoCollector : MonoBehaviour
    {
        [SerializeField] private StorableScriptableObject storable;
        [SerializeField] private Inventory inventory;

        [ContextMenu("Collect")]
        public void Collect()
        {
            inventory.AddItem(storable);
        }
    }
}