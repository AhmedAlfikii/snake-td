using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.Cinemachine;
using TafraKit.GraphViews;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Combat/Damage/Send Damage In Shape"), GraphNodeName("Send Damage In Shape")]
    public class SendDamageInShapeNode : AbilityTaskNode
    {
        public enum DamageShape
        {
            Cube
        }

        [SerializeField] private DamageShape shape;
        [Tooltip("The center actor that the damage shape will be placed in accordance to. If null, the actor that has this ability will be used.")]
        [SerializeField] private BlackboardActorGetter centerPointActor = new BlackboardActorGetter();
        [Tooltip("The local offset of the damage shape from the character's position.")]
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 halfExtents;
        [SerializeField] private TafraLayerMask layerMask;
        [SerializeField] private BlackboardAdvancedFloatGetter damage = new BlackboardAdvancedFloatGetter();

        [Header("Time Freeze")]
        [SerializeField] private bool freezeTimeOnHit;
        [SerializeField] private float freezeDuration = 0.05f;

        [Header("Screen Shake")]
        [SerializeField] private bool shakeScreenOnHit;
        [SerializeField] private float amplitude = 0.25f;
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float shakeDuration = 0.25f;

        [NonSerialized] private Collider[] colliders = new Collider[20];

        public SendDamageInShapeNode(SendDamageInShapeNode other) : base(other)
        {
            shape = other.shape;
            centerPointActor = new BlackboardActorGetter(other.centerPointActor);
            offset = other.offset;
            halfExtents = other.halfExtents;
            layerMask = other.layerMask;
            damage = new BlackboardAdvancedFloatGetter(other.damage);
            freezeTimeOnHit = other.freezeTimeOnHit;
            freezeDuration = other.freezeDuration;
            shakeScreenOnHit = other.shakeScreenOnHit;
            amplitude = other.amplitude;
            frequency = other.frequency;
            shakeDuration = other.shakeDuration;
        }
        public SendDamageInShapeNode()
        {

        }

        protected override void OnInitialize()
        {
            centerPointActor.Initialize(ability.BlackboardCollection);
            damage.Initialize(ability.BlackboardCollection);
        }
        protected override void OnTriggerBlackboardSet()
        {
            centerPointActor.SetSecondaryBlackboard(triggerBlackboard);
        }
        protected override void OnStart()
        {
            TafraActor targetActor = centerPointActor.Value;

            if(targetActor == null)
                targetActor = actor;

            Transform actorTransform = targetActor.transform;

            Vector3 center = actorTransform.position;
            Vector3 position = center + actorTransform.TransformDirection(offset);
            int foundTargets = Physics.OverlapBoxNonAlloc(position, halfExtents, colliders, actorTransform.rotation, layerMask.Value);

            if(foundTargets == 0)
                return;

            bool dealtDamage = false;
            for (int i = 0; i < foundTargets; i++)
            {
                var col = colliders[i];

                Healthy healthy = ComponentProvider.GetComponent<Healthy>(col.gameObject);

                if(healthy == null)
                    continue;

                healthy.TakeDamage(new HitInfo(damage.Value.Value, actor, false));
                dealtDamage = true;
            }

            if(dealtDamage)
            {
                if (freezeTimeOnHit)
                    TimeScaler.SetTimeScaleForDuration(guid, 0, freezeDuration);
                if(shakeScreenOnHit)
                    ScreenShaker.Shake(amplitude, frequency, shakeDuration);
            }
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            SendDamageInShapeNode clonedNode = new SendDamageInShapeNode(this);

            return clonedNode;
        }
    }
}