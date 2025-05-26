using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.Healthies;
using TafraKit.ZTweeners;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.Weaponry
{
    public class MagicStaffAutoAttacking : RangedWeaponAutoAttacking
    {
        #region Serialized Fields
        [Tooltip("The number of projectiles that will be thrown in the same attack")]
        [SerializeField] private int shotsCount = 3;
        [Tooltip("The duration between each projectile shot in a single attack.")]
        [SerializeField] private float shotsInterval = 0.15f;
        #endregion

        #region Private Fields
        private List<Healthy> tempTargets = new List<Healthy>();
        #endregion

        #region Properties
        #endregion

        public override void StartAttackingTarget(Healthy target)
        {
            StartCoroutine(AttackProcess(target));
        }

        private IEnumerator AttackProcess(Healthy target)
        {
            animator.SetTrigger(animatorAttackTriggerHash);

            yield return Yielders.GetWaitForSeconds(shootDelay);

            List<Collider> detectedColliders = targetsDetector.GetSortedDetectedColliders();

            tempTargets.Clear();
            
            //Fill up the targets list.
            for(int i = 0; i < detectedColliders.Count; i++)
            {
                var collider = detectedColliders[i];

                float distanceSqr = (collider.transform.position - transform.position).sqrMagnitude;

                //If this target is out of range, then no need to look at the rest since it's an sorted list.
                if(distanceSqr > curAutoAttackRangeSqr)
                    break;

                Healthy targetHealthy = ComponentProvider.GetComponent<Healthy>(collider.gameObject);

                if(targetHealthy == null)
                    continue;

                tempTargets.Add(targetHealthy);

                if(tempTargets.Count >= shotsCount)
                    break;
            }

            if(tempTargets.Count == 0)
                yield break;

            int shotsMade = 0;
            while(shotsMade < shotsCount)
            {
                int skippedTargets = 0;
                for (int i = 0; i < tempTargets.Count; i++)
                {
                    var curTarget = tempTargets[i];

                    if(curTarget == null || curTarget.IsDead)
                    {
                        skippedTargets++;
                        continue;
                    }

                    //if(!fakeAttack)
                        PerformAttackAction(curTarget);
                    
                    shotsMade++;

                    if (shotsMade >= shotsCount) 
                        break;

                    yield return Yielders.GetWaitForSeconds(shotsInterval);
                }

                if (skippedTargets == tempTargets.Count)
                    break;
            }
        }
    }
}