using UnityEngine;

namespace TafraKit.RPG
{
    [System.Serializable]
    public abstract class RepeatingUpgradeModule : UpgradeModule
    {
        protected int recurrenceNumber;

        public abstract RepeatingUpgradeModule Clone(int recurrenceNumber);
    }
}