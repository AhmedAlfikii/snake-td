using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Character Controls/Motor/Look At Combat Point of Interest"), GraphNodeName("Look At CPoI")]
    public class LookAtCombatPoINode : AbilityTaskNode
    {
        [SerializeField] private float lookDuration = 0.25f;

        [NonSerialized] private CharacterCombat characterCombat;
        [NonSerialized] private Transform transform;
        [NonSerialized] private Transform combatPointOfInterest;
        [NonSerialized] private float startTime;
        [NonSerialized] private Quaternion startRotation;

        public LookAtCombatPoINode(LookAtCombatPoINode other) : base(other)
        {
            lookDuration = other.lookDuration;
            characterCombat = other.characterCombat;
            transform = other.transform;
            combatPointOfInterest = other.combatPointOfInterest;
        }
        public LookAtCombatPoINode()
        {

        }

        protected override void OnInitialize()
        {
            characterCombat = actor.GetCachedComponent<CharacterCombat>();
            transform = characterCombat.transform;
            combatPointOfInterest = characterCombat.PointOfInterest;
        }
        protected override void OnStart()
        {
            startTime = Time.time;
            startRotation = transform.rotation;
        }
        protected override BTNodeState OnUpdate()
        {
            if (characterCombat == null)
                return BTNodeState.Success;

            Vector3 combatPoIposition;
            Vector3 targetPoint;
            Quaternion targetRotation;

            if(lookDuration > 0.01f)
            {
                if(Time.time < startTime + lookDuration)
                {
                    float t = (Time.time - startTime) / lookDuration;

                    combatPoIposition = combatPointOfInterest.position;
                    targetPoint = new Vector3(combatPoIposition.x, transform.position.y, combatPoIposition.z);
                    targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

                    transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

                    return BTNodeState.Running;
                }
            }

            combatPoIposition = combatPointOfInterest.position;
            targetPoint = new Vector3(combatPoIposition.x, transform.position.y, combatPoIposition.z);
            targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

            transform.rotation = targetRotation;

            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            LookAtCombatPoINode clonedNode = new LookAtCombatPoINode(this);

            return clonedNode;
        }
    }
}