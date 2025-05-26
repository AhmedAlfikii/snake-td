using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.Weaponry
{
    public class RangedWeapon : WeaponManualAttacking
    {
        #region Fields
        [Header("Ranged Weapon Properties")]
        [SerializeField] protected Transform muzzle;

        protected UnityEvent<Projectile> onFiredProjectile = new UnityEvent<Projectile>();
        #endregion

        #region Properties
        public Transform Muzzle => muzzle;
        public UnityEvent<Projectile> OnFiredProjectile => onFiredProjectile;
        #endregion

        #region MonoBehaviour Messages
        #endregion

        protected override void OnInitialize()
        {

        }

        public void Fire(Projectile projectile)
        {
            onFiredProjectile?.Invoke(projectile);
        }
    }
}