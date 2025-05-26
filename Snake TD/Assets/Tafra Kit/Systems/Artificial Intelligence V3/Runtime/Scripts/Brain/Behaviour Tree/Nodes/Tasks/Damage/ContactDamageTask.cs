using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;
using TafraKit.Healthies;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    /// <summary>
    /// Damages healthies it detects around the agent. Damage is applied to each healthy only once per task run.
    /// </summary>
    [SearchMenuItem("Tasks/Damage/Contact Damage"), GraphNodeName("Contact Damage", "Contact Damage")]
    public class ContactDamageTask : TaskNode
    {
        [SerializeField] private bool completeOnStart;
        [SerializeField] private TafraLayerMask layerMask = new TafraLayerMask(~0, null);
        [SerializeField] private BlackboardDynamicFloatGetter damage = new BlackboardDynamicFloatGetter(10);
        [SerializeField] private BlackboardDynamicFloatGetter height = new BlackboardDynamicFloatGetter(2f);
        [SerializeField] private BlackboardDynamicFloatGetter width = new BlackboardDynamicFloatGetter(0.5f);

        private Transform agentTransform;
        private Collider agentCollider;
        private Vector3 lastPosition;
        private RaycastHit[] raycastHits = new RaycastHit[10];
        private Collider[] colliderHits = new Collider[10];
        private HashSet<Collider> recentlyDamagedColliders = new HashSet<Collider>();

        protected override void OnInitialize()
        {
            agentTransform = agent.transform;
            agentCollider = agent.GetCachedComponent<Collider>();

            damage.Initialize(agent.BlackboardCollection);
            height.Initialize(agent.BlackboardCollection);
            width.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            lastPosition = agentTransform.position;
            recentlyDamagedColliders.Clear();
        }
        protected override BTNodeState OnUpdate()
        {
            float heightValue = height.Value;
            float radiusValue = width.Value / 2f;

            Vector3 curPosition = agentTransform.position;
            Vector3 point1 = lastPosition;
            Vector3 point2 = point1 + new Vector3(0, heightValue, 0);
            Vector3 dir = curPosition - lastPosition;
            float distance = dir.magnitude;

            if(distance > 0)
            {
                dir.Normalize();

                int hits = Physics.CapsuleCastNonAlloc(point1, point2, radiusValue, dir, raycastHits, distance, layerMask.Value);

                for(int i = 0; i < hits; i++)
                {
                    var hit = raycastHits[i];
                    
                    AttemptToDamageCollider(hit.collider);
                }
            }
            else
            {
                int hits = Physics.OverlapCapsuleNonAlloc(point1, point2, radiusValue, colliderHits, layerMask.Value);

                for(int i = 0; i < hits; i++)
                {
                    AttemptToDamageCollider(colliderHits[i]);
                }
            }

            lastPosition = curPosition;

            if(completeOnStart)
                return BTNodeState.Success;
            else
                return BTNodeState.Running;
        }

        private void AttemptToDamageCollider(Collider col)
        {
            if(col == agentCollider || recentlyDamagedColliders.Contains(col))
                return;

            Healthy healthy = col.GetComponent<Healthy>();

            if(healthy == null)
                return;

            healthy.TakeDamage(new HitInfo(damage.Value, agent));

            recentlyDamagedColliders.Add(col);
        }
    }
}