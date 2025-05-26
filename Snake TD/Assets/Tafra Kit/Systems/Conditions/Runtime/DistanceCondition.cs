using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Conditions
{
    public class DistanceCondition : TickingCondition
    {
        [SerializeField] private Transform targetA;
        [SerializeField] private Transform targetB;
        [Tooltip("Use squared distance for optimization.")]
        [SerializeField] private bool useSquaredDistance = true;
        [SerializeField] private float distance;
        [SerializeField] private NumberRelation relation;

        protected override bool PerformCheck()
        {
            Vector3 dir = targetB.position - targetA.position;

            float d = useSquaredDistance ? dir.sqrMagnitude : dir.magnitude;

            return ZHelper.IsNumberRelationValid(d, distance, relation);
        }
    }
}