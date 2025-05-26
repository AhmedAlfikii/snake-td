using System.Collections;
using System.Collections.Generic;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit
{
    public class WeaponsProfileSetter : MonoBehaviour
    {
        [System.Serializable]
        public class WeaponProfile
        {
            public Equipment[] Weapons;
            public WeaponRigConstraintsProfile RigConstraintsProfile;
        }

        [SerializeField] private WeaponProfile[] weaponProfiles;
    }
}