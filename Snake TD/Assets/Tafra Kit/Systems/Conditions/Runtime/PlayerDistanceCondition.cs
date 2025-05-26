using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Conditions
{
    public class PlayerDistanceCondition : TickingCondition
    {
        [SerializeField] private Transform myTransform;
        [Tooltip("Use squared distance for optimization.")]
        [SerializeField] private bool useSquaredDistance = true;
        [SerializeField] private float distance;
        [SerializeField] private NumberRelation relation;

        Transform playerTransform;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            playerTransform = SceneReferences.PlayerHealthy.transform;
        }

        protected override bool PerformCheck()
        {
            Vector3 dir = playerTransform.position - myTransform.position;
            float d = useSquaredDistance ? dir.sqrMagnitude : dir.magnitude;
            return ZHelper.IsNumberRelationValid(d, distance, relation);
        }
    }
}