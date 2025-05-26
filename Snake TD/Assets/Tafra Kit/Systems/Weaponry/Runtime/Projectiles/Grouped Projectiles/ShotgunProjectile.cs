using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Weaponry
{
    public class ShotgunProjectile : GroupedProjectiles
    {
        [Header("Projectiles Arc")]
        [SerializeField] private float scatterSideAngle = 15f;
        [SerializeField] private float scatterUpAngle = 15f;
        [Tooltip("Should the arc ignore the target's Y offset? And use it's spawn point's Y instead.")]
        [SerializeField] private bool flattenYDirection = true;

        protected override void OnInitialize()
        {
            int projectilesCount = projectiles.Length;

            Vector3 direction = transform.forward;

            if (flattenYDirection)
                direction.y = transform.position.y;

            for (int i = 0; i < projectilesCount; i++)
            {
                Projectile projectile = projectiles[i];

                Quaternion rotationUpAngle = Quaternion.AngleAxis(Random.Range(-scatterSideAngle, scatterSideAngle), Vector3.up);
                Quaternion rotationRightAngle = Quaternion.AngleAxis(Random.Range(-scatterUpAngle, scatterUpAngle), Vector3.right);
                
                Vector3 curForward = rotationUpAngle * direction;
                curForward = rotationRightAngle * curForward;

                projectile.transform.position = transform.position;
                //projectile.transform.forward = curForward;

                Debug.LogError("Why is the pool set to null here?");
                projectile.Initialize(hitInfo, spawnPoint, curForward, null, null, null);
            }
        }
    }
}
