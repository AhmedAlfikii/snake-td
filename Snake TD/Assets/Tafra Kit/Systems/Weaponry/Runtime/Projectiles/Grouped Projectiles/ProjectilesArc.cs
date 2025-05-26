using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Weaponry
{
    public class ProjectilesArc : GroupedProjectiles
    {
        [Header("Projectiles Arc")]
        [SerializeField] private float angleBetweenProjectiles = 15f;
        [Tooltip("Should the arc ignore the target's Y offset? And use it's spawn point's Y instead.")]
        [SerializeField] private bool flattenYDirection = true;

        protected override void OnInitialize()
        {
            int projectilesCount = projectiles.Length;

            Vector3 direction = transform.forward;
            
            if (flattenYDirection)
                direction.y = 0;

            Quaternion rotationAngle = Quaternion.AngleAxis(angleBetweenProjectiles, Vector3.up);

            Vector3 curForward = Quaternion.AngleAxis(-angleBetweenProjectiles * ((projectilesCount - 1) / 2f), Vector3.up) * direction;

            for (int i = 0; i < projectilesCount; i++)
            {
                Projectile projectile = projectiles[i];

                projectile.gameObject.SetActive(false); //To make sure trails and trailing particles don't stretch from previous position to current one.
                projectile.transform.position = transform.position;
                projectile.gameObject.SetActive(true);

                //projectile.transform.forward = curForward;

                Debug.LogError("Why is the pool set to null here?");
                projectile.Initialize(hitInfo, spawnPoint, curForward, null, null, null);

                curForward = rotationAngle * curForward;
            }
        }
    }
}