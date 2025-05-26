using TafraKit.Healthies;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit
{
    [SearchMenuItem("Equipment/Equip Equimpent")]
    public class EquipmentEquipperTestModule : ActionOnInputTestingModule
    {
        [SerializeField] private Equipment equipment;
        [SerializeField] private bool save = false;

        private CharacterEquipment characterEquipment;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            characterEquipment = characterTesting.GetComponent<CharacterEquipment>();
        }

        protected override void OnInputReceived()
        {
            characterEquipment.Equip(equipment, -1, save);
        }
    }
}