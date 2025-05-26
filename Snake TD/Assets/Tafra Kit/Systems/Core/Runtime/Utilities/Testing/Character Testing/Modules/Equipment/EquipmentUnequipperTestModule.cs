using TafraKit.Healthies;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit
{
    [SearchMenuItem("Equipment/Unequip Equimpent")]
    public class EquipmentUnequipperTestModule : ActionOnInputTestingModule
    {
        [SerializeField] private TafraString slot;
        [SerializeField] private bool save = false;

        private CharacterEquipment characterEquipment;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            characterEquipment = characterTesting.GetComponent<CharacterEquipment>();
        }

        protected override void OnInputReceived()
        {
            characterEquipment.Unequip(slot.Value, null, save);
        }
    }
}