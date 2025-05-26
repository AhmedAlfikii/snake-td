using TafraKit.ModularSystem;
using UnityEngine;

namespace TafraKit.RPG
{
    [System.Serializable]
    public abstract class CharacterEquipmentModule : InternalModule
    {
        protected CharacterEquipment characterEquipment;

        public void Initialize(CharacterEquipment characterEquipment)
        {
            this.characterEquipment = characterEquipment;

            OnInitialize();
        }
    }
}