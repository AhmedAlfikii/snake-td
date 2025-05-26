using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Weaponry
{
    /// <summary>
    /// A projectile that is guaranteed to hit it's target regardless of collisions in between or target position change.
    /// </summary>
    public class DirectedProjectile : SingleProjectile
    {
        [Tooltip("The speed of which the projectile will move towards its target")]
        [SerializeField] protected float speed = 30;
        [SerializeField] protected bool lookAtTargetWhileTraveling;

        protected void Update()
        {
            if(!isActive)
                return;

            if(targetDamagePoint == null)
            {
                FoundTarget(target, transform.position, -transform.forward);
                return;
            }

            Vector3 position = targetDamagePoint.position;

            if(!foundTarget)
            {
                transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);

                if(lookAtTargetWhileTraveling)
                    transform.LookAt(position);
            }

            //If reached target
            if((transform.position - position).sqrMagnitude < 0.01f)
                FoundTarget(target, transform.position, -transform.forward);
        }
    }
}