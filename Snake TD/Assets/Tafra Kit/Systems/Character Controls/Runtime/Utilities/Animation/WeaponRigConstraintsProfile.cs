using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [CreateAssetMenu(fileName = "Weapon Rig Constraints Profile", menuName = "Tafra Kit/Weaponry/Animations/Weapon Rig Constraints Profile")]
    public class WeaponRigConstraintsProfile : ScriptableObject
    {
        [SerializeField] private RigConstraintProperties[] rigConstraintProperties;

        public RigConstraintProperties[] RigConstraintProperties => rigConstraintProperties;
    }
}