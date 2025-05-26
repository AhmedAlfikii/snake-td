using UnityEngine;

namespace TafraKit.RPG
{
    [CreateAssetMenu(menuName = "Tafra Kit/RPG/Weapon Equipment", fileName = "Weapon Equipment")]
    public class WeaponEquipment : CombatEquipment
    {
        [SerializeField] private TafraFloat attackDamageMultiplier;
        [SerializeField] private TafraFloat attackSpeedMultiplier;

        public float AttackDamageMultiplier => attackDamageMultiplier.Value;
        public float AttackSpeedMultiplier => attackSpeedMultiplier.Value;
    }
}