using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Weaponry
{
    public class BarrageProjectiles : GroupedProjectiles
    {
        [SerializeField] private float delayBetweenProjectiles = 0.1f;

        protected override void OnInitialize()
        {
            for(int i = 0; i < projectiles.Length; i++)
            {
                projectiles[i].gameObject.SetActive(false);
                StartCoroutine(FireProjectileAfterDelay(projectiles[i], delayBetweenProjectiles * i));
            }
        }

        IEnumerator FireProjectileAfterDelay(Projectile projectile, float delay) 
        {
            yield return Yielders.GetWaitForSecondsRealtime(delay);

            projectile.transform.position = spawnPoint;

            Vector3 direction = transform.forward;

            direction.y = 0;

            //projectile.transform.forward = direction;

            projectile.gameObject.SetActive(true);
            Debug.LogError("Why is the pool set to null here?");
            projectile.Initialize(hitInfo, spawnPoint, direction, target, null, null);
        }
    }
}