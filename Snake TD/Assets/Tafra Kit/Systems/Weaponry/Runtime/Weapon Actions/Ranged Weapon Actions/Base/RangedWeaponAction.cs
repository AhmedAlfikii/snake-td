using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Weaponry
{
    public abstract class RangedWeaponAction : WeaponAction
    {
        protected RangedWeapon rangedWeapon;
        protected Transform weaponMuzzle;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            rangedWeapon = weapon as RangedWeapon;

            if(rangedWeapon == null)
            {
                TafraDebugger.Log("Ranged Weapon Action", "The weapon this action is assigned to is not ranged. The action will not work.", TafraDebugger.LogType.Error);
                return;
            }

            weaponMuzzle = rangedWeapon.Muzzle;
        }
    }
}