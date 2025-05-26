using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Weaponry
{
    public class GroupedProjectiles : Projectile
    {
        [Header("Grouped Projectiles")]
        [SerializeField] protected Projectile[] projectiles;

        private int remainingLiveProjectiles;

        protected override void OnInitialize()
        {
            remainingLiveProjectiles = projectiles.Length;

            for(int i = 0; i < projectiles.Length; i++)
            {
                Projectile projectile = projectiles[i];

                projectile.SetDisposeOnDespawnWithoutPoolState(false);
                projectile.OnDespawn.RemoveListener(OnProjectileShouldDespawn);
                projectile.OnDespawn.AddListener(OnProjectileShouldDespawn);
            }
        }
        private void OnProjectileShouldDespawn()
        {
            remainingLiveProjectiles--;

            if(remainingLiveProjectiles <= 0)
            {
                isActive = false;

                Despawn();
            }
        }
    }
}